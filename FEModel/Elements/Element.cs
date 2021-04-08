using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ALFE
{
    public enum ElementType
    {
        TriangleElement,
        QuadElement,
        SquareElement,
        TetrahedronElement,
        HexahedronElement,
        VoxelElement
    }
    public abstract class Element
    {
        public int ID;

        public List<Node> Nodes = new List<Node>();
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

        /// <summary>
        /// Degree of freedom
        /// </summary>
        public int DOF;

        /// <summary>
        /// Elementary displacement vector
        /// </summary>
        public Matrix<double> Ue;

        /// <summary>
        /// Compute elementary displacement vector
        /// </summary>
        public void ComputeUe()
        {
            Ue = new DenseMatrix(Nodes.Count * DOF, 1);

            int i = 0;
            foreach (var node in Nodes)
            {
                Ue[i * DOF, 0] = node.Displacement.X;
                Ue[i * DOF + 1, 0] = node.Displacement.Y;
                if (DOF == 3)
                    Ue[i * DOF + 2, 0] = node.Displacement.Z;

                i++;                
            }
        }

        public abstract void ComputeD();
        public abstract void ComputeKe();
    }
}
