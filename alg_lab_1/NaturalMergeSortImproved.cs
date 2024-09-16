namespace alg_lab_1
{
using System;
using System.Collections.Generic;
using System.IO;

class NaturalMergeSortImproved
{
    // Розмір буфера для читання/запису (залежить від доступної пам'яті)
    private const int BufferSize = 1024 * 1024; // 1 MB

    // Основний метод сортування
    public static void Sort(string inputFile, string outputFile, long seriesSize)
    {
        List<string> tempFiles = SplitIntoSeries(inputFile, seriesSize);
        
        while (tempFiles.Count > 1)
        {
            List<string> mergedFiles = new List<string>();
            for (int i = 0; i < tempFiles.Count; i += 2)
            {
                if (i + 1 < tempFiles.Count)
                {
                    string mergedFile = Path.GetTempFileName();
                    MergeFiles(tempFiles[i], tempFiles[i + 1], mergedFile);
                    mergedFiles.Add(mergedFile);
                    // Видаляємо тимчасові файли після злиття
                    File.Delete(tempFiles[i]);
                    File.Delete(tempFiles[i + 1]);
                }
                else
                {
                    mergedFiles.Add(tempFiles[i]);
                }
            }
            tempFiles = mergedFiles;
        }

        // Переміщаємо результат у вихідний файл
        File.Move(tempFiles[0], outputFile);
    }

    // Розбиває вхідний файл на серії
    private static List<string> SplitIntoSeries(string inputFile, long seriesSize)
    {
        List<string> tempFiles = new List<string>();
        using (var reader = new BufferedStream(new FileStream(inputFile, FileMode.Open, FileAccess.Read), BufferSize))
        {
            int[] buffer = new int[seriesSize];
            int bytesRead;
            while ((bytesRead = ReadSeries(reader, buffer, seriesSize)) > 0)
            {
                string tempFile = Path.GetTempFileName();
                using (var writer = new BufferedStream(new FileStream(tempFile, FileMode.Create, FileAccess.Write), BufferSize))
                {
                    WriteSeries(writer, buffer, bytesRead);
                }
                tempFiles.Add(tempFile);
            }
        }
        return tempFiles;
    }

    // Читає серію елементів із файлу
    private static int ReadSeries(BufferedStream reader, int[] buffer, long seriesSize)
    {
        byte[] bytes = new byte[seriesSize * sizeof(int)];
        int bytesRead = reader.Read(bytes, 0, bytes.Length);
        Buffer.BlockCopy(bytes, 0, buffer, 0, bytesRead);
        return bytesRead / sizeof(int);
    }

    // Записує серію елементів у файл
    private static void WriteSeries(BufferedStream writer, int[] buffer, int length)
    {
        byte[] bytes = new byte[length * sizeof(int)];
        Buffer.BlockCopy(buffer, 0, bytes, 0, bytes.Length);
        writer.Write(bytes, 0, bytes.Length);
    }

    // Злиття двох файлів в один
    private static void MergeFiles(string file1, string file2, string outputFile)
    {
        using (var reader1 = new BufferedStream(new FileStream(file1, FileMode.Open, FileAccess.Read), BufferSize))
        using (var reader2 = new BufferedStream(new FileStream(file2, FileMode.Open, FileAccess.Read), BufferSize))
        using (var writer = new BufferedStream(new FileStream(outputFile, FileMode.Create, FileAccess.Write), BufferSize))
        {
            int[] buffer1 = new int[BufferSize / sizeof(int)];
            int[] buffer2 = new int[BufferSize / sizeof(int)];
            int len1 = ReadSeries(reader1, buffer1, buffer1.Length);
            int len2 = ReadSeries(reader2, buffer2, buffer2.Length);
            int i = 0, j = 0;

            while (i < len1 && j < len2)
            {
                if (buffer1[i] <= buffer2[j])
                {
                    WriteInt(writer, buffer1[i]);
                    i++;
                }
                else
                {
                    WriteInt(writer, buffer2[j]);
                    j++;
                }

                if (i == len1)
                {
                    len1 = ReadSeries(reader1, buffer1, buffer1.Length);
                    i = 0;
                }

                if (j == len2)
                {
                    len2 = ReadSeries(reader2, buffer2, buffer2.Length);
                    j = 0;
                }
            }

            // Дозаписуємо залишки з обох файлів
            WriteRemaining(writer, reader1, buffer1, i, len1);
            WriteRemaining(writer, reader2, buffer2, j, len2);
        }
    }

    // Записує окреме значення в потік
    private static void WriteInt(BufferedStream writer, int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        writer.Write(bytes, 0, bytes.Length);
    }

    // Записує залишки серії у файл
    private static void WriteRemaining(BufferedStream writer, BufferedStream reader, int[] buffer, int index, int length)
    {
        for (int i = index; i < length; i++)
        {
            WriteInt(writer, buffer[i]);
        }

        while ((length = ReadSeries(reader, buffer, buffer.Length)) > 0)
        {
            WriteSeries(writer, buffer, length);
        }
    }
}
}