using System;
using System.IO;

namespace alg_lab_1
{
    public static class NaturalMergeSort
    {
        public static void Sort(string inputFile, string outputFile)
        {
            string fileB = "fileB.dat";
            string fileC = "fileC.dat";
            
            while (!IsSorted(inputFile))
            {
                SplitFile(inputFile, fileB, fileC);
                MergeFiles(inputFile, fileB, fileC);
            }

            File.Copy(inputFile, outputFile, true);
        }
        
        private static bool IsSorted(string filename)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filename, FileMode.Open)))
            {
                if (reader.BaseStream.Length == 0)
                    return true;

                int prev = reader.ReadInt32();

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int current = reader.ReadInt32();
                    if (current < prev)
                        return false;

                    prev = current;
                }
            }
            return true;
        }

        private static void SplitFile(string inputFile, string fileB, string fileC)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
            using (BinaryWriter writerB = new BinaryWriter(File.Open(fileB, FileMode.Create)))
            using (BinaryWriter writerC = new BinaryWriter(File.Open(fileC, FileMode.Create)))
            {
                bool writeToB = true;
                int prev = reader.ReadInt32();
                (writeToB ? writerB : writerC).Write(prev);

                while (reader.BaseStream.Position < reader.BaseStream.Length)
                {
                    int current = reader.ReadInt32();

                    Program.compCount++;
                    if (current < prev)
                        writeToB = !writeToB;

                    (writeToB ? writerB : writerC).Write(current);
                    prev = current;
                }
            }
        }
        
        private static void MergeFiles(string outputFile, string fileB, string fileC)
        {
            using (BinaryReader readerB = new BinaryReader(File.Open(fileB, FileMode.Open)))
            using (BinaryReader readerC = new BinaryReader(File.Open(fileC, FileMode.Open)))
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputFile, FileMode.Create)))
            {
                int? valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                int? valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;

                while (valueB.HasValue || valueC.HasValue)
                {
                    if (!valueB.HasValue)
                    {
                        writer.Write(valueC.Value);
                        valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;
                    }
                    else if (!valueC.HasValue)
                    {
                        writer.Write(valueB.Value);
                        valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                    }
                    else
                    {
                        Program.compCount++;
                        if (valueB <= valueC)
                        {
                            writer.Write(valueB.Value);
                            valueB = readerB.BaseStream.Position < readerB.BaseStream.Length ? readerB.ReadInt32() : (int?)null;
                        }
                        else
                        {
                            writer.Write(valueC.Value);
                            valueC = readerC.BaseStream.Position < readerC.BaseStream.Length ? readerC.ReadInt32() : (int?)null;
                        }
                    }
                }
            }
        }
    }
}
