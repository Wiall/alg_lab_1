using System;
using System.Collections.Generic;
using System.IO;

namespace alg_lab_1
{
    class NaturalMergeSort
    {
        public static void Sort(string inputFile, string outputFile)
        {
            // Крок 1: Розбиття вхідного файлу на серії та розподіл у два файли A і B
            string fileA = Path.Combine(Path.GetTempPath(), "A.tmp");
            string fileB = Path.Combine(Path.GetTempPath(), "B.tmp");
            SplitIntoRuns(inputFile, fileA, fileB);

            // Крок 2: Злиття серій з файлів A та B, поки не залишиться один відсортований файл
            bool sorted = false;
            while (!sorted)
            {
                sorted = MergeRuns(fileA, fileB, outputFile);

                // Після кожного злиття обмінюємо ролі файлів A та B для наступного етапу
                string temp = fileA;
                fileA = fileB;
                fileB = temp;
            }

            // Переміщуємо остаточно відсортований файл
            if (File.Exists(outputFile))
            {
                File.Delete(outputFile); // Видаляємо наявний файл, якщо він існує
            }
            File.Move(fileA, outputFile); // Переміщуємо файл
        }

        private static void SplitIntoRuns(string inputFile, string fileA, string fileB)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            using (BinaryWriter writerA = new BinaryWriter(File.Open(fileA, FileMode.Create)))
            using (BinaryWriter writerB = new BinaryWriter(File.Open(fileB, FileMode.Create)))
            {
                List<int> currentRun = new List<int>();
                int? previousNumber = null;
                bool writeToA = true;

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int number = reader.ReadInt32();
                    Program.compCount++; // лічильник порівнянь

                    // Якщо число порушує зростаючий порядок, зберігаємо поточну серію і починаємо нову
                    if (previousNumber.HasValue && number < previousNumber.Value)
                    {
                        WriteRunToFile(currentRun, writeToA ? writerA : writerB);
                        currentRun.Clear();
                        writeToA = !writeToA; // чергуємо файли
                    }

                    currentRun.Add(number);
                    previousNumber = number;
                }

                // Записуємо залишкову серію
                if (currentRun.Count > 0)
                {
                    WriteRunToFile(currentRun, writeToA ? writerA : writerB);
                }
            }
        }

        private static void WriteRunToFile(List<int> run, BinaryWriter writer)
        {
            foreach (int number in run)
            {
                writer.Write(number);
            }
        }

        private static bool MergeRuns(string fileA, string fileB, string outputFile)
{
    bool mergedAny = false;
    
    // Список для збереження тимчасових файлів після злиття
    List<string> tempFiles = new List<string>();

    using (BinaryReader readerA = new BinaryReader(File.Open(fileA, FileMode.Open, FileAccess.Read, FileShare.Read)))
    using (BinaryReader readerB = new BinaryReader(File.Open(fileB, FileMode.Open, FileAccess.Read, FileShare.Read)))
    {
        while (readerA.BaseStream.Position < readerA.BaseStream.Length || readerB.BaseStream.Position < readerB.BaseStream.Length)
        {
            string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
            tempFiles.Add(tempFile);

            using (BinaryWriter writer = new BinaryWriter(File.Open(tempFile, FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                mergedAny = true;

                int a = readerA.BaseStream.Position < readerA.BaseStream.Length ? readerA.ReadInt32() : int.MaxValue;
                int b = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : int.MaxValue;

                while (readerA.BaseStream.Position < readerA.BaseStream.Length || readerB.BaseStream.Position < readerB.BaseStream.Length)
                {
                    Program.compCount++; // Лічильник порівнянь
                    if (a <= b)
                    {
                        writer.Write(a);
                        a = readerA.BaseStream.Position < readerA.BaseStream.Length ? readerA.ReadInt32() : int.MaxValue;
                    }
                    else
                    {
                        writer.Write(b);
                        b = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : int.MaxValue;
                    }
                }
            }
        }
    }

    // Переміщуємо злиті файли в A та B, чергуючи між ними
    bool toFileA = true;
    foreach (var tempFile in tempFiles)
    {
        if (toFileA)
        {
            File.Copy(tempFile, fileA, true);
        }
        else
        {
            File.Copy(tempFile, fileB, true);
        }
        File.Delete(tempFile); // Видаляємо тимчасовий файл після копіювання
        toFileA = !toFileA;
    }

    return !mergedAny; // Повертаємо, чи завершене сортування
}


        private static List<int> ReadNextRun(BinaryReader reader)
        {
            List<int> run = new List<int>();
            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                int number = reader.ReadInt32();
                run.Add(number);
                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int nextNumber = reader.ReadInt32();
                    if (nextNumber < number) break;
                    run.Add(nextNumber);
                    number = nextNumber;
                }
            }
            return run;
        }

        private static void MergeAndWriteRuns(List<int> runA, List<int> runB, BinaryWriter writerA, BinaryWriter writerB)
        {
            int i = 0, j = 0;
            bool writeToA = true;

            while (i < runA.Count && j < runB.Count)
            {
                if (runA[i] <= runB[j])
                {
                    (writeToA ? writerA : writerB).Write(runA[i++]);
                }
                else
                {
                    (writeToA ? writerA : writerB).Write(runB[j++]);
                }
            }

            // Записуємо залишок
            while (i < runA.Count)
                (writeToA ? writerA : writerB).Write(runA[i++]);
            while (j < runB.Count)
                (writeToA ? writerA : writerB).Write(runB[j++]);

            // Після кожного злиття серій чергуємо записувач
            writeToA = !writeToA;
        }
    }
}
