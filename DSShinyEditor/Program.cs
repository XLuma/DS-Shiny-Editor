using System;
using System.IO;



namespace DS_Shiny_Editor
{
    class Program
    {
        static void Main(string[] args)
        {
            

            int ptShinyAddr = 0x79E50;
      

            Console.WriteLine("This program allows you to change the shiny rate in DS Pokemon games. As of now, ONLY Platinum is supported. The new rate can only range between 00 to FF (basically 0 to 255, being 0/65535 to roughly 1/257). for example, a rate of 10 gives 1/4096");
            Console.WriteLine("To calculate the percentage, the following can be used: (Rate/65535)*100. The default value is 08 (1/8192)");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine("Input your shiny rate, in decimal (0 to 255)");
            int inputShinyRate = Convert.ToInt16(Console.ReadLine());
            string hexShinyRate = "0x" + inputShinyRate.ToString("X");
            Console.WriteLine("The new shiny rate will be " + hexShinyRate);

            //We check if the rom exists
            string ptRom = Path.Combine(Directory.GetCurrentDirectory() + @"\platinum.nds");

            Console.WriteLine("Checking for rom...");
            if (File.Exists("platinum.nds"))
            {
                Console.WriteLine("Rom found! Changing the value..");

                //Main code to write the new rate

                BinaryWriter bw = new BinaryWriter(File.OpenWrite(ptRom));
                bw.Seek(ptShinyAddr, SeekOrigin.Begin);
                bw.Write((byte)inputShinyRate);
                bw.Dispose();

                Console.WriteLine("Done !");
            }
            else {
                Console.WriteLine("Sorry, the File does not exists, or is not present in the working directory. Please make sure the rom is there, and try again.");
            }
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
            Environment.Exit(1);
  




        }
    }
}
