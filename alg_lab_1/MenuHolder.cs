using System;

namespace alg_lab_1
{
    public class MenuHolder
    {
        public static int index = 0;

        public static int HoldMenu(string[] options)
        {
            Console.Clear();
            ConsoleKey key;
            
            do
            {
                Console.Clear();
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == index)
                    {
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }
                
                key = Console.ReadKey(true).Key;
                
                if (key == ConsoleKey.UpArrow)
                {
                    index--;
                    if (index < 0) index = options.Length - 1;
                }
                else if (key == ConsoleKey.DownArrow)
                {
                    index++;
                    if (index >= options.Length) index = 0;
                }

            } while (key != ConsoleKey.Enter);

            Console.Clear();
            return index;
        }
    }
}
