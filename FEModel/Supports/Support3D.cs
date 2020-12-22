using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Support3D
    {
        public int Nodes { get; set; }
        public SupportType Type;
        public Support3D(int node, SupportType type)
        {
            this.Nodes = node;
            this.Type = type;
        }
    }
}
