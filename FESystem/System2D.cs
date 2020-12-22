using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALFE.FEModel;

namespace ALFE.FESystem
{
    public class System2D
    {
        /// <summary>
        ///  Finite element model
        /// </summary>
        public Model2D Model { get; set; }

        /// <summary>
        /// Force Vector
        /// </summary>
        public double[] ForceVector { get; set; }

        /// <summary>
        /// Global stiffness matrix
        /// </summary>
        public double[,] KG { get; set; }

        public System2D(Model2D model)
        {
            this.Model = model;
            int dim = model.Nodes.Count - model.Loads.Count;
            ForceVector = new double[dim];
            KG = new double[dim, dim];

        }

    }
}
