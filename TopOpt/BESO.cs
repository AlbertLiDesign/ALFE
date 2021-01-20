using ALFE;
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
        public FESystem System;
        public Model Model;

        /// <summary>
        /// Minimum filter radius
        /// </summary>
        public float Rmin;

        /// <summary>
        /// .Target volume
        /// </summary>
        public float Vt;

        /// <summary>
        /// Sensitivity
        /// </summary>
        public List<float> Ae;

        /// <summary>
        /// Global strain energy
        /// </summary>
        public float SumC;


        public int P;

        /// <summary>
        /// The maximum iteration
        /// </summary>
        public int MaxIteration;

        public BESO(FESystem system, float rmin, int p=3,  float vt=0.5f, int maxIter=100)
        {
            System = system;
            Model = system.Model;
            Vt = vt;
            P = p;
            MaxIteration = maxIter;
            Rmin = rmin;
        }

        public void Optimize()
        {
            Filter filter = new Filter(Model.Elements, Rmin, 2);



            CalSensitivity();
            CalGlobalStrainEnergy();
        }
        private void CalSensitivity()
        {
            float[] Sensitivities = new float[Model.Elements.Count];
            Parallel.ForEach(Model.Elements, elem =>
            {
               elem.ComputeUe();

               var Ke = elem.Ke;
               var Ue = elem.Ue;
               if (elem.Exist != true)
                   Ke.Multiply((float)Math.Pow(0.001, P));

               elem.C = Ue.TransposeThisAndMultiply(Ke).Multiply(Ue)[0, 0];

                Sensitivities[elem.ID] = elem.C / elem.Xe;
            });

            Ae = Sensitivities.ToList();
            Ae.Sort();
        }
        private void CalGlobalStrainEnergy()
        {
            foreach (var elem in Model.Elements)
                SumC += elem.C;
        }
    }
}
