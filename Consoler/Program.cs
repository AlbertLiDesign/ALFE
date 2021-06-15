using ALFE.TopOpt;
using System;
using System.Collections.Generic;

namespace ALFE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start to test, please wait a few seconds...");

            RunVerify3D();

            Console.ReadKey();
        }
        public static void RunVerify2D()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest\Example2\Verify";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize();
            FEIO.WriteIsovalues(path, beso);
        }
        public static void RunVerify3D()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest\Example3\Verify2";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize(true);
            FEIO.WriteIsovalues(path, beso);
        }
        public static void RunExample3D()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest\Example3";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize();
            FEIO.WriteIsovalues(path, beso);
        }

    }
}
