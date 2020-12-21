using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace ALFE.FEModel.Elements
{
    public class Tetrahedron : Element
    {
        public Node3D[] Nodes = new Node3D[4];

        /// <summary>
        /// Stiffness matrix of the element of size 12x12
        /// </summary>
        public double[,] K;

        public static double[,] D;

        public Tetrahedron(int[] nodeID, Node3D[] nodes, Material material, bool exist = true)
        {
            if (nodes.Length != 4 || nodes.Length != 4)
            {
                throw new Exception("The number of nodes must be 4.");
            }
            this.NodeID = nodeID;
            this.Nodes = nodes;
            this.Material = material;
            this.Exist = exist;
            this.Type = ElementType.TetrahedronElement;
        }

        public void ComputeD()
        {
            double coeff1 = Material.E / ((1.0 + Material.u) * (1.0 - 2.0 * Material.u));
            D[0, 0] = D[1, 1] = D[2, 2] = (1.0 - Material.u) * coeff1;
            D[0, 1] = D[0, 2] = D[1, 2] = D[1, 0] = D[2, 0] = D[2, 1] = Material.u * coeff1;
            D[3, 3] = D[4, 4] = D[5, 5] = (0.5 - Material.u) * coeff1;
        }

        public void ComputeK()
        {
            Node3D n1 = Nodes[0];
            Node3D n2 = Nodes[1];
            Node3D n3 = Nodes[2];
            Node3D n4 = Nodes[3];

            // Jacobian of the tetrahedral element
            double[,] J = new double[4, 4]
            {{ 1, 1, 1, 1},
            { n1.Position.X, n2.Position.X, n3.Position.X, n4.Position.X},
            { n1.Position.Y, n2.Position.Y, n3.Position.Y, n4.Position.Y},
            { n1.Position.Z, n2.Position.Z, n3.Position.Z, n4.Position.Z }};

            double[,] Ji = J.Inverse();
            double a1 = Ji[0, 1], a2 = Ji[1, 1], a3 = Ji[2, 1], a4 = Ji[3, 1];
            double b1 = Ji[0, 2], b2 = Ji[1, 2], b3 = Ji[2, 2], b4 = Ji[3, 2];
            double c1 = Ji[0, 3], c2 = Ji[1, 3], c3 = Ji[2, 3], c4 = Ji[3, 3];

            // Strain-displacement matrix B
            double[,] B = new double[6, 12]
            {
                { a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0 },
                { 0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0 },
                { 0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4 },
                { b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0 },
                { 0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4 },
                { c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4 } 
            };


            // K = (BT x D x B) * V
            K = B.TransposeAndDot(D).Multiply(B).Multiply(J.Determinant() / 6.0);
        }
    }
}
