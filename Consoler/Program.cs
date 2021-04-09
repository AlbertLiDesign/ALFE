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

            // TestAllSolver();

            TestHexahedron();

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
            Model model2d = new Cantilever2D(ElementType.SquareElement, 100, 100).Model;

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
            Model model2d = new Cantilever2D(ElementType.SquareElement, 100, 200).Model;

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

            FESystem sys3 = new FESystem(model2d, true, false, Solver.CholmodSimplicialLLT);
            sys3.Initialize();
            sys3.Solve();

            Console.WriteLine("Solver: " + sys3._Solver.ToString());
            Console.WriteLine("Solving: " + sys3.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            FESystem sys4 = new FESystem(model2d, true, false, Solver.CholmodSuperNodalLLT);
            sys4.Initialize();
            sys4.Solve();

            Console.WriteLine("Solver: " + sys4._Solver.ToString());
            Console.WriteLine("Solving: " + sys4.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            FESystem sys5 = new FESystem(model2d, true, false, Solver.CG);
            sys5.Initialize();
            sys5.Solve();

            Console.WriteLine("Solver: " + sys5._Solver.ToString());
            Console.WriteLine("Solving: " + sys5.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

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
        public static void TestQuads()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(6.0,2.0),
                new Node(21.0,9.0),
                new Node(27.0,-9.0),
                new Node(10.0,-10.0),
            };

            var element = new Quadrilateral(nodes, new Material(10, 0.3));
            element.ComputeKe();

            Console.WriteLine(element.J);
            Console.WriteLine(element.D);
            Console.WriteLine(element.B);

            Console.WriteLine(element.Ke);
        }

        public static void TestHexahedron()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(0.0,0.0,0.0),
                new Node(7.0,13.0,0.0),
                new Node(8.0, 22.0, 7.0),
                new Node(3.0, 5.0, 10.0),

                new Node(11.0,0.0,0.0),
                new Node(16.0,13.0,0.0),
                new Node(17.0, 16.0, 8.0),
                new Node(12.0, 5.0, 14.0)
            };

            var element = new Hexahedron(nodes, new Material(10, 0.3));
            element.ComputeKe();

            Console.WriteLine(element.J.Determinant());
            Console.WriteLine(element.D);
            Console.WriteLine(element.B);

            Console.WriteLine(element.Ke);
        }
    }
}
