using ALFE.TopOpt;
using System;
using System.Collections.Generic;

namespace ALFE
{
    public class TestMethods
    {

        public static void TestTriangles(int x = 7, int y = 5)
        {
            Model model2d = new Cantilever2D(ElementType.TriangleElement, x, y).Model;
            FESystem sys = new FESystem(model2d);
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
            Model model2d = new Cantilever2D(ElementType.SquareElement, x+1, y+1).Model;
            var _Filter = new ALFE.TopOpt.Filter(model2d.Elements, 2.0, 2);
            _Filter.PreFlt();

            FESystem sys = new FESystem(model2d);
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
