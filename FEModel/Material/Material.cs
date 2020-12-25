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
        public float E { get; set; }

        /// <summary>
        /// Poisson ratio
        /// </summary>
        public float u { get; set; }
        public Material() 
        {
            this.E = 1.0f;
            this.u = 0.3f;
        }
        public Material(float E, float u)
        {
            this.E = E;
            this.u = u;
        }
    }
}
