using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra.Double;

namespace ALFE
{
    public class QuadTree : Element
    {
        /// <summary>
        /// Node position
        /// </summary>
        public Vector3D Position { get; set; }

        /// <summary>
        /// Edge length
        /// </summary>
        public double EdgeLength { get; set; }

        /// <summary>
        /// The layer level
        /// </summary>
        public int LayerLevel { get; set; }

        /// <summary>
        /// The branches
        /// </summary>
        public List<QuadTree> Branches { get; set; }

        /// <summary>
        /// The value
        /// </summary>
        public double Value { get; set; }

        #region Constructive Methods
        public QuadTree(List<Node> nodes, double x, double y, double edgeLength, double value, int layerLevel, Material material, bool exist = true)
        {
            Nodes = nodes;
            Position = new Vector3D(x, y, 0.0);
            EdgeLength = edgeLength;
            LayerLevel = layerLevel;
            Branches = new List<QuadTree>();
            Value = value;

            Material = material;
            Exist = exist;
            Type = ElementType.QuadTreeElement;
            Dim = 2;
        }

        public QuadTree(List<Node> nodes, Vector3D position, double edgeLength, double value, int layerLevel, Material material, bool exist = true)
        {
            Nodes = nodes;
            Position = position;
            EdgeLength = edgeLength;
            LayerLevel = layerLevel;
            Branches = new List<QuadTree>();
            Value = value;

            Material = material;
            Exist = exist;
            Type = ElementType.QuadTreeElement;
            Dim = 2;
        }
        #endregion

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
