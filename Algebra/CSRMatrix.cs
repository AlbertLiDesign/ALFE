using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class CSRMatrix
    {
        /// <summary>
        /// Rows contains indices of the first non-zero elements in each row. The size of the rows array is N+1, and Rows[N] = NNZ.
        /// </summary>
        public int[] Rows;  // structure of the sparse matrix

        /// <summary>
        /// The Cols array holds column indices of the elements, hence its size is also equal to NNZ.
        /// </summary>
        public int[] Cols;

        /// <summary>
        /// The Vals array holds only the non-zero elements listed left-to-right, top-to-bottom, and its size is NNZ.
        /// </summary>
        public float[] Vals;

        /// <summary>
        ///  The size of CSR matrix.
        /// </summary>
        public int N;

        /// <summary>
        /// The number of non-zero entries
        /// </summary>
        public int NNZ;

        public CSRMatrix(int n, int nnz)
        {
            N = n; 
            NNZ = nnz;
            Rows = new int[n + 1];
            Rows[n] = nnz;
            Cols = new int[NNZ];
            Cols[n] = nnz;
            Vals = new float[NNZ];
        }

        public COOMatrix ToCOO()
        {
            List<Triplet> triplets = new List<Triplet>();

            int id = 0;
            for (int i = 0; i < N; i++)
            {
                int dif = Rows[i + 1] - Rows[i];
                for (int j = 0; j < dif; j++)
                {
                    triplets.Add(new Triplet(i, Cols[id], Vals[id]));
                    id++;
                }
            }

            return new COOMatrix(triplets, N, N);
        }
        public void Clear()
        {
            Array.Clear(Vals, 0, Vals.Length);
        }
    }
}