using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE.FEModel
{
    public class Triangle : Element
    {
        public float Area;
        public float Thickness = 1.0f;

        public Triangle(List<Node2D> nodes, Material material, float thickness = 1.0f, bool exist = true)
        {
            if (nodes.Count != 3)
                throw new Exception("The number of nodes must be 3.");

            foreach (var item in nodes)
                Nodes.Add(item);

            Material = material;
            Thickness = thickness;
            Exist = exist;
            Type = ElementType.TriangleElement;
            ComputeArea();
        }

        public override void ComputeD()
        {
            D = new DenseMatrix(3, 3);

            float coeff1 = Material.E / (1.0f - Material.u* Material.u);

            D[0, 0] = D[1, 1] = coeff1;
            D[0, 1] = D[1, 0] = Material.u * coeff1;
            D[2, 2] = (1.0f - Material.u) * 0.5f * coeff1;
        }

        public void ComputeB()
        {
            var val = 1.0f / (2.0f * Area);

            var belta0 = ((Nodes[1] as Node2D).Position.Y - (Nodes[2] as Node2D).Position.Y) * val;
            var belta1 = ((Nodes[2] as Node2D).Position.Y - (Nodes[0] as Node2D).Position.Y) * val;
            var belta2 = ((Nodes[0] as Node2D).Position.Y - (Nodes[1] as Node2D).Position.Y) * val;

            var gama0 = ((Nodes[2]as Node2D).Position.X - (Nodes[1] as Node2D).Position.X) * val;
            var gama1 = ((Nodes[0]as Node2D).Position.X - (Nodes[2] as Node2D).Position.X) * val;
            var gama2 = ((Nodes[1] as Node2D).Position.X - (Nodes[0] as Node2D).Position.X) * val;

            B = DenseMatrix.OfArray(new float[3,6]
               {
                    { belta0, 0.0f, belta1, 0.0f, belta2, 0.0f },
                    { 0.0f, gama0, 0.0f, gama1, 0.0f, gama2 },
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
            Area = Math.Abs((Nodes[0] as Node2D).Position.X * ((Nodes[1] as Node2D).Position.Y - (Nodes[2] as Node2D).Position.Y) +
                (Nodes[1] as Node2D).Position.X * ((Nodes[2] as Node2D).Position.Y - (Nodes[0] as Node2D).Position.Y) + 
                (Nodes[2] as Node2D).Position.X * ((Nodes[0] as Node2D).Position.Y - (Nodes[1] as Node2D).Position.Y)) * 0.5f;
        }
    }
}
