namespace ALFE.FEModel
{
    public class Support2D
    {
        public int Nodes { get; set; }
        public SupportType Type;
        public Support2D(int node, SupportType type)
        {
            this.Nodes = node;
            this.Type = type;
        }
    }
}
