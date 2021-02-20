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

            TestAllSolver();

            //TestBESO();

            Console.ReadKey();
        }
        public static void TestBESO()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest";
            BESO beso = FEIO.ReadBESO(path);
            beso.Initialize();
            beso.Optimize();
        }
        public static void TestSXAMG()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement, 110, 80).Model;

            FESystem sys0 = new FESystem(model2d, true, false, Solver.SXAMG);
            sys0.Initialize();
            sys0.Solve();

            Console.Write(sys0.Model.ModelInfo());
            Console.Write(sys0.MatrixInfo());

            Console.Write(sys0.SolvingInfo());
            Console.WriteLine();
        }
        public static void TestAllSolver()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement,110,80).Model;

            FESystem sys0 = new FESystem(model2d, true, false, Solver.SimplicialLLT);
            sys0.Initialize();
            sys0.Solve();

            Console.Write(sys0.Model.ModelInfo());
            Console.Write(sys0.MatrixInfo());

            Console.Write(sys0.SolvingInfo());
            Console.WriteLine();

            FESystem sys1 = new FESystem(model2d, true, false, Solver.CholmodSimplicialLLT);
            sys1.Initialize();
            sys1.Solve();

            Console.Write(sys1.SolvingInfo());
            Console.WriteLine();

            FESystem sys2 = new FESystem(model2d, true, false, Solver.CholmodSuperNodalLLT);
            sys2.Initialize();
            sys2.Solve();

            Console.Write(sys2.SolvingInfo());
            Console.WriteLine();

            FESystem sys3 = new FESystem(model2d, true, false, Solver.PARDISO);
            sys3.Initialize();
            sys3.Solve();

            Console.Write(sys3.SolvingInfo());
            Console.WriteLine();

            FESystem sys4 = new FESystem(model2d, true, false, Solver.SXAMG);
            sys4.Initialize();
            sys4.Solve();

            Console.Write(sys4.SolvingInfo());
            Console.WriteLine();

            //FEIO.WriteKG(sys.GetKG(), @"E:\ALCoding\ALFE\topoptTest\KG.mtx");
            //FEPrint.PrintDisplacement(sys);
        }
        public static void TestTet()
        {
            //string path = "box.vtk";
            //var model = FEIO.ReadVTK(path);
        }
    }
}
