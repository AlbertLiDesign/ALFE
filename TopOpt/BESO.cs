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
        /// Filter Radius
        /// </summary>
        public float FilterRadius;

        /// <summary>
        /// .Volume fraction
        /// </summary>
        public float VolumeFraction;     

        /// <summary>
        /// Penalty exponent
        /// </summary>
        public int PenaltyExponent;

        /// <summary>
        /// Evolution rate
        /// </summary>
        public float EvolutionRate;

        /// <summary>
        /// The maximum iteration
        /// </summary>
        public int MaximumIteration;

        /// <summary>
        /// Dimension
        /// </summary>
        public int Dim;

        /// <summary>
        /// The iterative history of the global strain energy
        /// </summary>
        private List<float> HistoryGSE = new List<float>();

        /// <summary>
        /// The iterative history of the volume
        /// </summary>
        private List<float> HistoryV = new List<float>();

        public BESO(FESystem system, float rmin, float ert = 0.02f, int p=3,  float vf=0.5f, int maxIter=100)
        {
            if (rmin <= 0.0f)
                throw new Exception("Rmin must be large than 0.");
            if (!(vf> 0.0f && vf< 1.0f))
                throw new Exception("Vt must be large than 0 and be less than 1.");

            System = system;
            Model = system.Model;
            VolumeFraction = vf;
            PenaltyExponent = p;
            EvolutionRate = ert;
            MaximumIteration = maxIter;
            FilterRadius = rmin;
            Dim = system.Model.DOF;
        }

        public void Optimize(string path)
        {

            FEPrint.PrintPreprocessing(this);

            foreach (var elem in Model.Elements)
                elem.Xe = 1.0f;

            Filter filter = new Filter(Model.Elements, FilterRadius, Dim);
            filter.PreFlt();

            float delta = 1.0f;
            int iter = 0;

            List<float> Ae_old = new List<float>();
            FEIO.WriteValidElements(iter, path, Model.Elements);

            while (delta > 0.01f && iter < MaximumIteration)
            {
                // Run FEA
                System.Initialize();
                System.Solve();

                FEIO.WriteKG(System.GetKG(), "E:\\KG.mtx");

                // Calculate sensitivities and global strain energy
                List<float> Ae = CalSensitivity();
                HistoryGSE.Add(CalGlobalStrainEnergy());

                // Process sensitivities
                FltAe(filter, ref Ae);
                if (iter > 0)
                    for (int i = 0; i < Ae.Count; i++)
                        Ae[i] = (Ae[i] + Ae_old[i]) * 0.5f;

                // Record the sensitiveies in each step
                var raw = new float[Ae.Count];
                Ae.CopyTo(raw);
                Ae_old = raw.ToList();

                // Run BESO
                float sum = 0.0f;
                foreach (var elem in Model.Elements)
                    sum += elem.Xe;

                HistoryV.Add(sum / Model.Elements.Count);
                float curV = Math.Max(VolumeFraction, HistoryV.Last() * (1.0f - EvolutionRate));
                MarkElements(curV, Ae);
                FEPrint.PrintBESOInfo(this, iter, HistoryGSE.Last(), HistoryV.Last());
                
                iter += 1;
                FEIO.WriteValidElements(iter, path, Model.Elements);

                System.Update();

                // Check convergence 
                if (iter > 10)
                {
                    var newV = 0.0f;
                    var lastV = 0.0f;
                    for (int i = 1; i < 6; i++)
                    {
                        newV += HistoryGSE[HistoryGSE.Count - i];
                        lastV += HistoryGSE[HistoryGSE.Count - 5 - i];
                    }
                    delta = Math.Abs((newV - lastV) / lastV);
                }
            }
        }


        private void MarkElements(float curV, List<float> Ae)
        {
            float lowest = Ae.Min();
            float highest = Ae.Max();

            float tv = curV * Model.Elements.Count;

            while (((highest - lowest) / highest) > 1.0e-5f)
            {
                float th = (highest + lowest) * 0.5f;
                float sum = 0.0f;
                foreach (var elem in Model.Elements)
                {
                    var v = Ae[elem.ID] > th ? 1.0f : 0.001f;
                    elem.Xe = v;
                    sum += v;
                }
                if (sum - tv > 0.0f) lowest = th;
                else highest = th;
            }

            foreach (var elem in Model.Elements)
                elem.Exist = elem.Xe == 1.0f ? true : false;
        }
        private List<float> CalSensitivity()
        {
            float[] Sensitivities = new float[Model.Elements.Count];
            Parallel.ForEach(Model.Elements, elem =>
            {
               elem.ComputeUe();

               var Ke = elem.Ke;
               var Ue = elem.Ue;
               if (elem.Exist != true)
                   Ke.Multiply((float)Math.Pow(0.001, PenaltyExponent));

               elem.C = Ue.TransposeThisAndMultiply(Ke).Multiply(Ue)[0, 0];

                Sensitivities[elem.ID] = elem.C / elem.Xe;
            });

            return Sensitivities.ToList();
        }
        private float CalGlobalStrainEnergy()
        {
            float GlobalStrainEnergy = 0.0f;
            foreach (var elem in Model.Elements)
                GlobalStrainEnergy += elem.C;
            return GlobalStrainEnergy;
        }
        private void FltAe(Filter filter, ref List<float> Ae)
        {
            var raw = new float[Ae.Count];
            Ae.CopyTo(raw);

            foreach (var elem in Model.Elements)
            {
                Ae[elem.ID] = 0.0f;
                for (int i = 0; i < filter.FME[elem].Count; i++)
                    Ae[elem.ID] += raw[filter.FME[elem][i].ID] * filter.FMW[elem][i];
            }
        }
    }
}
