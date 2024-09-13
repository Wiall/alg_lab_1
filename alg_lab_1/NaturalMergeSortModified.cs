using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace alg_lab_1
{
    class NaturalMergeSortModified
    {
        public static void Sort(string inputFile, string outputFile, int initialSeriesSize)
        {
            int seriesSize = initialSeriesSize;

            // Крок 1: Розбиття вхідного файлу на серії відомого розміру та запис у два файли A та B
            string fileA = Path.Combine(Path.GetTempPath(), "A.tmp");
            string fileB = Path.Combine(Path.GetTempPath(), "B.tmp");

            SplitIntoRuns(inputFile, seriesSize, fileA, fileB);

            // Крок 2: Злиття серій до тих пір, поки не залишиться один відсортований файл
            string currentInputFile = fileA;
            string nextOutputFile = fileB;

            while (true)
            {
                int mergedCount = MergeRuns(currentInputFile, nextOutputFile, outputFile);

                if (mergedCount == 1)
                {
                    break; // Сортування завершено
                }

                // Обмінюємо файли A та B для наступного злиття
                string temp = currentInputFile;
                currentInputFile = nextOutputFile;
                nextOutputFile = temp;

                // Подвоюємо розмір серії для наступного етапу злиття
                seriesSize *= 2;
            }

            // Переміщуємо остаточно відсортований файл
            File.Move(currentInputFile, outputFile);
        }

        private static void SplitIntoRuns(string inputFile, int seriesSize, string fileA, string fileB)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            using (BinaryWriter writerA = new BinaryWriter(File.Open(fileA, FileMode.Create)))
            using (BinaryWriter writerB = new BinaryWriter(File.Open(fileB, FileMode.Create)))
            {
                List<int> currentRun = new List<int>();
                bool writeToA = true;

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    if (currentRun.Count < seriesSize)
                    {
                        currentRun.Add(reader.ReadInt32());
                    }
                    else
                    {
                        WriteRunToFile(currentRun, writeToA ? writerA : writerB);
                        currentRun.Clear();
                        writeToA = !writeToA; // чергуємо файли для запису
                    }
                }

                // Записуємо залишкові дані
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

        private static int MergeRuns(string fileA, string fileB, string outputFile)
        {
            int mergedRuns = 0;

            using (BinaryReader readerA = new BinaryReader(File.Open(fileA, FileMode.Open)))
            using (BinaryReader readerB = new BinaryReader(File.Open(fileB, FileMode.Open)))
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
            {
                bool aFinished = false, bFinished = false;
                int numA = int.MaxValue, numB = int.MaxValue;

                if (readerA.BaseStream.Position < readerA.BaseStream.Length)
                    numA = readerA.ReadInt32();
                else
                    aFinished = true;

                if (readerB.BaseStream.Position < readerB.BaseStream.Length)
                    numB = readerB.ReadInt32();
                else
                    bFinished = true;

                while (!aFinished || !bFinished)
                {
                    if (aFinished || (!bFinished && numB <= numA))
                    {
                        writer.Write(numB);
                        if (readerB.BaseStream.Position < readerB.BaseStream.Length)
                            numB = readerB.ReadInt32();
                        else
                            bFinished = true;
                    }
                    else
                    {
                        writer.Write(numA);
                        if (readerA.BaseStream.Position < readerA.BaseStream.Length)
                            numA = readerA.ReadInt32();
                        else
                            aFinished = true;
                    }
                }

                mergedRuns++;
            }

            return mergedRuns;
        }
    }
}
