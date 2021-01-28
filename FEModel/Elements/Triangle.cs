using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ALFE
{
    public class Triangle : Element
    {
        public double Area;
        public double Thickness = 1.0;

        public Triangle(List<Node> nodes, Material material, double thickness = 1.0, bool exist = true)
        {
            if (nodes.Count != 3)
                throw new Exception("The number of nodes must be 3.");

            foreach (var item in nodes)
            {
                if (item.DOF != 2)
                    throw new Exception("The dof of all nodes in the element must be 2");
                Nodes.Add(item);
            }

            Material = material;
            Thickness = thickness;
            Exist = exist;
            Type = ElementType.TriangleElement;
            DOF = 2;
            ComputeArea();
        }

        public override void ComputeD()
        {
            D = new DenseMatrix(3, 3);

            double coeff1 = Material.E / (1.0 - Material.nu* Material.nu);

            D[0, 0] = D[1, 1] = coeff1;
            D[0, 1] = D[1, 0] = Material.nu * coeff1;
            D[2, 2] = (1.0 - Material.nu) * 0.5 * coeff1;
        }

        public void ComputeB()
        {
            var val = 1.0 / (2.0 * Area);

            var belta0 = (Nodes[1].Position.Y - Nodes[2].Position.Y) * val;
            var belta1 = (Nodes[2].Position.Y - Nodes[0].Position.Y) * val;
            var belta2 = (Nodes[0].Position.Y - Nodes[1].Position.Y) * val;

            var gama0 = (Nodes[2].Position.X - Nodes[1].Position.X) * val;
            var gama1 = (Nodes[0].Position.X - Nodes[2].Position.X) * val;
            var gama2 = (Nodes[1].Position.X - Nodes[0].Position.X) * val;

            B = DenseMatrix.OfArray(new double[3,6]
               {
                    { belta0, 0.0, belta1, 0.0, belta2, 0.0 },
                    { 0.0, gama0, 0.0, gama1, 0.0, gama2 },
                    { gama0, belta0, gama1, belta1, gama2,belta2 }
               });
        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            ComputeB();
            ComputeD();

            Ke = (Matrix)B.TransposeThisAndMultiply(D).Multiply(B).Multiply(Area).Multiply(Thickness);
        }

        public void ComputeArea()
        {
            Area = Math.Abs(Nodes[0].Position.X * (Nodes[1].Position.Y - Nodes[2].Position.Y) +
                Nodes[1].Position.X * (Nodes[2].Position.Y - Nodes[0].Position.Y) + 
                Nodes[2].Position.X * (Nodes[0].Position.Y - Nodes[1].Position.Y)) * 0.5;
        }
    }
}
