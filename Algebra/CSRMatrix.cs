using System;
using System.Collections.Generic;
using System.Linq;

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
        public double[] Vals;

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
            Vals = new double[NNZ];
        }

        public void DeleteRowAndCol(int id)
        {
            // Step 1: To compute the nnz of rows we want to remove
            int row_nnz = Rows[id + 1] - Rows[id];

            var new_Vals = Vals.ToList();
            var new_Cols = Cols.ToList();
            var new_Rows = Rows.ToList();
            // Step 2: To remove the item in Vals and Cols
            for (int i = 0; i < row_nnz; i++)
            {
                new_Vals.RemoveAt(Rows[id]);
                new_Cols.RemoveAt(Rows[id]);
            }

            // Step 3: To remove the item in Rows
            new_Rows.RemoveAt(id + 1);

            // Step 4: To change the indices of the items after the removed item
            for (int i = id+1; i < new_Rows.Count; i++)
                new_Rows[i] -= row_nnz;

            // Step 5: To compute the nnz of cols we want to remove
            int col_nnz = 0;

            List<int> col_ids = new List<int>();
            for (int i = 0; i < new_Cols.Count; i++)
            {
                if (new_Cols[i] == id)
                {
                    col_nnz++;
                    col_ids.Add(i);
                }
            }

            // Step 6: To remove the item in Vals and Cols
            for (int i = 0; i < col_nnz; i++)
            {
                new_Vals.RemoveAt(col_ids[i] -i);
                new_Cols.RemoveAt(col_ids[i] -i);
            }

            // Step 7: To change the column-index of the items after the removed item
            for (int i = 0; i < new_Cols.Count; i++)
                if (new_Cols[i] >= id)
                    new_Cols[i] -= 1;
            
            // Step 8: To change the row-index of the items after the removed item
            for (int i = 0; i < new_Rows.Count; i++)
            {
                int num = 0;
                for (int j = 0; j < col_ids.Count; j++)
                    if (new_Rows[i] > col_ids[j])
                        num++;

                new_Rows[i] -= num;
            }

            // Step 8: To update the CSR matrix
            Rows = new_Rows.ToArray();
            Cols = new_Cols.ToArray();
            Vals = new_Vals.ToArray();
            NNZ = Rows.Last();
            N -= 1;
        }
        public COOMatrix ToCOO(bool symmetry = false)
        {
            List<Triplet> triplets = new List<Triplet>();

            int id = 0;
            for (int i = 0; i < N; i++)
            {
                int dif = Rows[i + 1] - Rows[i];
                for (int j = 0; j < dif; j++)
                {

                    if (symmetry)
                    {
                        if (Cols[id] >= i)
                            triplets.Add(new Triplet(i, Cols[id], Vals[id]));
                    }
                    else
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