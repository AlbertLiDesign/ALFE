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

            //Test1001();
            TestBESO();

            Console.ReadKey();
        }
        public static void TestBESO()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest";
            BESO beso = FEIO.ReadBESO(path);
            beso.Initialize();
            beso.Optimize();
        }
        public static void Test1001()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement,200,200).Model;
            FESystem sys = new FESystem(model2d, true, false);
            sys.Initialize();
            sys.Solve(3);
            //FEIO.WriteKG(sys.GetKG(), @"E:\ALCoding\ALFE\topoptTest\KG.mtx");
            FEPrint.PrintSystemInfo(sys);
            //FEPrint.PrintDisplacement(sys);
        }
        public static void TestTet()
        {
            //string path = "box.vtk";
            //var model = FEIO.ReadVTK(path);
        }
    }
}
