namespace ALFE
{
    public class Support
    {
        public int NodeID { get; set; }
        public SupportType Type;
        public Support(int node, SupportType type)
        {
            this.NodeID = node;
            this.Type = type;
        }
    }
    public enum SupportType
    {
        Fixed = 0
    }
}
