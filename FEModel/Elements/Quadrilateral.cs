using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Integration;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE.FEModel
{
    public class Quadrilateral : Element
    {
        public float J;
        public float Thickness = 1.0f;

        public Quadrilateral(List<Node> nodes, Material material, float thichness = 1.0f, bool exist = true)
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
        }

        public override void ComputeD()
        {
            D = new DenseMatrix(3, 3);

            float coeff1 = Material.E / (1.0f - Material.nu * Material.nu);

            D[0, 0] = D[1, 1] = coeff1;
            D[0, 1] = D[1, 0] = Material.nu * coeff1;
            D[2, 2] = (1.0f - Material.nu) * 0.5f * coeff1;
        }

        public void ComputeB(float s, float t)
        {
            var x0 = Nodes[0].Position.X;
            var x1 = Nodes[1].Position.X;
            var x2 = Nodes[2].Position.X;
            var x3 = Nodes[3].Position.X;

            var y0 = Nodes[0].Position.Y;
            var y1 = Nodes[1].Position.Y;
            var y2 = Nodes[2].Position.Y;
            var y3 = Nodes[3].Position.Y;

            var a = 0.25f * (y0 * (s - 1.0f) + y1 * (-1.0f - s) + y2 * (1.0f + s) + y3 * (1.0f - s));
            var b = 0.25f * (y0 * (t - 1.0f) + y1 * (1.0f - t) + y2 * (1.0f + t) + y3 * (-1.0f - t));
            var c = 0.25f * (x0 * (t - 1.0f) + x1 * (1.0f - t) + x2 * (1.0f + t) + x3 * (-1.0f - t));
            var d = 0.25f * (x0 * (s - 1.0f) + x1 * (-1.0f - s) + x2 * (1.0f + s) + x3 * (1.0f - s));

            var b100 = -0.25f * a * (1.0f - t) + 0.25f * b * (1.0f - s);
            var b111 = -0.25f * c * (1.0f - s) + 0.25f * d * (1.0f - t);
            var b120 = b111; var b121 = b100;

            var b200 = 0.25f * a * (1.0f - t) + 0.25f * b * (1.0f + s);
            var b211 = -0.25f * c * (1.0f + s) - 0.25f * d * (1.0f - t);
            var b220 = b211; var b221 = b200;

            var b300 = 0.25f * a * (1.0f + t) - 0.25f * b * (1.0f + s);
            var b311 = 0.25f * c * (1.0f + s) - 0.25f * d * (1.0f + t);
            var b320 = b311; var b321 = b300;

            var b400 = -0.25f * a * (1.0f + t) - 0.25f * b * (1.0f - s);
            var b411 = 0.25f * c * (1.0f - s) + 0.25f * d * (1.0f + t);
            var b420 = b411; var b421 = b400;

            B = DenseMatrix.OfArray(new float[3, 8]
            {
                { b100, 0.0f, b200, 0.0f , b300, 0.0f, b400, 0.0f },
                {0.0f, b111,0.0f,b211, 0.0f, b311, 0.0f, b411 },
                {b120, b121, b220, b221, b320, b321, b420, b421}
            });

            float[,] X = new float[1, 4]
            {
                {x0, x1,x2, x3}
            };
            float[,] Y = new float[4, 1]
            {
                    {y0 }, {y1 }, {y2 }, {y3 }
            };

            float[,] matJ = new float[4, 4]
            {
                {0.0f, 1.0f - t, t - s, s - 1.0f},
                {t - 1.0f, 0.0f, s + 1.0f, -s-t},
                {s-t, -s-1.0f,0.0f, t+1.0f },
                {1.0f-s,s+t,-t-1.0f,0.0f}
            };

            J = DenseMatrix.OfArray(X).Multiply(DenseMatrix.OfArray(matJ)).Multiply(DenseMatrix.OfArray(Y))[0, 0] * 0.125f;
            B = (Matrix)B.Multiply(1.0f / J);
        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            
        }

        public Matrix IntegralFunction(double a, double b)
        {
            ComputeD();
            ComputeB((float)a, (float)b);
            return (Matrix)B.TransposeThisAndMultiply(D).Multiply(B).Multiply(J).Multiply(Thickness);
        }
        public void GaussLegendreIntergration(Func<double, double> func, int n)
        {
            double integrate2D = GaussLegendreRule.Integrate(func, -1.0, 1.0, n);
        }
    }
}
