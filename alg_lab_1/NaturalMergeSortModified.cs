using System;
using System.IO;

namespace alg_lab_1
{
    public static class NaturalMergeSortModified
    {
        public static void Sort(string inputFile, string outputFile, long initialSeriesSize)
        {
            string fileB = "fileB_modified.dat";
            string fileC = "fileC_modified.dat";
            
            long currentSeriesSize = initialSeriesSize;

            while (!IsSorted(inputFile, currentSeriesSize))
            {
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
            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            using (BinaryWriter writerB = new BinaryWriter(File.Open(fileB, FileMode.Create)))
            using (BinaryWriter writerC = new BinaryWriter(File.Open(fileC, FileMode.Create)))
            {
                bool writeToB = true;
                long elementsInSeries = 0;

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int current = reader.ReadInt32();
                    (writeToB ? writerB : writerC).Write(current);
                    elementsInSeries++;
                    if (elementsInSeries >= seriesSize)
                    {
                        writeToB = !writeToB;
                        elementsInSeries = 0;
                    }
                }
            }
        }
        
        private static void MergeFiles(string outputFile, string fileB, string fileC, long seriesSize)
        {
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
