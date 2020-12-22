namespace ALFE.FEModel
{
    public class Support2D
    {
        public int NodeID { get; set; }
        public SupportType Type;
        public Support2D(int node, SupportType type)
        {
            this.NodeID = node;
            this.Type = type;
        }
    }
}
