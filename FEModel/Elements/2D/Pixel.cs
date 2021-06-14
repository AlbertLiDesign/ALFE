using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;

namespace ALFE
{
    public class Pixel : Element
    {
        public Pixel(List<Node> nodes, Material material, bool exist = true)
        {
            foreach (var item in nodes)
            {
                if (item.Dim != 2)
                    throw new Exception("The dof of all nodes in the element must be 2");
                Nodes.Add(item);
            }

            Nodes = nodes;
            Material = material;
            Type = ElementType.PixelElement;
            Dim = 2;
        }

        public override void ComputeD() { }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            double coeff = Material.E / (1.0 - Material.nu * Material.nu);

            double[] array = new double[8]
            {
                0.5-Material.nu/6.0 , 0.125f + Material.nu * 0.125f, -0.25f - Material.nu / 12.0, -0.125f + Material.nu * 0.375f,
                -0.25f + Material.nu/12f, -0.125f - Material.nu *0.125f, Material.nu / 6.0, 0.125f- Material.nu*0.375f
             };

            for (int i = 0; i < 8; i++)
            {
                array[i] *= coeff;
            }

            Ke = DenseMatrix.OfArray(new double[8, 8]
            {
                { array[0], array[1], array[2],array[3],array[4],array[5], array[6], array[7]},
                { array[1], array[0], array[7],array[6],array[5],array[4], array[3], array[2]},
                { array[2], array[7], array[0],array[5],array[6],array[3], array[4], array[1]},
                { array[3], array[6], array[5],array[0],array[7],array[2], array[1], array[4]},
                { array[4], array[5], array[6],array[7],array[0],array[1], array[2], array[3]},
                { array[5], array[4], array[3],array[2],array[1],array[0], array[7], array[6]},
                { array[6], array[3], array[4],array[1],array[2],array[7], array[0], array[5]},
                { array[7], array[2], array[1],array[4],array[3],array[6], array[5], array[0]}
            });
        }
    }
}
