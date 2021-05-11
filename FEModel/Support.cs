namespace ALFE
{
    public class Support
    {
        public int NodeID { get; set; }
        public bool FixedX = true;
        public bool FixedY = true;
        public bool FixedZ = true;
        public int Dim;

        public Support(int node, bool fixedX, bool fixedY)
        {
            NodeID = node;
            FixedX = fixedX;
            FixedY = fixedY;
            Dim = 2;
        }
        public Support(int node, bool fixedX, bool fixedY, bool fixedZ)
        {
            NodeID = node;
            FixedX = fixedX;
            FixedY = fixedY;
            FixedZ = fixedZ;
            Dim = 3;
        }
    }
}
