using System;
using System.IO;

namespace alg_lab_1
{
    public static class NaturalMergeSortBuffered
    {
        public static void Sort(string inputFile, string outputFile, long initialSeriesSize)
        {
            string fileB = "fileB_modified.bin";
            string fileC = "fileC_modified.bin";
            
            long currentSeriesSize = initialSeriesSize;

            while (!IsSorted(inputFile, currentSeriesSize))
            {
                Console.WriteLine("Вiдбувається сортування...");

                // Паралельний поділ файлів
                SplitFile(inputFile, fileB, fileC, currentSeriesSize);
                MergeFiles(inputFile, fileB, fileC, currentSeriesSize);

                currentSeriesSize *= 2;
            }

            File.Copy(inputFile, outputFile, true);
        }

        private static bool IsSorted(string filename, long seriesSize)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                if (reader.BaseStream.Length == 0)
                    return true;

                long fileSize = reader.BaseStream.Length;
                int elementsInFile = (int)(fileSize / sizeof(int));

                return seriesSize >= elementsInFile;
            }
        }

        private static void SplitFile(string inputFile, string fileB, string fileC, long seriesSize)
        {
            Console.WriteLine("Вiдбувається розділення...");
            Console.WriteLine($"Серія з {seriesSize} елементів");

            const int bufferSize = 1024 * 1024 * 200;
            int[] buffer = new int[bufferSize];
            long elementsInSeries = 0;

            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            using (BinaryWriter writerB = new BinaryWriter(new BufferedStream(File.Open(fileB, FileMode.Create), bufferSize * sizeof(int))))
            using (BinaryWriter writerC = new BinaryWriter(new BufferedStream(File.Open(fileC, FileMode.Create), bufferSize * sizeof(int))))
            {
                bool writeToB = true;

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int count = 0;

                    for (int i = 0; i < bufferSize && reader.BaseStream.Position < reader.BaseStream.Length; i++)
                    {
                        buffer[i] = reader.ReadInt32();
                        count++;
                    }

                    for (int i = 0; i < count; i++)
                    {
                        (writeToB ? writerB : writerC).Write(buffer[i]);
                        elementsInSeries++;

                        if (elementsInSeries >= seriesSize)
                        {
                            writeToB = !writeToB;
                            elementsInSeries = 0;
                        }
                    }
                }
            }
        }

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

                        if (countB >= seriesSize && countC >= seriesSize)
                        {
                            countB = countC = 0;
                        }
                    }
                }

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
