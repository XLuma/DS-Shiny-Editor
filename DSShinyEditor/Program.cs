using DSShinyEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace DS_Shiny_Editor
{
    class Program
    {
        static string usage = @"This program allows you to change the shiny rate in DS Pokemon games. As of now, ONLY Platinum is supported.
You can use the default settings by using no arguments, or use it as such:
    DSShinyEditor.exe ROMNAME.nds SHINYRATE
To use SHINYRATE via cli ROMNAME must be supplied.
The new rate can only range between 00 to FF (basically 0 to 255, being 0/65535 to roughly 1/257). for example, a rate of 10 gives 1/4096
To calculate the percentage, the following can be used: (Rate/65535)*100. The default value is 08 (1/8192)";

        static void Main(string[] args)
        {
            string path = null;
            byte rate = 0;

            if (args.Length is 0)
            {
                path = PathPrompt();
                rate = RatePrompt();

                new Editor(path, rate);
            }
            else
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-r":
                            if (i + 1 >= args.Length)
                                throw new IndexOutOfRangeException("Out of bounds.");
                            if (!byte.TryParse(args[++i], out rate))
                                throw new Exception("Invalid rate.");
                            break;
                        case "-p":
                            if (i + 1 >= args.Length)
                                throw new IndexOutOfRangeException("Out of bounds.");
                            path = args[++i];
                            break;
                        case "-h":
                            Console.WriteLine(usage);
                            i = args.Length;
                            break;
                    }
                    if (path == null)
                        path = PathPrompt();
                    if (rate == 0)
                        rate = RatePrompt();

                    new Editor(path, rate);
                }
            }
        }

        static String PathPrompt()
        {
            // Default ROM is "platinum.nds"
            Console.Write("Enter the path of your Pokémon ROM (including the extension), and press enter. ");
            return Console.ReadLine();
        }

        static byte RatePrompt()
        {
            byte rate;
            Console.Write("Shiny Rate? ");

            while (!byte.TryParse(Console.ReadLine(), out rate) && rate < 0x0 && rate > 0xFF)
                Console.Write("Invalid value. Let's try again: Shiny Rate? ");

            return rate;
        }
    }
}
