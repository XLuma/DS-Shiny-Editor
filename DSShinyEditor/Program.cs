using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DS_Shiny_Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            int ptShinyAddr = 0x79E50;

            Console.WriteLine("This program allows you to change the shiny rate in DS Pokemon games. As of now, ONLY Platinum is supported.");
            Console.WriteLine("You can use the default settings by using no arguments, or use it as such:");
            Console.WriteLine("     DSShinyEditor.exe ROMNAME.nds SHINYRATE");
            Console.WriteLine("To use SHINYRATE via cli ROMNAME must be supplied.");
            Console.WriteLine("The new rate can only range between 00 to FF (basically 0 to 255, being 0/65535 to roughly 1/257). for example, a rate of 10 gives 1/4096");
            Console.WriteLine("To calculate the percentage, the following can be used: (Rate/65535)*100. The default value is 08 (1/8192)");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            int shinyRate = 8;
            if (args.Length == 2)
            {
                shinyRate = Convert.ToInt16(args[1]);
                Console.WriteLine("Shiny rate of " + shinyRate.ToString() + " out of 255 selected.");

            }
            else
            {
                Console.WriteLine("No shiny rate selected, please choose one.");
                Console.WriteLine("Input your shiny rate, in decimal (0 to 255)");
                shinyRate = Convert.ToInt16(Console.ReadLine());
            }
            string hexShinyRate = "0x" + shinyRate.ToString("X");
            Console.WriteLine("The new shiny rate will be " + hexShinyRate);

            //We check if the rom exists
            string ptRom = "";
            string romName = "";
            if (args.Length == 0)
            {
                romName = "platinum.nds";
                Console.WriteLine("No rom selected, searching for default " + romName);
            }
            else
            {
                romName = args[0].ToString();
                Console.WriteLine("Searching for rom " + romName);
            }
            ptRom = Path.Combine(Directory.GetCurrentDirectory() + @"\" + romName);
            Console.WriteLine("Checking for rom...");
            if (File.Exists(romName))
            {
                string gameCode = "";
                Console.WriteLine("Rom found! Checking version..");
                using (BinaryReader br = new BinaryReader(File.OpenRead(romName)))
                {
                    br.BaseStream.Seek(0xC, SeekOrigin.Begin); // Get ROM ID
                    gameCode = Encoding.UTF8.GetString(br.ReadBytes(4));
                }
                string gameVersion = GetVersion(gameCode);
                if (gameVersion == "Platinum")
                {
                    //Main code to write the new rate
                    Console.WriteLine("Rom is valid. Changing the value..");
                    BinaryWriter bw = new BinaryWriter(File.OpenWrite(ptRom));
                    bw.Seek(ptShinyAddr, SeekOrigin.Begin);
                    bw.Write((byte)shinyRate);
                    bw.Dispose();
                }
                else
                {
                    if (gameVersion == "Not Found")
                    {
                        Console.WriteLine("Sorry, the input ROM is not supported.\nPress any key to close");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                    else
                    {
                        Console.WriteLine("Sorry, " + gameVersion + " support is not implemented.\nPress any key to close");
                        Console.ReadKey();
                        Environment.Exit(0);
                    }
                }
                Console.WriteLine("Done !");
            }
            else
            {
                Console.WriteLine("Sorry, the File does not exists, or is not present in the working directory. Please make sure the rom is there, and try again.");
            }
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static string GetVersion(string gameVers)
        {
            Dictionary<string, string> versions = new Dictionary<string, string>()
            {
                ["ADAE"] = "Diamond",
                ["ADAS"] = "Diamond",
                ["ADAI"] = "Diamond",
                ["ADAF"] = "Diamond",
                ["ADAD"] = "Diamond",
                ["ADAJ"] = "Diamond",
                ["APAE"] = "Pearl",
                ["APAS"] = "Pearl",
                ["APAI"] = "Pearl",
                ["APAF"] = "Pearl",
                ["APAD"] = "Pearl",
                ["APAJ"] = "Pearl",
                ["CPUE"] = "Platinum",
                ["CPUS"] = "Platinum",
                ["CPUI"] = "Platinum",
                ["CPUF"] = "Platinum",
                ["CPUD"] = "Platinum",
                ["CPUJ"] = "Platinum",
                ["IPKE"] = "HeartGold",
                ["IPKS"] = "HeartGold",
                ["IPKI"] = "HeartGold",
                ["IPKF"] = "HeartGold",
                ["IPKD"] = "HeartGold",
                ["IPKJ"] = "HeartGold",
                ["IPGE"] = "SoulSilver",
                ["IPGS"] = "SoulSilver",
                ["IPGI"] = "SoulSilver",
                ["IPGF"] = "SoulSilver",
                ["IPGD"] = "SoulSilver",
                ["IPGJ"] = "SoulSilver",
            };
            if (versions.ContainsKey(gameVers))
                return versions[gameVers];
            else
                return "Not Found";
        }
    }
}
