namespace ALFE
{
    public class Support
    {
        public int NodeID { get; set; }
        public bool FixedX = true;
        public bool FixedY = true;
        public bool FixedZ = true;

        public Support(int node, bool fixedX, bool fixedY, bool fixedZ)
        {
            NodeID = node;
            FixedX = fixedX;
            FixedY = fixedY;
            FixedZ = fixedZ;
        }
    }
}
