using System;
using System.IO;
using System.Diagnostics;

namespace alg_lab_1
{
    class Program
    {
        public static long compCount = 0;
        public static long seriesSize;
        
        public static string[] options1 = new string[] 
        {
            "Оригiнальний метод сортування природнього злиття",
            "Модифікований метод сортування (фiксовані впорядкованi серii)",
            "Модифікований метод сортування (фiксовані впорядкованi серii + буфернi читання/запис)",
            "Модифікований метод сортування (фiксовані впорядкованi серii + буфернi читання/запис + кiлькiсть ддопомiжних файлiв = кiлькостi серiй)"
        };
        public static string[] options2 = new string[] 
        {
            "Стандартний метод генерацii",
            "Модифiкований метод генерацii"
        };
        static void Main()
        {
            
            string inputFile = "input.dat";
            string nameCopy = "inputCopy.dat";
            string outputFile = "output.dat";
            string inputFileFormatted = "input.txt";
            string outputFileFormatted = "output.txt";
            
            Console.WriteLine("Оберiть метод сортування:");
            int choice = MenuHolder.HoldMenu(options1);
            
            Console.WriteLine("Виберiть метод генерацii даних:");
            int dataGenerationChoice = MenuHolder.HoldMenu(options2);
            
            Console.WriteLine("Введiть розмiр файлу в мегабайтах (МБ):");
            if (!long.TryParse(Console.ReadLine(), out long fileSizeMB) || fileSizeMB <= 0)
            {
                Console.WriteLine("Неправильний ввiд. Будь ласка, введiть дiйсне додатне число.");
                return;
            }

            long fileSizeInBytes = fileSizeMB * 1024 * 1024;
            if (dataGenerationChoice == 0)
                DataGenerator.GenerateRandomData(inputFile, fileSizeInBytes);
            else if (dataGenerationChoice == 1)
                DataGenerator.GenerateRandomDataModified(inputFile, fileSizeInBytes, 16);
            else
            {
                Console.WriteLine("Неправильний вибiр. Будь ласка, спробуйте ще раз.");
                return;
            }

            using (FileStream sourceStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(nameCopy, FileMode.Create, FileAccess.Write))
            {
                sourceStream.CopyTo(destinationStream);
            }
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            switch (choice)
            {
                case 0:
                    NaturalMergeSort.Sort(inputFile, outputFile);
                    break;
                case 1:
                    NaturalMergeSortModified.Sort(inputFile, outputFile, seriesSize);
                    break;
                case 2:
                    NaturalMergeSortBuffered.Sort(inputFile, outputFile, seriesSize);
                    break;
                case 3:
                    NaturalMergeSortImproved.Sort(inputFile, outputFile, seriesSize);
                    break;
                default:
                    Console.WriteLine("Неправильний вибiр. Будь ласка, спробуйте ще раз.");
                    return;
            }
            stopwatch.Stop();

            TimeSpan timeTaken = stopwatch.Elapsed;
            Console.WriteLine("Сортування завершено. Вiдсортованi данi знаходяться в " + outputFile);
            Console.WriteLine($"Час виконання: {timeTaken.TotalSeconds} секунд");
            Console.WriteLine($"Кiлькість порiвнянь: {compCount}");
            
            ConvertToText.Convert(nameCopy, "inputFileFormatted.txt");
            ConvertToText.Convert("output.dat", "outputFileFormatted.txt");

            Console.WriteLine($"Файли перетворено в текстовi документи: {inputFileFormatted} та {outputFileFormatted}");
        }
    }
}

