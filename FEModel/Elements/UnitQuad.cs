using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class UnitQuad : Element
    {
        public Node2D[] Nodes = new Node2D[4];

        public UnitQuad(int[] nodeID, Node2D[] nodes, Material material, bool exist = true)
        {
            if (nodes.Length != 4 || nodes.Length != 4)
            {
                throw new Exception("The number of nodes must be 4.");
            }
            this.NodeID = nodeID;
            this.Nodes = nodes;
            this.Material = material;
            this.Exist = exist;
            this.Type = ElementType.UnitQuadElement;
        }

        public override void ComputeD()
        {
            D = new double[3, 3];

            double coeff1 = Material.E / ((1.0 - Material.u) * (1.0 - Material.u));

            D[0, 0] = D[1, 1] = coeff1;
            D[0, 1] = D[1, 0] = Material.u * coeff1;
            D[3, 3] = (1.0 - Material.u) * 0.5 * coeff1;
        }

        /// <summary>
        /// Compute the stiffness matrix
        /// </summary>
        public override void ComputeK()
        {
            double coeff = Material.E / ((1.0 - Material.u) * (1.0 - Material.u));

            double[] array = new double[8]
            {
                0.5-Material.u/6.0 * coeff, 0.125 + Material.u * 0.125 * coeff, -0.25 - Material.u / 12.0 * coeff, -0.125 + Material.u * 0.375 * coeff,
                -0.25 + Material.u/12 * coeff, -0.125 - Material.u *0.125 * coeff, Material.u / 6 * coeff, 0.125- Material.u*0.375 * coeff
             };

            K = new double[8, 8]
            {
                { array[0], array[1], array[2],array[3],array[4],array[5], array[6], array[7]},
                { array[1], array[0], array[7],array[6],array[5],array[4], array[3], array[2]},
                { array[2], array[7], array[0],array[5],array[6],array[3], array[4], array[1]},
                { array[3], array[6], array[5],array[0],array[7],array[2], array[1], array[4]},
                { array[4], array[5], array[6],array[7],array[0],array[1], array[2], array[3]},
                { array[5], array[4], array[3],array[2],array[1],array[0], array[7], array[6]},
                { array[6], array[3], array[4],array[1],array[2],array[7], array[0], array[5]},
                { array[7], array[2], array[1],array[4],array[3],array[6], array[5], array[0]}
            };
        }
    }
}
