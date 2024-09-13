using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace alg_lab_1
{
    class Program
    {
        public static long compCount = 0;
        public static int seriesSize;
        
        static void Main()
        {
            
   
            string inputFile = "input.bin";
            string outputFile = "output.bin";
            string inputFileFormatted = "input.txt";
            string outputFileFormatted = "output.txt";

            Console.WriteLine("Оберiть метод сортування:");
            Console.WriteLine("1 - Немодифiкований метод сортування");
            Console.WriteLine("2 - Модифiкований метод сортування");

            string choice = Console.ReadLine();

            Console.WriteLine("Виберiть метод генерацii даних:");
            Console.WriteLine("1 - Стандартний метод генерацii");
            Console.WriteLine("2 - Модифiкований метод генерацii");

            string dataGenerationChoice = Console.ReadLine();

            // Запитуємо користувача про розмір файлу
            Console.WriteLine("Введiть розмiр файлу в мегабайтах (МБ):");
            if (!int.TryParse(Console.ReadLine(), out int fileSizeMB) || fileSizeMB <= 0)
            {
                Console.WriteLine("Неправильний ввiд. Будь ласка, введiть дiйсне додатне число.");
                return;
            }
            
            int fileSizeInBytes = fileSizeMB * 1024 * 1024; // Конвертуємо розмір з МБ в байти
            // Генеруємо дані для сортування
            if (dataGenerationChoice == "1")
                DataGenerator.GenerateRandomData(inputFile, fileSizeInBytes);
            else if (dataGenerationChoice == "2")
                DataGenerator.GenerateRandomDataModified(inputFile, fileSizeInBytes, 8);
            else
            {
                Console.WriteLine("Неправильний вибiр. Будь ласка, спробуйте ще раз.");
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            
            switch (choice)
            {
                case "1":
                    Console.WriteLine("Використовується немодифiкований метод сортування...");
                    stopwatch.Start();
                    NaturalMergeSort.Sort(inputFile, outputFile);
                    stopwatch.Stop();
                    break;
                case "2":
                    Console.WriteLine("Використовується модифiкований метод сортування...");
                    stopwatch.Start();
                    NaturalMergeSortModified.Sort(inputFile, outputFile, seriesSize);
                    stopwatch.Stop();
                    break;
                default:
                    Console.WriteLine("Неправильний вибiр. Будь ласка, спробуйте ще раз.");
                    return;
            }

            TimeSpan timeTaken = stopwatch.Elapsed;
            Console.WriteLine("Сортування завершено. Вiдсортованi данi знаходяться в " + outputFile);
            Console.WriteLine($"Час виконання: {timeTaken.TotalSeconds} секунд");
            Console.WriteLine($"Кiлькість порiвнянь: {compCount}");
            
            BinaryTextConverter.ConvertBinaryToText("input.bin", "inputFileFormatted.txt");
            BinaryTextConverter.ConvertBinaryToText("output.bin", "outputFileFormatted.txt");

            Console.WriteLine("Бiнарнi файли перетворено в текстовi: {inputFileFormatted} та {outputFileFormatted}");
        }
    }
}

