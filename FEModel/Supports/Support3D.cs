using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Support3D
    {
        public int NodeID { get; set; }
        public SupportType Type;
        public Support3D(int node, SupportType type)
        {
            this.NodeID = node;
            this.Type = type;
        }
    }
}
