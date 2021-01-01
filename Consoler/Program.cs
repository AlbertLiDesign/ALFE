using ALFE.FEModel;
using ALFE.FESystem;
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
            TestQuads();
            Console.ReadKey();
        }
        public static void TestTriangles(int x = 7, int y = 5)
        {
            Model2D model2d = new Cantilever2D(ElementType.TriangleElement, x, y).Model;
            System2D sys = new System2D(model2d, false);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);
            FEPrint.PrintDisplacement(sys);
            FEIO.WriteCOOMatrix(sys.GetKG().ToCOO(), "C:/Users/alber/Desktop/matA.mtx");
        }
        public static void TestQuads(int x = 7, int y = 5)
        {
            Model2D model2d = new Cantilever2D(ElementType.QuadElement, x, y).Model;
            model2d.Elements[0].ComputeKe();
            FEPrint.PrintMatrix(model2d.Elements[0].Ke);
        }
        public static void TestPixels(int x = 7, int y = 5)
        {
            Model2D model2d = new Cantilever2D(ElementType.PixelElement, x, y).Model;
            System2D sys = new System2D(model2d, true);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);

            ////FEPrint.PrintDisplacement(sys);
            ////var disp = sys.GetDisplacement();
            ////FEPrint.PrintCSR(KG);
            ////FEIO.WriteCOOMatrix(KG.ToCOO(), "C:/Users/alber/Desktop/matA.mtx");

            ////Console.WriteLine(sys.GetDisplacement()[5000, 1]);
            FEPrint.PrintDisplacement(sys);
        }
    }
}
