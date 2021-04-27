using System;

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

        public override bool Equals(Object obj)
        {
            if (obj is Triplet)
            {
                if (Row == (obj as Triplet).Row && Col == (obj as Triplet).Col)
                {
                    Value += (obj as Triplet).Value;
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 0;
        }

    }
}
