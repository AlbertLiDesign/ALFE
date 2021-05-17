using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Collections.Generic;

namespace ALFE
{
    public enum ElementType
    {
        TriangleElement,
        QuadElement,
        PixelElement,
        TetrahedronElement,
        HexahedronElement,
        VoxelElement,
        QuadTreeElement,
    }
    public abstract class Element
    {
        public int ID;

        public List<Node> Nodes = new List<Node>();
        public Dictionary<int, int> DOF_ID = new Dictionary<int, int>();
        public Material Material { get; set; }

        /// <summary>
        /// Design Variable
        /// </summary>
        public double Xe;

        /// <summary>
        /// Compliance
        /// </summary>
        public double C;
        public bool Exist { get; set; }

        public ElementType Type;

        /// <summary>
        /// Elementary stiffness matrix
        /// </summary>
        public Matrix<double> Ke;
        public Matrix<double> B;
        public Matrix<double> D;

        public int Dim;

        /// <summary>
        /// Elementary displacement vector
        /// </summary>
        public Matrix<double> Ue;

        /// <summary>
        /// Compute elementary displacement vector
        /// </summary>
        public void ComputeUe()
        {
            Ue = new DenseMatrix(Nodes.Count * Dim, 1);

            int i = 0;
            foreach (var node in Nodes)
            {
                Ue[i * Dim, 0] = node.Displacement.X;
                Ue[i * Dim + 1, 0] = node.Displacement.Y;
                if (Dim == 3)
                    Ue[i * Dim + 2, 0] = node.Displacement.Z;

                i++;
            }
        }

        public abstract void ComputeD();
        public abstract void ComputeKe();
    }
}
