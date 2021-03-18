using ALFE.TopOpt;
using System;

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
        public static void Test()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement, 100, 100).Model;

            FESystem sys0 = new FESystem(model2d, true, false, Solver.PARDISO_Single);
            sys0.Initialize();
            sys0.Solve();

            //FEIO.WriteKG(sys0.GetKG(), "E:/test/test.mtx");

            Console.Write(sys0.Model.ModelInfo());
            Console.Write(sys0.MatrixInfo());

            Console.Write(sys0.SolvingInfo());
            Console.WriteLine();
        }
        public static void TestAllSolver()
        { 
            Model model2d = new Cantilever2D(ElementType.PixelElement, 100, 100).Model;

            FESystem sys0 = new FESystem(model2d, true, false, Solver.SimplicialLLT);
            sys0.Initialize();
            sys0.Solve();

            Console.Write(sys0.Model.ModelInfo());
            Console.Write(sys0.MatrixInfo());


            Console.WriteLine("------------------- Time Cost -------------------");
            Console.WriteLine("Solver: " + sys0._Solver.ToString());
            Console.WriteLine("Solving: " + sys0.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            FESystem sys1 = new FESystem(model2d, true, false, Solver.PARDISO);
            sys1.Initialize();
            sys1.Solve();

            Console.WriteLine("Solver: " + sys1._Solver.ToString());
            Console.WriteLine("Solving: " + sys1.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            FESystem sys2 = new FESystem(model2d, true, false, Solver.PARDISO_Single);
            sys2.Initialize();
            sys2.Solve();

            Console.WriteLine("Solver: " + sys2._Solver.ToString());
            Console.WriteLine("Solving: " + sys2.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            //FESystem sys1 = new FESystem(model2d, true, false, Solver.CholmodSimplicialLLT);
            //sys1.Initialize();
            //sys1.Solve();

            //Console.WriteLine("Solver: " + sys1._Solver.ToString());
            //Console.WriteLine("Solving: " + sys1.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();

            //FESystem sys2 = new FESystem(model2d, true, false, Solver.CholmodSuperNodalLLT);
            //sys2.Initialize();
            //sys2.Solve();

            //Console.WriteLine("Solver: " + sys2._Solver.ToString());
            //Console.WriteLine("Solving: " + sys2.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();

            //FESystem sys3 = new FESystem(model2d, true, false, Solver.PARDISO);
            //sys3.Initialize();
            //sys3.Solve();

            //Console.WriteLine("Solver: " + sys3._Solver.ToString());
            //Console.WriteLine("Solving: " + sys3.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();

            //FESystem sys4 = new FESystem(model2d, true, false, Solver.CG);
            //sys4.Initialize();
            //sys4.Solve();

            //Console.WriteLine("Solver: " + sys4._Solver.ToString());
            //Console.WriteLine("Solving: " + sys4.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();

            //FESystem sys5 = new FESystem(model2d, true, false, Solver.AMG_CG);
            //sys5.Initialize();
            //sys5.Solve();

            //Console.WriteLine("Solver: " + sys5._Solver.ToString());
            //Console.WriteLine("Solving: " + sys5.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();


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
