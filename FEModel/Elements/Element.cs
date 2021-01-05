using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE.FEModel
{
    public enum ElementType
    {
        TriangleElement,
        QuadElement,
        PixelElement,
        TetrahedronElement,
        HexahedronElement,
        VoxelElement
    }
    public abstract class Element
    {
        public List<int> NodeID = new List<int>();
        public List<Node> Nodes = new List<Node>();
        public Material Material { get; set; }

        public bool Exist { get; set; }

        public ElementType Type;

        /// <summary>
        /// Elementary stiffness matrix
        /// </summary>
        public Matrix Ke;
        public Matrix B;
        public Matrix D;

        public abstract void ComputeD();
        public abstract void ComputeKe();
    }
}
