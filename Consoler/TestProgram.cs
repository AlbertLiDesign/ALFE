using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALFE;
using ALFE.TopOpt;

namespace Consoler
{
    class TestProgram
    {
        public static void TestCSR()
        {
            CSRMatrix csr = new CSRMatrix(6, 16);
            csr.Vals = new double[16] { 10, 20, 20, 30, 35, 40, 35, 50, 60, 40, 70, 80, 90, 60, 90, 100 };
            csr.Cols = new int[16] { 0, 1, 0, 1, 2, 3, 1, 2, 5, 1, 3, 4, 5, 2, 4, 5 };
            csr.Rows = new int[7] { 0, 2, 6, 9, 11, 13, 16 };

            csr.DeleteRowAndCol(2);
            FEIO.WriteKG(csr, "E:\\testCSR2.mtx", true);

        }
        public static void TestRollerSupport()
        {
            List<Node> nds = new List<Node>();
            List<Element> elems = new List<Element>();

            nds.Add(new Node(3, 1));
            nds.Add(new Node(3, 0));
            nds.Add(new Node(2, 1));
            nds.Add(new Node(2, 0));
            nds.Add(new Node(0, 1));
            nds.Add(new Node(1, 1));
            nds.Add(new Node(1, 0));
            nds.Add(new Node(0, 0));
            nds.Add(new Node(3, 2));
            nds.Add(new Node(2, 2));
            nds.Add(new Node(0, 2));
            nds.Add(new Node(1, 2));

            elems.Add(new Quadrilateral(new List<Node>(4) { nds[2], nds[0], nds[1], nds[3] }, new Material(1.0, 0.3)));
            elems.Add(new Quadrilateral(new List<Node>(4) { nds[5], nds[2], nds[3], nds[6] }, new Material(1.0, 0.3)));
            elems.Add(new Quadrilateral(new List<Node>(4) { nds[4], nds[5], nds[6], nds[7] }, new Material(1.0, 0.3)));
            elems.Add(new Quadrilateral(new List<Node>(4) { nds[9], nds[8], nds[0], nds[2] }, new Material(1.0, 0.3)));
            elems.Add(new Quadrilateral(new List<Node>(4) { nds[11], nds[9], nds[2], nds[5] }, new Material(1.0, 0.3)));
            elems.Add(new Quadrilateral(new List<Node>(4) { nds[10], nds[11], nds[5], nds[4] }, new Material(1.0, 0.3)));

            Model model = new Model(2, nds, elems,
                new List<Load>(1) { new Load(11, new Vector2D(0.0, -1.0)) },
                new List<Support>(2) { new Support(7, SupportType.Fixed), new Support(1, SupportType.Fixed) });
            FESystem sys = new FESystem(model, Solver.SimplicialLLT, false, false);
            Console.Write(sys.Model.ModelInfo());
            sys.Initialize();
            Console.Write(sys.MatrixInfo());

            var KG = sys.GetKG();
            sys.Solve();
            //FEIO.WriteKG(KG, "E:\\testKG.mtx", false);
            Console.Write(sys.SolvingInfo());
            Console.Write(sys.DisplacementInfo());
        }
        public static void Test()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement, 200, 200).Model;

            FESystem sys0 = new FESystem(model2d, Solver.SimplicialLLT);
            sys0.Initialize();
            sys0.Solve();

            //FEIO.WriteKG(sys0.GetKG(), "E:/test/test.mtx");

            Console.Write(sys0.Model.ModelInfo());
            Console.Write(sys0.MatrixInfo());

            Console.Write(sys0.SolvingInfo());
            //Console.Write(sys0.DisplacementInfo());
            //FEIO.WriteKG(sys0.GetKG(), "E:\\ALCoding\\ALFE\\topoptTest");
            Console.WriteLine();
        }
        public static void TestAllSolver()
        {
            Model model2d = new Cantilever2D(ElementType.PixelElement, 500, 500).Model;

            //FESystem sys0 = new FESystem(model2d, Solver.PARDISO);
            //sys0.Initialize();
            //FEIO.WriteKG(sys0.GetKG(), "E:\\KG" + ".mtx");
            //sys0.Solve();

            //Console.Write(sys0.Model.ModelInfo());
            //Console.Write(sys0.MatrixInfo());

            //Console.WriteLine("------------------- Time Cost -------------------");
            //Console.WriteLine("Solver: " + sys0._Solver.ToString());
            //Console.WriteLine("Solving: " + sys0.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();

            FESystem sys1 = new FESystem(model2d, Solver.SimplicialLLT);
            sys1.Initialize();
            sys1.Solve();

            Console.WriteLine("Solver: " + sys1._Solver.ToString());
            Console.WriteLine("Solving: " + sys1.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            FESystem sys2 = new FESystem(model2d, Solver.PARDISO);
            sys2.Initialize();
            sys2.Solve();


            Console.WriteLine("Solver: " + sys2._Solver.ToString());
            Console.WriteLine("Solving: " + sys2.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            FESystem sys3 = new FESystem(model2d, Solver.CG);
            sys3.Initialize();
            sys3.Solve();

            Console.WriteLine("Solver: " + sys3._Solver.ToString());
            Console.WriteLine("Solving: " + sys3.TimeCost[3].ToString() + " ms");
            Console.WriteLine();

            //FESystem sys4 = new FESystem(model2d, Solver
            //sys4.Initialize();
            //sys4.Solve();

            //Console.WriteLine("Solver: " + sys4._Solver.ToString());
            //Console.WriteLine("Solving: " + sys4.TimeCost[3].ToString() + " ms");
            //Console.WriteLine();

            //FESystem sys5 = new FESystem(model2d, false, Solver.CG);
            //sys5.Initialize();
            //sys5.Solve();

            //Console.WriteLine("Solver: " + sys5._Solver.ToString());
            //Console.WriteLine("Solving: " + sys5.TimeCost[3].ToString() + " ms");
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
        public static void TestQuads()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(0.0,4.0),
                new Node(10.0,7.0),
                new Node(10.0,0.0),
                new Node(5.0,-3.0),
            };

            var element = new Quadrilateral(nodes, new Material(10, 0.3));
            element.ComputeKe();

            Console.WriteLine(element.Ke);

            List<Node> nodes2 = new List<Node>()
            {
                new Node(-17.916667,9.166667),
                new Node(22.083333,21.166667),
                new Node(22.083333,-6.833333),
                new Node(2.083333,-18.833333),
            };

            var element2 = new Quadrilateral(nodes2, new Material(10, 0.3));
            element2.ComputeKe();

            Console.WriteLine(element2.Ke);
        }

        public static void TestHexahedron()
        {
            List<Node> nodes = new List<Node>()
            {
                new Node(0.0,0.0,0.0),
                new Node(0.25,0.0,0.0),
                new Node(0.25,0.25,0.0),
                new Node(0.0,0.25,0.0),

                new Node(0.0,0.0,0.25),
                new Node(0.25,0.0,0.25),
                new Node(0.25,0.25,0.25),
                new Node(0.0,0.25,0.25),


                new Node(0.5,0.0,0.0),
                new Node(0.5,0.0,0.25),
                new Node(0.5,0.25,0.25),
                new Node(0.5,0.25,0.0)
            };

            var element0 = new Hexahedron(new List<Node>(8) { nodes[0], nodes[1], nodes[2], nodes[3], nodes[4], nodes[5], nodes[6], nodes[7] }, new Material(1, 0.3));
            //var element1 = new Hexahedron(new List<Node>(8) { nodes[4], nodes[5], nodes[6], nodes[7], nodes[8], nodes[9], nodes[10], nodes[11] }, new Material(1, 0.3));

            var elements = new List<Element>(1) { element0 };

            //var loads = new List<Load>(4)
            //{
            //    new Load(8, 4.6875, 0.0,0.0),
            //    new Load(9, 4.6875, 0.0,0.0),
            //    new Load(10,4.6875, 0.0,0.0),
            //    new Load(11, 4.6875, 0.0,0.0)
            //};

            //var supports = new List<Support>(4)
            //{
            //    new Support(0, SupportType.Fixed),
            //    new Support(1, SupportType.Fixed),
            //    new Support(2, SupportType.Fixed),
            //    new Support(3, SupportType.Fixed)
            //};

            var model = new Model(3, nodes, elements);

            FESystem sys = new FESystem(model, Solver.SimplicialLLT);
            sys.Initialize();
            Console.WriteLine(model.Elements[0].Ke);
            Console.WriteLine(model.Elements[0].B);
            Console.WriteLine(model.Elements[0].D);
            //sys.Solve();
            Console.Write(sys.Model.ModelInfo());
            Console.Write(sys.MatrixInfo());
            Console.WriteLine("Solver: " + sys._Solver.ToString());
            Console.WriteLine("Solving: " + sys.TimeCost[3].ToString() + " ms");
            Console.WriteLine(sys.DisplacementInfo());
            FEIO.WriteKG(sys.GetKG(), "E:\\ALCoding\\ALFE\\topoptTest", false);
            Console.WriteLine();
        }
    }
}
