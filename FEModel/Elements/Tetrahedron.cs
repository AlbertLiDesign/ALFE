using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE.FEModel
{
    public class Tetrahedron : Element
    {
        public Tetrahedron(List<Node> nodes, Material material, bool exist = true)
        {
            if (nodes.Count != 4)
                throw new Exception("The number of nodes must be 4.");

            foreach (var item in nodes)
            {
                if (item.Dof != 3)
                    throw new Exception("The dof of all nodes in the element must be 3");
                Nodes.Add(item);
            }
                
            Material = material;
            Exist = exist;
            Type = ElementType.TetrahedronElement;
            DOF = 3;
        }

        public override void ComputeD()
        {
            D = new DenseMatrix(6, 6);

            float coeff1 = Material.E / ((1.0f + Material.nu) * (1.0f - 2.0f * Material.nu));

            D[0, 0] = D[1, 1] = D[2, 2] = (1.0f - Material.nu) * coeff1;
            D[0, 1] = D[0, 2] = D[1, 2] = D[1, 0] = D[2, 0] = D[2, 1] = Material.nu * coeff1;
            D[3, 3] = D[4, 4] = D[5, 5] = (0.5f - Material.nu) * coeff1;
        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            ComputeD();

            Node n0 = Nodes[0];
            Node n1 = Nodes[1];
            Node n2 = Nodes[2];
            Node n3 = Nodes[3];

            // Jacobian of the tetrahedral element
            var J = DenseMatrix.OfArray(new float[4, 4]
            {
                {1.0f, 1.0f, 1.0f, 1.0f},
                { n0.Position.X, n1.Position.X, n2.Position.X, n3.Position.X },
                { n0.Position.Y, n1.Position.Y, n2.Position.Y, n3.Position.Y },
                { n0.Position.Z, n1.Position.Z, n2.Position.Z, n3.Position.Z }
            });

            var Ji = J.Inverse();
            float a0 = Ji[0, 1], a1 = Ji[1, 1], a2 = Ji[2, 1], a3 = Ji[3, 1];
            float b0 = Ji[0, 2], b1 = Ji[1, 2], b2 = Ji[2, 2], b3 = Ji[3, 2];
            float c0 = Ji[0, 3], c1 = Ji[1, 3], c2 = Ji[2, 3], c3 = Ji[3, 3];

            B = DenseMatrix.OfArray(new float[6, 12]
            {
                {a0, 0.0f, 0.0f, a1, 0.0f, 0.0f, a2, 0.0f, 0.0f, a3, 0.0f, 0.0f},
                {0.0f, b0, 0.0f, 0.0f, b1, 0.0f, 0.0f, b2, 0.0f, 0.0f, b3, 0.0f},
                {0.0f, 0.0f, c0, 0.0f, 0.0f, c1, 0.0f, 0.0f, c2, 0.0f, 0.0f, c3},
                {b0, a0, 0.0f, b1, a1, 0.0f, b2, a2, 0.0f, b3, a3, 0.0f},
                {0.0f, c0, b0, 0.0f, c1, b1, 0.0f, c2, b2, 0.0f, c3, b3},
                {c0, 0.0f, a0, c1, 0.0f, a1, c2, 0.0f, a2, c3, 0.0f, a3}
            });

            Ke = (Matrix)(B.TransposeThisAndMultiply(D).Multiply(B).Multiply(J.Determinant()) / 6.0f);
        }
    }
}
