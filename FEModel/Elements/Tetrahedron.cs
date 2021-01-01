using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel.Elements
{
    public class Tetrahedron : Element
    {
        public Tetrahedron(List<int> nodesID, Material material, bool exist = true)
        {
            if (nodesID.Count != 4 || nodesID.Count != 4)
            {
                throw new Exception("The number of nodes must be 4.");
            }

            NodeID = nodesID;
            Material = material;
            Exist = exist;
            Type = ElementType.TetrahedronElement;
        }

        public override void ComputeD()
        {
            D = new float[6, 6];

            float coeff1 = Material.E / ((1.0f + Material.u) * (1.0f - 2.0f * Material.u));
            D[0, 0] = D[1, 1] = D[2, 2] = (1.0f - Material.u) * coeff1;
            D[0, 1] = D[0, 2] = D[1, 2] = D[1, 0] = D[2, 0] = D[2, 1] = Material.u * coeff1;
            D[3, 3] = D[4, 4] = D[5, 5] = (0.5f - Material.u) * coeff1;
        }

        public override void ComputeB()
        {

        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            //ComputeD();

            //Node3D n1 = (Node3D)Nodes[0];
            //Node3D n2 = (Node3D)Nodes[1];
            //Node3D n3 = (Node3D)Nodes[2];
            //Node3D n4 = (Node3D)Nodes[3];

            //// Jacobian of the tetrahedral element
            //float[,] J = new float[4, 4]
            //{{ 1, 1, 1, 1},
            //{ n1.Position.X, n2.Position.X, n3.Position.X, n4.Position.X},
            //{ n1.Position.Y, n2.Position.Y, n3.Position.Y, n4.Position.Y},
            //{ n1.Position.Z, n2.Position.Z, n3.Position.Z, n4.Position.Z }};

            //float[,] Ji = J.Inverse();
            //float a1 = Ji[0, 1], a2 = Ji[1, 1], a3 = Ji[2, 1], a4 = Ji[3, 1];
            //float b1 = Ji[0, 2], b2 = Ji[1, 2], b3 = Ji[2, 2], b4 = Ji[3, 2];
            //float c1 = Ji[0, 3], c2 = Ji[1, 3], c3 = Ji[2, 3], c4 = Ji[3, 3];

            //// Strain-displacement matrix B
            //float[,] B = new float[6, 12]
            //{
            //    { a1, 0, 0, a2, 0, 0, a3, 0, 0, a4, 0, 0 },
            //    { 0, b1, 0, 0, b2, 0, 0, b3, 0, 0, b4, 0 },
            //    { 0, 0, c1, 0, 0, c2, 0, 0, c3, 0, 0, c4 },
            //    { b1, a1, 0, b2, a2, 0, b3, a3, 0, b4, a4, 0 },
            //    { 0, c1, b1, 0, c2, b2, 0, c3, b3, 0, c4, b4 },
            //    { c1, 0, a1, c2, 0, a2, c3, 0, a3, c4, 0, a4 } 
            //};


            //// Ke = (BT x D x B) * V
            //Ke = B.TransposeAndDot(D).Multiply(B).Multiply((float)J.Determinant() / 6.0f);
        }
    }
}
