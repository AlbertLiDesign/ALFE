using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Load3D
    {
        public int NodeID { get; set; }
        public Vector3D Load { get; set; }

        public Load3D() { }
        public Load3D(int node, Vector3D load)
        {
            this.NodeID = node;
            this.Load = load;
        }
    }
}
