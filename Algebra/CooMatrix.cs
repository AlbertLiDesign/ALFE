using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class CooMatrix
    {
        public List<Triplet> Triplets = new List<Triplet>();
        public int Rows, Cols, NNZ; // Row number, column number and non-zero number

        public CooMatrix(List<Triplet> triplets, int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            NNZ = triplets.Count;
            Triplets = triplets;
        }
        public CooMatrix(int[] m, int[] n, double[] vals, int rows, int cols)
        {
            if (m.Length == n.Length && n.Length == vals.Length)
            {
                Rows = rows;
                Cols = cols;
                NNZ = vals.Length;
                for (int i = 0; i < NNZ; i++)
                {
                    Triplets.Add(new Triplet(m[i], n[i], vals[i]));
                }
            }
            else
            {
                throw new Exception("Fail to create a coo matrix.");
            }
        }
    }
}