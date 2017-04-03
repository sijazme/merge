using System;

namespace merge
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Scanning for .dat files...");
            FileManager f = new FileManager(Environment.CurrentDirectory);
            f.Save();
            Console.ReadLine();
        }
    }
}
