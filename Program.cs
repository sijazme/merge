using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
