using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra.Single;

namespace ALFE.FEModel
{
    public class Pixel : Element
    {
        public Pixel(List<Node> nodes, Material material, bool exist = true)
        {
            foreach (var item in nodes)
            {
                if (item.DOF != 2)
                    throw new Exception("The dof of all nodes in the element must be 2");
                Nodes.Add(item);
            }

            Nodes = nodes;
            Material = material;
            Exist = exist;
            Type = ElementType.PixelElement;
            DOF = 2;
        }

        public override void ComputeD() { }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeKe()
        {
            float coeff = Material.E / (1.0f - Material.nu * Material.nu);

            float[] array = new float[8]
            {
                0.5f-Material.nu/6.0f , 0.125f + Material.nu * 0.125f, -0.25f - Material.nu / 12.0f, -0.125f + Material.nu * 0.375f,
                -0.25f + Material.nu/12f, -0.125f - Material.nu *0.125f, Material.nu / 6.0f, 0.125f- Material.nu*0.375f
             };

            for (int i = 0; i < 8; i++)
            {
                array[i] *= coeff;
            }

            Ke = DenseMatrix.OfArray(new float[8, 8]
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
