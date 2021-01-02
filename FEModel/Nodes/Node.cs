using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Node
    {
        /// <summary>
        /// The neighbours of the nodes
        /// </summary>
        public HashSet<int> Neighbours = new HashSet<int>();

        /// <summary>
        /// Whether the node has been applied a boundary condition
        /// </summary>
        public bool Active = true;

        public List<int> ElementID = new List<int>();

        public int ID;
        public bool hasID = false;

        /// <summary>
        /// If the node is active, it will get an ID.
        /// </summary>
        public int ActiveID;

        public int row_nnz;
        public SortedList<int, int> PositionKG;
    }
}
