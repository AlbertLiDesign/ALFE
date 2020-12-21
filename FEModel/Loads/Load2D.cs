using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Load2D
    {
        /// <summary>
        /// The index of the node, which has been applied a load.
        /// </summary>
        public int Nodes { get; set; }

        public Vector2D Load { get; set; }
        public Load2D(int node, Vector2D load)
        {
            this.Nodes = node;
            this.Load = load;
        }
    }
}
