﻿using System;
using System.IO;

namespace alg_lab_1
{
    public static class NaturalMergeSortModified
    {
        // Метод для виконання модифікованого природного злиття
        public static void Sort(string inputFile, string outputFile, long initialSeriesSize)
        {
            string fileB = "fileB_modified.bin";
            string fileC = "fileC_modified.bin";
            
            long currentSeriesSize = initialSeriesSize;

            // Поки розмір серії не перевищить кількість елементів у файлі, продовжуємо сортування
            while (!IsSorted(inputFile, currentSeriesSize))
            {
                Console.WriteLine("Вiдбувається сортування...");
                // Поділ файлу на серії відомого розміру і розподіл між fileB та fileC
                SplitFile(inputFile, fileB, fileC, currentSeriesSize);

                // Злиття fileB та fileC назад у inputFile з поточним розміром серії
                MergeFiles(inputFile, fileB, fileC, currentSeriesSize);

                // Подвоюємо розмір серії для наступного кроку
                currentSeriesSize *= 2;
            }

            // Після завершення процесу копіюємо відсортовані дані в outputFile
            File.Copy(inputFile, outputFile, true);
        }

        // Перевірка чи файл повністю відсортований, використовуючи відомий розмір серії
        private static bool IsSorted(string filename, long seriesSize)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                if (reader.BaseStream.Length == 0)
                    return true;

                long fileSize = reader.BaseStream.Length;
                int elementsInFile = (int)(fileSize / sizeof(int));

                // Якщо розмір серії перевищує кількість елементів, файл відсортований
                return seriesSize >= elementsInFile;
            }
        }

        // Розподіл файлу на серії відомого розміру між fileB та fileC
        private static void SplitFile(string inputFile, string fileB, string fileC, long seriesSize)
        {
            Console.WriteLine("Вiдбувається розділення...");
            Console.WriteLine($"Серія з {seriesSize} елементів");
            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            using (BinaryWriter writerB = new BinaryWriter(File.Open(fileB, FileMode.Create)))
            using (BinaryWriter writerC = new BinaryWriter(File.Open(fileC, FileMode.Create)))
            {
                bool writeToB = true;
                long elementsInSeries = 0;

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    // Console.WriteLine("РОЗДІЛЕННЯ");
                    int current = reader.ReadInt32();
                    (writeToB ? writerB : writerC).Write(current);
                    elementsInSeries++;
                    Console.WriteLine($"Write to B: {writeToB}");
                    // Якщо кількість елементів у серії досягла розміру серії, змінюємо файл
                    if (elementsInSeries >= seriesSize)
                    {
                        writeToB = !writeToB;
                        elementsInSeries = 0;
                    }
                }
            }
        }

        // Злиття серій з fileB і fileC назад у inputFile з відомим розміром серії
        private static void MergeFiles(string outputFile, string fileB, string fileC, long seriesSize)
        {
            Console.WriteLine("Вiдбувається злиття...");
            using (BinaryReader readerB = new BinaryReader(File.Open(fileB, FileMode.Open)))
            using (BinaryReader readerC = new BinaryReader(File.Open(fileC, FileMode.Open)))
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
            {
                int? valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                int? valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;

                int countB = 0;
                int countC = 0;

                while (valueB.HasValue || valueC.HasValue)
                {
                    // Якщо одна серія закінчилась у fileB, копіюємо елементи з fileC
                    if (countB >= seriesSize)
                    {
                        writer.Write(valueC.Value);
                        valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;
                        countC++;
                        if (countC >= seriesSize)
                        {
                            countB = countC = 0;
                        }
                    }
                    // Якщо одна серія закінчилась у fileC, копіюємо елементи з fileB
                    else if (countC >= seriesSize)
                    {
                        writer.Write(valueB.Value);
                        valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                        countB++;
                        if (countB >= seriesSize)
                        {
                            countB = countC = 0;
                        }
                    }
                    else
                    {
                        Program.compCount++;
                        // Порівнюємо і зливаємо поточні елементи серій з fileB і fileC
                        if (valueB <= valueC)
                        {
                            writer.Write(valueB.Value);
                            valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                            countB++;
                        }
                        else
                        {
                            writer.Write(valueC.Value);
                            valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;
                            countC++;
                        }

                        // Якщо обидві серії завершилися, скидаємо лічильники
                        if (countB >= seriesSize && countC >= seriesSize)
                        {
                            countB = countC = 0;
                        }
                    }
                }

                // Якщо одна серія закінчилася, залишок іншої копіюється до кінця
                while (valueB.HasValue)
                {
                    writer.Write(valueB.Value);
                    valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                }

                while (valueC.HasValue)
                {
                    writer.Write(valueC.Value);
                    valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;
                }
            }
        }
    }
}
