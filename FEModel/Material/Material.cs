using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Material
    {
        /// <summary>
        /// // Young's modulus
        /// </summary>
        public double E { get; set; }

        /// <summary>
        /// Poisson ratio
        /// </summary>
        public double u { get; set; }
        public Material() 
        {
            this.E = 1;
            this.u = 0.35;
        }
        public Material(double E, double u)
        {
            this.E = E;
            this.u = u;
        }
    }
}
