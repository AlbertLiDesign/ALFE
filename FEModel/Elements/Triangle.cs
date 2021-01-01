using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace ALFE.FEModel
{
    public class Triangle : Element
    {
        public List<Node2D> Nodes;
        public float Area;
        public float[,] B;
        public float Thickness = 1.0f;

        public Triangle(List<Node2D> nodes, Material material, float thickness = 1.0f, bool exist = true)
        {
            if (nodes.Count != 3)
                throw new Exception("The number of nodes must be 3.");
            Nodes = nodes;
            List<int> nodesID = new List<int>(3);
            foreach (var item in nodes)
                nodesID.Add(item.ID);

            NodeID = nodesID;
            Material = material;
            this.Thickness = thickness;
            Exist = exist;
            Type = ElementType.TriangleElement;
            ComputeArea();
        }

        public override void ComputeD()
        {
            D = new float[3, 3];

            float coeff1 = Material.E / (1.0f - Material.u* Material.u);

            D[0, 0] = D[1, 1] = coeff1;
            D[0, 1] = D[1, 0] = Material.u * coeff1;
            D[2, 2] = (1.0f - Material.u) * 0.5f * coeff1;
        }

        public override void ComputeB()
        {
            var val = 1.0f / (2.0f * Area);

            var belta0 = (Nodes[1].Position.Y - Nodes[2].Position.Y) * val;
            var belta1 = (Nodes[2].Position.Y - Nodes[0].Position.Y) * val;
            var belta2 = (Nodes[0].Position.Y - Nodes[1].Position.Y) * val;

            var gama0 = (Nodes[2].Position.X - Nodes[1].Position.X) * val;
            var gama1 = (Nodes[0].Position.X - Nodes[2].Position.X) * val;
            var gama2 = (Nodes[1].Position.X - Nodes[0].Position.X) * val;

            B = new float[3, 6]
               {
                    { belta0, 0.0f, belta1, 0.0f, belta2, 0.0f },
                    { 0.0f, gama0, 0.0f, gama1, 0.0f, gama2 },
                    { gama0, belta0, gama1, belta1, gama2,belta2 }
               };
        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            ComputeB();
            ComputeD();

            Ke = B.TransposeAndDot(D).Dot(B).Multiply(Area).Multiply(Thickness); ;
        }

        public void ComputeArea()
        {
            Area = Math.Abs(Nodes[0].Position.X * (Nodes[1].Position.Y - Nodes[2].Position.Y) +
                Nodes[1].Position.X * (Nodes[2].Position.Y - Nodes[0].Position.Y) + 
                Nodes[2].Position.X * (Nodes[0].Position.Y - Nodes[1].Position.Y)) * 0.5f;
        }
    }
}
