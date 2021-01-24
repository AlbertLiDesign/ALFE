using ALFE;
using ALFE;
using ALFE.TopOpt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start to test, please wait a few seconds...");

            TestBESO();

            Console.ReadKey();
        }
        public static void TestBESO()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest";
            BESO beso = FEIO.ReadBESO(@"E:\beso.al");
            beso.Optimize(path);
        }
        public static void Test1001()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement, 1001, 1001).Model;
            FESystem sys = new FESystem(model2d, true);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);
        }
        public static void TestTet()
        {
            //string path = "box.vtk";
            //var model = FEIO.ReadVTK(path);
        }
    }
}
