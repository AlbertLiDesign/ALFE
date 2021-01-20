using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE
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
        public int ID;

        public List<Node> Nodes = new List<Node>();
        public Material Material { get; set; }

        /// <summary>
        /// Design Variable
        /// </summary>
        public float Xe;

        /// <summary>
        /// Strain energy
        /// </summary>
        public float C;
        public bool Exist { get; set; }

        public ElementType Type;

        /// <summary>
        /// Elementary stiffness matrix
        /// </summary>
        public Matrix Ke;
        public Matrix B;
        public Matrix D;

        public int DOF;

        /// <summary>
        /// Elementary displacement vector
        /// </summary>
        public Matrix Ue;

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
