using ALFE;
using MathNet.Numerics.LinearAlgebra.Single;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ALFE.TopOpt
{
    public class BESO
    {
        public FESystem System;
        public Model Model;
        public string Path;

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

        private Filter _Filter;

        /// <summary>
        /// The iterative history of the global strain energy
        /// </summary>
        private List<float> HistoryGSE = new List<float>();

        /// <summary>
        /// The iterative history of the volume
        /// </summary>
        private List<float> HistoryV = new List<float>();

        public BESO(string path, FESystem system, float rmin, float ert = 0.02f, int p=3,  float vf=0.5f, int maxIter=100)
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
            Path = path;
        }

        public void Initialize()
        {
            // Print basic info
            FEPrint.PrintPreprocessing(this);

            foreach (var elem in Model.Elements)
                elem.Xe = 1.0f;

            // Filtering
            Stopwatch sw = new Stopwatch();
            sw.Start();
            _Filter = new Filter(Model.Elements, FilterRadius, Dim);
            _Filter.PreFlt();
            sw.Stop();
            Console.WriteLine("Prefiltering: " + sw.Elapsed.TotalMilliseconds.ToString() + " ms");
            Console.WriteLine();
            
            FEIO.WriteValidElements(0, Path, Model.Elements);
        }
        public void Optimize()
        {
            float delta = 1.0f;
            int iter = 0;
            List<float> Ae_old = new List<float>();
            while (delta > 0.01f && iter < MaximumIteration)
            {
                // Run FEA
                System.Initialize();
                System.Solve();

                //FEPrint.PrintDisplacement(System);
                //FEIO.WriteKG(System.GetKG(), "E:\\KG.mtx");

                // Calculate sensitivities and global strain energy
                List<float> Ae = CalSensitivity();
                HistoryGSE.Add(CalGlobalStrainEnergy());

                // Process sensitivities
                Ae = FltAe(_Filter, Ae);
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
                FEIO.WriteValidElements(iter, Path, Model.Elements);

                System.Update();

                // Check convergence 
                if (iter >= 10)
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

            Console.WriteLine("------------------- GSE -------------------");
            int num = 0;
            foreach (var item in HistoryGSE)
            {
                Console.WriteLine(num.ToString() + '\t' + item.ToString());
                num++;
            }

            Console.WriteLine("Done");
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
                    elem.Exist = elem.Xe == 1.0f ? true : false;
                    sum += v;
                }
                if (sum - tv > 0.0f) lowest = th;
                else highest = th;
            }
        }
        private List<float> CalSensitivity()
        {
            float[] Sensitivities = new float[Model.Elements.Count];
            Parallel.ForEach(Model.Elements, elem =>
            {
                if (elem.Exist == true)
                {
                    elem.ComputeUe();

                    var Ke = elem.Ke;
                    var Ue = elem.Ue;
                    if (elem.Exist != true)
                        Ke.Multiply((float)Math.Pow(0.001, PenaltyExponent));

                    elem.C = 0.5f * Ue.TransposeThisAndMultiply(Ke).Multiply(Ue)[0, 0];

                    Sensitivities[elem.ID] = elem.C / elem.Xe;
                }
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
        private List<float> FltAe(Filter filter, List<float> Ae)
        {
            var raw = new float[Ae.Count];

            foreach (var elem in Model.Elements)
            {
                raw[elem.ID] = 0.0f;
                for (int i = 0; i < filter.FME[elem].Count; i++)
                    raw[elem.ID] += Ae[filter.FME[elem][i].ID] * filter.FMW[elem][i];
            }
            return raw.ToList();
        }
    }
}
