using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
class NaturalMergeSort
{
    public static void Sort(string inputFile, string outputFile)
    {
        // Крок 1: Розбиття вхідного файлу на природні серії
        List<string> seriesFiles = SplitIntoRuns(inputFile);

        // Крок 2: Злиття серій до тих пір, поки не залишиться один відсортований файл
        while (seriesFiles.Count > 1)
        {
            List<string> newSeriesFiles = new List<string>();

            for (int i = 0; i < seriesFiles.Count - 1; i += 2)
            {
                string mergedFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
                MergeFiles(seriesFiles[i], seriesFiles[i + 1], mergedFile);
                newSeriesFiles.Add(mergedFile);

                // Видаляємо тимчасові файли, щоб звільнити місце
                File.Delete(seriesFiles[i]);
                File.Delete(seriesFiles[i + 1]);
            }

            // Якщо залишився один файл, переносимо його далі
            if (seriesFiles.Count % 2 != 0)
                newSeriesFiles.Add(seriesFiles.Last());

            seriesFiles = newSeriesFiles;
        }

        // Остаточно відсортований файл
        File.Move(seriesFiles[0], outputFile);
    }

    private static List<string> SplitIntoRuns(string inputFile)
    {
        List<string> seriesFiles = new List<string>();
        using (StreamReader reader = new StreamReader(inputFile))
        {
            List<int> currentRun = new List<int>();
            string line;

            if ((line = reader.ReadLine()) != null)
                currentRun.Add(int.Parse(line));

            while ((line = reader.ReadLine()) != null)
            {
                int number = int.Parse(line);
                if (number < currentRun.Last())
                {
                    seriesFiles.Add(SaveRunToFile(currentRun));
                    currentRun.Clear();
                }
                currentRun.Add(number);
            }

            if (currentRun.Count > 0)
                seriesFiles.Add(SaveRunToFile(currentRun));
        }
        return seriesFiles;
    }

    private static void MergeFiles(string file1, string file2, string outputFile)
    {
        using (StreamReader reader1 = new StreamReader(file1))
        using (StreamReader reader2 = new StreamReader(file2))
        using (StreamWriter writer = new StreamWriter(outputFile))
        {
            string line1 = reader1.ReadLine();
            string line2 = reader2.ReadLine();

            while (line1 != null && line2 != null)
            {
                int num1 = int.Parse(line1);
                int num2 = int.Parse(line2);

                if (num1 <= num2)
                {
                    writer.WriteLine(num1);
                    line1 = reader1.ReadLine();
                }
                else
                {
                    writer.WriteLine(num2);
                    line2 = reader2.ReadLine();
                }
            }

            // Записуємо залишкові рядки
            while (line1 != null)
            {
                writer.WriteLine(line1);
                line1 = reader1.ReadLine();
            }

            while (line2 != null)
            {
                writer.WriteLine(line2);
                line2 = reader2.ReadLine();
            }
        }
    }
    private static string SaveRunToFile(List<int> run)
    {
        // Генеруємо унікальне ім'я файлу в тимчасовому каталозі
        string tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".tmp");
    
        using (StreamWriter writer = new StreamWriter(tempFile))
        {
            foreach (int number in run)
                writer.WriteLine(number);
        }
        return tempFile;
    }

}


partial class Program
{
    static void Main()
    {
        // Generate random data for the input file (not less than 10 MB in size)
        string inputFile = "input.txt";
        string outputFile = "output.txt";
        GenerateRandomData(inputFile, 1024 * 1024); // 10 MB

        // Sort the file
        NaturalMergeSort.Sort(inputFile, outputFile);

        Console.WriteLine("Sorting complete. Sorted data is in " + outputFile);
    }

    static void GenerateRandomData(string filename, int sizeInBytes, double orderProbability = 0.7)
    {
        Random random = new Random();
        using (StreamWriter writer = new StreamWriter(filename))
        {
            int currentNumber = random.Next(0, 1000000);  // Початкове число
            writer.WriteLine(currentNumber);  // Записуємо перше число

            while (new FileInfo(filename).Length < sizeInBytes)
            {
                // Вирішуємо, чи наступне число буде більшим за попереднє, щоб зберегти порядок
                if (random.NextDouble() < orderProbability)
                {
                    // Зберігаємо порядок (наростання)
                    currentNumber += random.Next(1, 100);  // Додаємо випадкове мале число
                }
                else
                {
                    // Порушуємо порядок (перепад до випадкового числа)
                    currentNumber = random.Next(0, 1000000);
                }

                writer.WriteLine(currentNumber);
            }
        }
    }
}
