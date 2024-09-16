using System;
using System.IO;

namespace alg_lab_1
{
    class ConvertToText
    {
        private const int BufferSize = 1024 * 1024; // 1 MB
        
        public static void Convert(string inputFile, string outputTextFile)
        {
            using (var reader = new BufferedStream(new FileStream(inputFile, FileMode.Open, FileAccess.Read), BufferSize))
            using (var writer = new StreamWriter(outputTextFile))
            {
                int[] buffer = new int[BufferSize / sizeof(int)];
                int bytesRead;

                while ((bytesRead = ReadSeries(reader, buffer, buffer.Length)) > 0)
                {
                    for (int i = 0; i < bytesRead; i++)
                    {
                        writer.WriteLine(buffer[i]);
                    }
                }
            }
        }
        
        private static int ReadSeries(BufferedStream reader, int[] buffer, int seriesSize)
        {
            byte[] bytes = new byte[seriesSize * sizeof(int)];
            int bytesRead = reader.Read(bytes, 0, bytes.Length);
            Buffer.BlockCopy(bytes, 0, buffer, 0, bytesRead);
            return bytesRead / sizeof(int);
        }
    }

}