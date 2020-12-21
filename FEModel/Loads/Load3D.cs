using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Load3D
    {
        public int Nodes { get; set; }
        public Vector3D Load { get; set; }

        public Load3D(int node, Vector3D load)
        {
            this.Nodes = node;
            this.Load = load;
        }
    }
}
