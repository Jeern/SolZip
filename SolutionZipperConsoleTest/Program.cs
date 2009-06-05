using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SolutionZipper;
using System.IO;

namespace SolutionZipperConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Test3();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.ToString());
            }
            finally
            {
                Console.WriteLine("Press Any Key");
                Console.ReadLine();
            }
        }

        private static void Test1()
        {
            using (var controller = new SolZipController(@"C:\Funky.zip"))
            {
                controller.ZipSolution(@"C:\Projects\KbhKommune.Ask.backup\KbhKommune.Ask.sln");
            }
            
        }

        private static void Test2()
        {
            Console.WriteLine(File.Exists(Path.Combine(@"C:\Projects\KbhKommune.Ask.backup\CrmDataGateway", @"..\microsoft.crm.sdktypeproxy.dll")));
        }

        private static void Test3()
        {
            using (var controller = new SolZipController(@"C:\Funky.zip"))
            {
                controller.ZipSolution(@"C:\Projects\CReMeSoFa\CReMeSoFa.sln");
            }

        }

    }
}
