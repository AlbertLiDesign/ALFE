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

            var model = FEIO.ReadFEModel("E:/model.txt");

            Console.ReadKey();
        }
        public static void TestBESO()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest";
            Model model = new Cantilever2D(ElementType.PixelElement, 7, 5).Model;
            FESystem sys = new FESystem(model, true);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);

            BESO beso = new BESO(sys, 1.5f);
            beso.Optimize(path);

            ////FEPrint.PrintDisplacement(sys);
            ////var disp = sys.GetDisplacement();
            ////FEPrint.PrintCSR(KG);
            ////FEIO.WriteCOOMatrix(KG.ToCOO(), "C:/Users/alber/Desktop/matA.mtx");
            Console.WriteLine("------------------- Result Info -------------------");
            Console.WriteLine("Displacement[12].Y = " + sys.GetDisplacement()[12, 1].ToString());

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
