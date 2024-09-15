using System;
using System.IO;

namespace alg_lab_1
{
    public static class DataGenerator
    {
        public static void GenerateRandomData(string filename, long sizeInBytes)
        {
            Random random = new Random();
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                int currentSize = 0;

                while (currentSize < sizeInBytes)
                {
                    int number = random.Next(0, 1000000); // Генеруємо випадкове число
                    writer.Write(number); // Записуємо число як 4 байти (int)
                    currentSize += sizeof(int); // Додаємо розмір int (4 байти)
                }
            }
        }

        public static void GenerateRandomDataModified(string filename, long sizeInBytes, int seriesCount = 8)
        {
            Console.WriteLine("Виконується генерацiя файлів...");
            Random random = new Random();
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                int maxNumber = 1000000; // Максимальне число для генерації
                long numbersPerSeries = sizeInBytes / seriesCount / sizeof(int); // Розмір серії у числах
                Program.seriesSize = numbersPerSeries;

                for (int i = 0; i < seriesCount; i++)
                {
                    int currentNumber = random.Next(0, maxNumber); // Початкове число в серії

                    for (int j = 0; j < numbersPerSeries; j++)
                    {
                        writer.Write(currentNumber);
                        currentNumber += random.Next(1, 100); // Додаємо випадкове мале число для збереження зростаючого порядку
                    }
                }
            }
        }

    }
}