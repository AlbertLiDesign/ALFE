using System;
using System.Collections.Generic;
using System.Linq;

namespace ALFE
{
    public class COOMatrix
    {
        public int Rows, Cols, NNZ; // Row number, column number and non-zero number
        public int[] RowArray, ColArray;
        public double[] ValueArray;

        public COOMatrix(List<Triplet> triplets, int rows, int cols)
        {
            List<int> rowArray = new List<int>();
            List<int> colArray = new List<int>();
            List<double> valArray = new List<double>();

            for (int i = 0; i < triplets.Count; i++)
            {
                if (triplets[i].Value != 0)
                {
                    rowArray.Add(triplets[i].Row);
                    colArray.Add(triplets[i].Col);
                    valArray.Add(triplets[i].Value);
                }
            }

            RowArray = rowArray.ToArray();
            ColArray = colArray.ToArray();
            ValueArray = valArray.ToArray();
            Rows = rows;
            Cols = cols;
            NNZ = ValueArray.Length;
        }
        public COOMatrix(int[] rowArray, int[] colArray, double[] valArray, int rows, int cols)
        {
            if (rowArray.Length == colArray.Length && colArray.Length == valArray.Length)
            {
                for (int i = 0; i < valArray.Length; i++)
                {
                    if (valArray[i] == 0)
                    {
                        rowArray.ToList().RemoveAt(i);
                        colArray.ToList().RemoveAt(i);
                        valArray.ToList().RemoveAt(i);
                    }
                }

                RowArray = rowArray;
                ColArray = colArray;
                ValueArray = valArray;

                NNZ = ValueArray.Length;
            }
            else
                throw new Exception("Fail to create a coo matrix.");
        }
    }
}