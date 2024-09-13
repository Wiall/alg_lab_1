using System;
using System.IO;

class BinaryTextConverter
{
    // Метод для перетворення бінарних записів у текстовий формат
    public static void ConvertBinaryToText(string inputFile, string outputFile)
    {
        using (BinaryReader reader = new BinaryReader(File.Open(inputFile, FileMode.Open)))
        using (StreamWriter writer = new StreamWriter(outputFile))
        {
            // Читаємо бінарні дані як int (4 байти за раз)
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                try
                {
                    int number = reader.ReadInt32(); // Читаємо ціле число з бінарного файлу
                    writer.WriteLine(number); // Записуємо число у текстовий файл
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при читанні з бінарного файлу: {ex.Message}");
                }
            }
        }
    }
}