using ALFE.FESystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE
{
    public class FEPrint
    {
        public static void PrintSystemInfo(System2D sys)
        {
            PrintModelInfo(sys);
            PrintMatrixInfo(sys.GetKG());
            PrintTimeCost(sys.TimeCost);
        }
        public static void PrintModelInfo(System2D sys)
        {
            Console.WriteLine("------------------- Model Info -------------------");
            Console.WriteLine("Nodes: " + sys.Model.Nodes.Count.ToString());
            Console.WriteLine("Elements: " + sys.Model.Elements.Count.ToString());
            Console.WriteLine("Type: " + sys.Model.Elements[0].Type.ToString());
        }
        public static void PrintCOO(COOMatrix coo)
        {
            for (int i = 0; i < coo.NNZ; i++)
            {
                Console.WriteLine(coo.RowArray[i].ToString() + '\t'
                        + coo.ColArray[i].ToString() + '\t'
                        + coo.ValueArray[i].ToString());
            }
        }
        public static void PrintMatrixInfo(CSRMatrix csr)
        {
            Console.WriteLine("------------------- Matrix Info -------------------");
            Console.WriteLine("Rows: " + csr.N.ToString());
            Console.WriteLine("Cols: " + csr.N.ToString());
            Console.WriteLine("NNZ: " + csr.NNZ.ToString());
        }
            public static void PrintMatrixInfo(COOMatrix coo)
        {
            Console.WriteLine("------------------- Matrix Info -------------------");
            Console.WriteLine("Rows: " + coo.Rows.ToString());
            Console.WriteLine("Cols: " + coo.Cols.ToString());
            Console.WriteLine("NNZ: " + coo.NNZ.ToString());
        }
        public static void PrintTimeCost(List<double> timeCost)
        {
            Console.WriteLine("------------------- Time Cost -------------------");
            Console.WriteLine("Computing Ke: " + timeCost[0].ToString() + " ms");
            Console.WriteLine("Assembling KG: " + timeCost[1].ToString() + " ms");
            Console.WriteLine("Solving: " + timeCost[2].ToString() + " ms");
        }
        public static void PrintDisplacement(System2D sys)
        {
            foreach (var item in sys.Model.Nodes)
            {
                Console.WriteLine(item.Displacement.X.ToString() + '\t'
                  + item.Displacement.Y.ToString());
            }
        }
    }
}
