using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace DSShinyEditor
{
    class Editor
    {
        private Dictionary<string, uint> gameAddrMap = new Dictionary<string, uint>()
        {
            ["ADA"] = 0x6CAC2,
            ["APA"] = 0x6CAC2,
            ["CPU"] = 0x79E50,
            ["IPK"] = 0x7007E,
            ["IPG"] = 0x7007E,
            ["IRB"] = 0x13F0b,
            ["IRA"] = 0x13F0b,
            ["IRE"] = 0x18DF0,
            ["IRD"] = 0x18DF0

        };

        private string gameCode;
        public Editor(string path, byte rate)
        {
            if (File.Exists(path))
            {
                Console.WriteLine("ROM found! Checking version...");
                using (BinaryReader br = new BinaryReader(File.OpenRead(path)))
                {
                    br.BaseStream.Seek(0xC, SeekOrigin.Begin); // Get ROM ID
                    gameCode = Encoding.UTF8.GetString(br.ReadBytes(3));
                }
                Console.WriteLine($"Game version is {gameCode}.");
                string workingFolder = Path.Combine(Directory.GetParent(path).ToString(), gameCode+@"\");
                Directory.CreateDirectory(Path.Combine(Directory.GetParent(path).ToString(), gameCode)); // Create folder at the root of the current directory
                if (gameCode == "IPK" || gameCode == "IPG" || gameCode == "IRB" || gameCode == "IRA" || gameCode == "IRE" || gameCode == "IRD" )
                {
                    // Unpack the ROM using ndstool
                    Process unpack = new Process();
                    unpack.StartInfo.FileName = @"Tools\ndstool.exe";
                    unpack.StartInfo.Arguments = "-x " + '"' + path + '"' + " -9 " + '"' + workingFolder + "arm9.bin" + '"' + " -7 " + '"' + workingFolder + "arm7.bin" + '"' + " -y9 " + '"' + workingFolder + "y9.bin" + '"' + " -y7 " + '"' + workingFolder + "y7.bin" + '"' + " -d " + '"' + workingFolder + "data" + '"' + " -y " + '"' + workingFolder + "overlay" + '"' + " -t " + '"' + workingFolder + "banner.bin" + '"' + " -h " + '"' + workingFolder + "header.bin" + '"';
                    Application.DoEvents();
                    unpack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    unpack.StartInfo.CreateNoWindow = true;
                    unpack.Start();
                    unpack.WaitForExit();

                    // Modify the arm9
                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(workingFolder + "arm9.bin")))
                    {
                        bw.BaseStream.Position = TryGetAddress(gameCode);
                        bw.Write(rate);
                    }

                    // Repack the ROM to shinyModified
                    Process repack = new Process();
                    repack.StartInfo.FileName = @"Tools\ndstool.exe";
                    repack.StartInfo.Arguments = "-c " + '"' + workingFolder + "shinyModified.nds" + '"' + " -9 " + '"' + workingFolder + "arm9.bin" + '"' + " -7 " + '"' + workingFolder + "arm7.bin" + '"' + " -y9 " + '"' + workingFolder + "y9.bin" + '"' + " -y7 " + '"' + workingFolder + "y7.bin" + '"' + " -d " + '"' + workingFolder + "data" + '"' + " -y " + '"' + workingFolder + "overlay" + '"' + " -t " + '"' + workingFolder + "banner.bin" + '"' + " -h " + '"' + workingFolder + "header.bin" + '"';
                    Application.DoEvents();
                    repack.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    repack.StartInfo.CreateNoWindow = true;
                    repack.Start();
                    repack.WaitForExit();
                } else
                {
                    // Copy ROM and modify it
                    File.Copy(path, workingFolder+"shinyModified.nds");
                    using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(workingFolder + "shinyModified.nds")))
                    {
                        bw.BaseStream.Position = TryGetAddress(gameCode);
                        bw.Write(rate);
                    }
                }
                Console.WriteLine("Finished modifying shiny rate. File shinyModified.nds can be found in the roms directory, inside the folder GAMECODE.");
                
                
            }
            else
                throw new Exception("File not found. Make sure the path you specified is valid, and try again.");
        }

        public uint TryGetAddress(string gameVers)
        {
            try
            {
                return gameAddrMap[gameVers];
            }
            catch (KeyNotFoundException e)
            {
                throw new Exception("Game not supported. Exiting.\n" + e.StackTrace);
            }
        }
    }
}