using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSShinyEditor
{
    class Editor
    {
        private Dictionary<string, uint> gameAddrMap = new Dictionary<string, uint>()
        {
            ["ADA"] = 0x0,
            ["APA"] = 0x0,
            ["CPU"] = 0x79E50,
            ["IPK"] = 0x0,
            ["IPG"] = 0x0
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
                Directory.CreateDirectory(Path.Combine(Directory.GetParent(path).ToString(), gameCode)); // Create folder at the root of the current directory

                /*
                 * Extraction magic goes here. You should extract the ROM, decompress the ARM9, then patch the byte.
                 */

                // While the below works, do NOT use it in its current state. I commented it out for a reason. This will just jump to the offset, and write the byte there. Do the approach I mentioned in the earlier comment.
                /*
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(path)))
                {
                    bw.BaseStream.Position = TryGetAddress(gameCode);
                    bw.Write(rate);
                }
                */
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
