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
        public int[] RowArray;
        public int[] ColArray;
        public double[] ValueArray;
        public int Rows, Cols, NNZ; // Row number, column number and non-zero number

        public CooMatrix(List<Triplet> triplets, int rows, int cols)
        {
            foreach (var item in triplets)
            {
                if (item.Value != 0)
                {
                    Triplets.Add(item);
                }
            }
            Rows = rows;
            Cols = cols;
            NNZ = Triplets.Count;
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
                    if (vals[i] !=0)
                    {
                        Triplets.Add(new Triplet(m[i], n[i], vals[i]));
                    }
                }
            }
            else
            {
                throw new Exception("Fail to create a coo matrix.");
            }
        }

        public void CompressMatrix()
        {
            double[,] mat = new double[Rows,Cols];
            for (int i = 0; i < Triplets.Count; i++)
            {
                mat[Triplets[i].Row, Triplets[i].Col] += Triplets[i].Value;
            }

        }
    }
}