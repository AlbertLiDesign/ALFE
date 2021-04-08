using ALFE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class TestMethods
    {

        public static void TestTriangles(int x = 7, int y = 5)
        {
            Model model2d = new Cantilever2D(ElementType.TriangleElement, x, y).Model;
            FESystem sys = new FESystem(model2d, false);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);
            FEPrint.PrintDisplacement(sys);
            FEIO.WriteCOOMatrix(sys.GetKG().ToCOO(), "C:/Users/alber/Desktop/matA.mtx");
        }
        public static void TestQuads(int x = 7, int y = 5)
        {
            Model model2d = new Cantilever2D(ElementType.QuadElement, x, y).Model;
            model2d.Elements[0].ComputeKe();
            Console.WriteLine(model2d.Elements[0].B);
        }
        public static void TestPixels(int x = 7, int y = 5)
        {
            Model model2d = new Cantilever2D(ElementType.SquareElement, x, y).Model;
            FESystem sys = new FESystem(model2d, true);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);

            ////FEPrint.PrintDisplacement(sys);
            ////var disp = sys.GetDisplacement();
            ////FEPrint.PrintCSR(KG);
            ////FEIO.WriteCOOMatrix(KG.ToCOO(), "C:/Users/alber/Desktop/matA.mtx");
            Console.WriteLine("------------------- Result Info -------------------");
            Console.WriteLine("Displacement[2].Y = " + sys.GetDisplacement()[2, 1].ToString());
            //FEPrint.PrintDisplacement(sys);
        }
    }
}
