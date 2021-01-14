using ALFE.FEModel;
using ALFE.FESystem;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.TopOpt
{
    public class BESO
    {
        public System2D System;
        public Model2D Model;

        public int P;
        public BESO(System2D system)
        {
            System = system;
            Model = system.Model;
        }
        public void CalSensitivity()
        {
            foreach (var elem in Model.Elements)
            {
                elem.ComputeUe();

                var Ke = elem.Ke;
                var Ue = elem.Ue;
                if (elem.Exist != true)
                    Ke.Multiply((float)Math.Pow(0.001, P));

                elem.C = Ue.TransposeThisAndMultiply(Ke).Multiply(Ue)[0,0];
            }
        }
    }
}
