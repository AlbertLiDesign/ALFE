using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public abstract class Element
    {
        public List<int> NodeID = new List<int>();

        public Material Material { get; set; }

        public bool Exist { get; set; }

        public ElementType Type;

        /// <summary>
        /// Stiffness matrix of the element of size 8x8
        /// </summary>
        public double[,] Ke;

        // 3x3 matrix
        public double[,] D;

        public abstract void ComputeD();
        public abstract void ComputeK();
    }
}
