using ALFE.FESystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class FEPrint
    {
        public static void PrintTimeCost(List<double> timeCost)
        {
            Console.WriteLine("---------------------------------- Time Cost ----------------------------------");
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
        public static void PrintMatrix(float[] mat, int m, int n)
        {
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(mat[i*n + j]);
                    Console.Write('\t');
                }
                Console.Write('\n');
            }
            Console.WriteLine("Length: " + m.ToString());
        }
        public static void PrintMatrix(float[,] mat)
        {
            int m = mat.GetLength(0);
            int n = mat.GetLength(1);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Console.Write(mat[i, j]);
                    Console.Write('\t');
                }
                Console.Write('\n');
            }
        }
    }
}
