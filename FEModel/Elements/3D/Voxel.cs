using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;

namespace ALFE
{
    public class Voxel : Element
    {
        private double[] Ns;
        private double[] Nt;
        private double[] Nu;
        public Matrix<double> J;
        public Voxel(List<Node> nodes, Material material, bool nondesign = false)
        {
            if (nodes.Count != 8)
                throw new Exception("The number of nodes must be 8.");

            foreach (var item in nodes)
            {
                if (item.Dim != 3)
                    throw new Exception("The dof of all nodes in the element must be 3");
                Nodes.Add(item);
            }

            Material = material;
            Type = ElementType.VoxelElement;
            NonDesign = nondesign;
            Dim = 3;

            J = new DenseMatrix(3, 3);
            B = new DenseMatrix(6, 24);
            Ke = new DenseMatrix(24, 24);
        }
        public override void ComputeD()
        {
            D = new DenseMatrix(6, 6);

            double coeff1 = Material.E / ((1.0 + Material.nu) * (1.0 - 2.0 * Material.nu));

            D[0, 0] = D[1, 1] = D[2, 2] = (1.0 - Material.nu) * coeff1;
            D[0, 1] = D[0, 2] = D[1, 2] = D[1, 0] = D[2, 0] = D[2, 1] = Material.nu * coeff1;
            D[3, 3] = D[4, 4] = D[5, 5] = (1 - 2 * Material.nu) * coeff1 * 0.5;
        }

        public DenseMatrix ComputeJ(double s = 0.0, double t = 0.0, double u = 0.0)
        {
            var N1s = -(1 - t) * (1 - u) * 0.125;
            var N1t = -(1 - s) * (1 - u) * 0.125;
            var N1u = -(1 - s) * (1 - t) * 0.125;

            var N2s = (1 - t) * (1 - u) * 0.125;
            var N2t = -(1 + s) * (1 - u) * 0.125;
            var N2u = -(1 + s) * (1 - t) * 0.125;

            var N3s = (1 + t) * (1 - u) * 0.125;
            var N3t = (1 + s) * (1 - u) * 0.125;
            var N3u = -(1 + s) * (1 + t) * 0.125;

            var N4s = -(1 + t) * (1 - u) * 0.125;
            var N4t = (1 - s) * (1 - u) * 0.125;
            var N4u = -(1 - s) * (1 + t) * 0.125;

            var N5s = -(1 - t) * (1 + u) * 0.125;
            var N5t = -(1 - s) * (1 + u) * 0.125;
            var N5u = (1 - s) * (1 - t) * 0.125;

            var N6s = (1 - t) * (1 + u) * 0.125;
            var N6t = -(1 + s) * (1 + u) * 0.125;
            var N6u = (1 + s) * (1 - t) * 0.125;

            var N7s = (1 + t) * (1 + u) * 0.125;
            var N7t = (1 + s) * (1 + u) * 0.125;
            var N7u = (1 + s) * (1 + t) * 0.125;

            var N8s = -(1 + t) * (1 + u) * 0.125;
            var N8t = (1 - s) * (1 + u) * 0.125;
            var N8u = (1 - s) * (1 + t) * 0.125;

            Ns = new double[8]
            {
                N1s, N2s, N3s, N4s, N5s, N6s, N7s, N8s
            };

            Nt = new double[8]
            {
                N1t, N2t, N3t, N4t, N5t, N6t, N7t, N8t
            };

            Nu = new double[8]
            {
                N1u, N2u, N3u, N4u, N5u, N6u, N7u, N8u
            };

            double xs = 0.0, xt = 0.0, xu = 0.0;
            double ys = 0.0, yt = 0.0, yu = 0.0;
            double zs = 0.0, zt = 0.0, zu = 0.0;
            for (int i = 0; i < 8; i++)
            {
                xs += Ns[i] * Nodes[i].Position.X;
                xt += Nt[i] * Nodes[i].Position.X;
                xu += Nu[i] * Nodes[i].Position.X;

                ys += Ns[i] * Nodes[i].Position.Y;
                yt += Nt[i] * Nodes[i].Position.Y;
                yu += Nu[i] * Nodes[i].Position.Y;

                zs += Ns[i] * Nodes[i].Position.Z;
                zt += Nt[i] * Nodes[i].Position.Z;
                zu += Nu[i] * Nodes[i].Position.Z;
            }

            return DenseMatrix.OfArray(new double[3, 3]
            {
                {xs, ys, zs},
                {xt, yt, zt},
                {xu, yu, zu}
            });
        }

        public DenseMatrix ComputeB(Matrix<double> J, double s = 0.0, double t = 0.0, double u = 0.0)
        {
            var J_v = J.Inverse();

            double[] Nx = new double[8];
            double[] Ny = new double[8];
            double[] Nz = new double[8];
            for (int i = 0; i < 8; i++)
            {
                Nx[i] = J_v[0, 0] * Ns[i] + J_v[0, 1] * Nt[i] + J_v[0, 2] * Nu[i];
                Ny[i] = J_v[1, 0] * Ns[i] + J_v[1, 1] * Nt[i] + J_v[1, 2] * Nu[i];
                Nz[i] = J_v[2, 0] * Ns[i] + J_v[2, 1] * Nt[i] + J_v[2, 2] * Nu[i];
            }

            return DenseMatrix.OfArray(new double[6, 24]
            {
                   { Nx[0], 0, 0, Nx[1], 0, 0, Nx[2], 0, 0, Nx[3], 0, 0, Nx[4], 0, 0, Nx[5], 0, 0, Nx[6], 0, 0, Nx[7], 0, 0 },
                   { 0, Ny[0], 0, 0, Ny[1], 0, 0, Ny[2], 0, 0, Ny[3], 0, 0, Ny[4], 0, 0, Ny[5], 0, 0, Ny[6], 0, 0, Ny[7], 0 },
                   { 0, 0, Nz[0], 0, 0, Nz[1], 0, 0, Nz[2], 0, 0, Nz[3], 0, 0, Nz[4], 0, 0, Nz[5], 0, 0, Nz[6], 0, 0, Nz[7]},
                   { Ny[0], Nx[0], 0, Ny[1], Nx[1], 0, Ny[2], Nx[2], 0, Ny[3], Nx[3], 0, Ny[4], Nx[4], 0, Ny[5], Nx[5], 0, Ny[6], Nx[6], 0, Ny[7], Nx[7], 0},
                   { 0, Nz[0], Ny[0], 0, Nz[1], Ny[1], 0, Nz[2], Ny[2], 0, Nz[3], Ny[3], 0, Nz[4], Ny[4], 0, Nz[5], Ny[5], 0, Nz[6], Ny[6], 0, Nz[7], Ny[7]},
                   { Nz[0], 0, Nx[0], Nz[1], 0, Nx[1], Nz[2], 0, Nx[2], Nz[3], 0, Nx[3], Nz[4], 0, Nx[4], Nz[5], 0, Nx[5], Nz[6], 0, Nx[6], Nz[7], 0, Nx[7]}
            });
        }


        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            ComputeD();

            GaussLegendreQuadrature glq = new GaussLegendreQuadrature(3);

            for (int i = 0; i < glq.Xi.Count; i++)
            {
                for (int j = 0; j < glq.Xi.Count; j++)
                {
                    for (int k = 0; k < glq.Xi.Count; k++)
                    {
                        var quad_J = ComputeJ(glq.Xi[i], glq.Xi[j], glq.Xi[k]);
                        var quad_B = ComputeB(quad_J, glq.Xi[i], glq.Xi[j], glq.Xi[k]);
                        Ke += glq.Weights[i] * glq.Weights[j] * glq.Weights[k] * quad_B.TransposeThisAndMultiply(D).Multiply(quad_B).Multiply(quad_J.Determinant());
                    }
                }
            }
        }
    }
}
