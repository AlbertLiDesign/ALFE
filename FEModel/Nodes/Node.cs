using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Node
    {
        public List<int> Neighbours = new List<int>();

        /// <summary>
        /// Whether the node has been applied a boundary condition
        /// </summary>
        public bool Anchored = false;

        public List<int> ElementID = new List<int>();
    }
}
