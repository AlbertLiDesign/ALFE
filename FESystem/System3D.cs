using ALFE.FEModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FESystem
{
    public class System3D
    {
        /// <summary>
        ///  Finite element model
        /// </summary>
        public Model3D Model { get; set; }

        /// <summary>
        /// Force Vector
        /// </summary>
        public float[] ForceVector { get; set; }

        /// <summary>
        /// Global stiffness matrix
        /// </summary>
        public float[,] KG { get; set; }

        public System3D(Model3D model)
        {
            this.Model = model;

            ApplySupports3D(this.Model.Nodes, this.Model.Supports);

            int dim = model.Nodes.Count - model.Loads.Count;
            ForceVector = new float[dim];
            KG = new float[dim, dim];

        }
        public static void ApplySupports3D(List<Node3D> nodes, List<Support3D> supports)
        {
            foreach (var item in supports)
            {
                nodes[item.NodeID].Anchored = true;
                if (item.Type == SupportType.Fixed)
                {
                    nodes[item.NodeID].Displacement.X = 0.0f;
                    nodes[item.NodeID].Displacement.Y = 0.0f;
                    nodes[item.NodeID].Displacement.Z = 0.0f;
                }
            }
        }

    }
}
