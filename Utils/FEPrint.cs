using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class FEPrint
    {
        public static void PrintMatrix(double[] mat, int m, int n)
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
        }
        public static void PrintMatrix(double[,] mat)
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
