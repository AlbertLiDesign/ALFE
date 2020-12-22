using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class Triplet
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public double Value { get; set; }
        public Triplet(int row, int col, double value)
        {
            Row = row;
            Col = col;
            Value = value;
        }
    }
}
