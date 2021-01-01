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
        /// Elementary stiffness matrix
        /// </summary>
        public float[,] Ke;

        public float[,] D;

        public abstract void ComputeD();
        public abstract void ComputeB();
        public abstract void ComputeKe();
    }
}
