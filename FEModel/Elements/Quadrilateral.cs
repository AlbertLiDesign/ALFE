using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ALFE
{
    public class Quadrilateral : Element
    {
        public double J;
        public double Thickness = 1.0;

        public Quadrilateral(List<Node> nodes, Material material, double thichness = 1.0, bool exist = true)
        {
            if (nodes.Count != 4)
                throw new Exception("The number of nodes must be 4.");

            foreach (var item in nodes)
            {
                if (item.DOF != 2)
                    throw new Exception("The dof of all nodes in the element must be 2");
                Nodes.Add(item);
            }

            Material = material;
            Thickness = thichness;
            Exist = exist;
            Type = ElementType.QuadElement;
            DOF = 2;

            D = new DenseMatrix(3, 3);
            B = new DenseMatrix(3, 8);
            Ke = new DenseMatrix(8, 8);
        }

        public override void ComputeD()
        {
            double coeff1 = Material.E / (1.0 - Material.nu * Material.nu);

            D[0, 0] = D[1, 1] = coeff1;
            D[0, 1] = D[1, 0] = Material.nu * coeff1;
            D[2, 2] = (1.0 - Material.nu) * 0.5 * coeff1;
        }

        public DenseMatrix ComputeB(double s=0.0, double t = 0.0)
        {
            var x0 = Nodes[0].Position.X;
            var x1 = Nodes[1].Position.X;
            var x2 = Nodes[2].Position.X;
            var x3 = Nodes[3].Position.X;

            var y0 = Nodes[0].Position.Y;
            var y1 = Nodes[1].Position.Y;
            var y2 = Nodes[2].Position.Y;
            var y3 = Nodes[3].Position.Y;

            var a = 0.25f * (y0 * (s - 1.0) + y1 * (-1.0 - s) + y2 * (1.0 + s) + y3 * (1.0 - s));
            var b = 0.25f * (y0 * (t - 1.0) + y1 * (1.0 - t) + y2 * (1.0 + t) + y3 * (-1.0 - t));
            var c = 0.25f * (x0 * (t - 1.0) + x1 * (1.0 - t) + x2 * (1.0 + t) + x3 * (-1.0 - t));
            var d = 0.25f * (x0 * (s - 1.0) + x1 * (-1.0 - s) + x2 * (1.0 + s) + x3 * (1.0 - s));

            var b100 = -0.25f * a * (1.0 - t) + 0.25f * b * (1.0 - s);
            var b111 = -0.25f * c * (1.0 - s) + 0.25f * d * (1.0 - t);
            var b120 = b111; var b121 = b100;

            var b200 = 0.25f * a * (1.0 - t) + 0.25f * b * (1.0 + s);
            var b211 = -0.25f * c * (1.0 + s) - 0.25f * d * (1.0 - t);
            var b220 = b211; var b221 = b200;

            var b300 = 0.25f * a * (1.0 + t) - 0.25f * b * (1.0 + s);
            var b311 = 0.25f * c * (1.0 + s) - 0.25f * d * (1.0 + t);
            var b320 = b311; var b321 = b300;

            var b400 = -0.25f * a * (1.0 + t) - 0.25f * b * (1.0 - s);
            var b411 = 0.25f * c * (1.0 - s) + 0.25f * d * (1.0 + t);
            var b420 = b411; var b421 = b400;

            return DenseMatrix.OfArray(new double[3, 8]
            {
                { b100, 0.0, b200, 0.0 , b300, 0.0, b400, 0.0 },
                {0.0, b111,0.0,b211, 0.0, b311, 0.0, b411 },
                {b120, b121, b220, b221, b320, b321, b420, b421}
            });
        }

        public double ComputeJ(double s = 0.0, double t = 0.0)
        {
            var x0 = Nodes[0].Position.X;
            var x1 = Nodes[1].Position.X;
            var x2 = Nodes[2].Position.X;
            var x3 = Nodes[3].Position.X;

            var y0 = Nodes[0].Position.Y;
            var y1 = Nodes[1].Position.Y;
            var y2 = Nodes[2].Position.Y;
            var y3 = Nodes[3].Position.Y;

            double[,] X = new double[1, 4]
{
                {x0, x1,x2, x3}
};
            double[,] Y = new double[4, 1]
            {
               {y0 }, {y1 }, {y2 }, {y3 }
            };

            double[,] matJ = new double[4, 4]
{
                {0.0, 1.0 - t, t - s, s - 1.0},
                {t - 1.0, 0.0, s + 1.0, -s-t},
                {s-t, -s-1.0,0.0, t+1.0 },
                {1.0-s,s+t,-t-1.0,0.0}
};

            return DenseMatrix.OfArray(X).Multiply(DenseMatrix.OfArray(matJ)).Multiply(DenseMatrix.OfArray(Y))[0, 0] * 0.125;
        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            ComputeD();
            J = ComputeJ();
            B = ComputeB()/J;
            
            GaussLegendreQuadrature glq = new GaussLegendreQuadrature(3);
            for (int i = 0; i < glq.Xi.Count; i++)
            {
                var quad_J = ComputeJ(glq.Xi[i], glq.Xi[i]);
                var quad_B = ComputeB(glq.Xi[i], glq.Xi[i] ).Multiply(1.0/quad_J);
                Ke += glq.Weights[i] * Thickness * quad_B.TransposeThisAndMultiply(D).Multiply(quad_B).Multiply(quad_J);
            }
        }
    }
}
