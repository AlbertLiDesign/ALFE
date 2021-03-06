﻿//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using MathNet.Numerics.LinearAlgebra;

//namespace ALFE.TopOpt
//{
//    class BESO_AMR
//    {
//        public string solvingInfo;
//        public FESystem System;
//        public Model Model;
//        public string Path;

//        /// <summary>
//        /// The isovalue for extracting isosurface.
//        /// </summary>
//        public double isovalue = 0.0;

//        /// <summary>
//        /// Volume fraction
//        /// </summary>
//        public double VolumeFraction;

//        /// <summary>
//        /// Penalty exponent
//        /// </summary>
//        public double PenaltyExponent;

//        /// <summary>
//        /// Evolution rate
//        /// </summary>
//        public double EvolutionRate;

//        /// <summary>
//        /// The maximum layer
//        /// </summary>
//        public int MaxLevel { get; set; }

//        /// <summary>
//        /// The maximum iteration
//        /// </summary>
//        public int MaximumIteration;

//        public List<double> Sensitivities = new List<double>();

//        /// <summary>
//        /// Dimension
//        /// </summary>
//        public int KG_Dim;

//        /// <summary>
//        /// The iterative history of the global compliance
//        /// </summary>
//        private List<double> HistoryC = new List<double>();

//        /// <summary>
//        /// The iterative history of the volume
//        /// </summary>
//        private List<double> HistoryV = new List<double>();

//        /// <summary>
//        /// The initial point
//        /// </summary>
//        public Vector3D InitialPoint { get; set; }

//        /// <summary>
//        /// The root node
//        /// </summary>
//        public QuadTree RootQuadTree { get; set; }

//        /// <summary>
//        /// The interior leafs
//        /// </summary>
//        public List<QuadTree> InteriorNodes { get; set; }

//        /// <summary>
//        /// The boundary leafs
//        /// </summary>
//        public List<QuadTree> BoundaryNodes { get; set; }
        
//        public BESO_AMR() { }
//        public BESO_AMR(string path, FESystem system, double ert = 0.02f, double p = 3.0, double vf = 0.5, int maxIter = 100, int maxLevel = 3, Solver solver = 0)
//        {
//            System = system;
//            Model = system.Model;
//            VolumeFraction = vf;
//            PenaltyExponent = p;
//            EvolutionRate = ert;
//            MaxLevel = maxLevel;
//            MaximumIteration = maxIter;
//            KG_Dim = system.Model.KG_Dim;
//            Path = path;
//            System._Solver = solver;
//        }

//        /// <summary>
//        ///  To refine the nodes
//        /// </summary>
//        public void Subdivision()
//        {
//            //// The edge length of the child nodes
//            //var childEdgeLength = EdgeLength / 4.0;

//            //// The layer level of the child nodes
//            //var childLevel = LayerLevel + 1;

//            //int activeNum = 0;
//            //for (int i = 0; i < 4; i++)
//            //{
//            //    if (Nodes[i].Active)
//            //    {
//            //        var ElemIds = Nodes[i].ElementID;
//            //        foreach (var elemId in ElemIds)
//            //        {

//            //        }
//            //        activeNum++;
//            //    }
//            //}
//            //var v0 =

//            //    // To add 4 new child nodes, the edge length of the child node would be EdgeLength / 2.0
//            //    Branches.Add(new QuadTree(Position.X - childEdgeLength, Position.Y - childEdgeLength, childEdgeLength * 2, 0.0, childLevel, Material));
//            //Branches.Add(new QuadTree(Position.X + childEdgeLength, Position.Y - childEdgeLength, childEdgeLength * 2, 0.0, childLevel, Material));
//            //Branches.Add(new QuadTree(Position.X + childEdgeLength, Position.Y + childEdgeLength, childEdgeLength * 2, 0.0, childLevel, Material));
//            //Branches.Add(new QuadTree(Position.X - childEdgeLength, Position.Y + childEdgeLength, childEdgeLength * 2, 0.0, childLevel, Material));
//        }


//        public void Initialize()
//        {
//            // Write basic info
//            solvingInfo = PreprocessingInfo();

//            foreach (var elem in Model.Elements)
//                elem.Xe = 1.0;

//            // Filtering
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//            _Filter = new Filter(Model.Elements, FilterRadius, KG_Dim);
//            _Filter.PreFlt();
//            sw.Stop();

//            solvingInfo += "Prefiltering: " + sw.Elapsed.TotalMilliseconds.ToString() + " ms";
//            solvingInfo += '\n';

//            FEIO.WriteInvalidElements(0, Path, Model.Elements);
//        }
//        public void Optimize()
//        {
//            double delta = 1.0;
//            int iter = 0;
//            List<double> Ae_old = new List<double>();
//            while (delta > 0.001 && iter < MaximumIteration)
//            {
//                List<double> timeCost = new List<double>();
//                Stopwatch sw = new Stopwatch();
//                sw.Start();
//                // Run FEA
//                System.Initialize();
//                sw.Stop();
//                timeCost.Add(sw.Elapsed.TotalMilliseconds);

//                Console.WriteLine("Prepare to solve the system");
//                sw.Restart();
//                //FEIO.WriteKG(System.GetKG(), "E:\\KG" + iter.ToString() + ".mtx");
//                System.Solve();
//                sw.Stop();
//                timeCost.Add(sw.Elapsed.TotalMilliseconds);

//                //FEPrint.PrintDisplacement(System);


//                // Calculate sensitivities and global compliance
//                sw.Restart();
//                List<double> Ae = CalSensitivities();
//                HistoryC.Add(CalGlobalCompliance());
//                sw.Stop();
//                timeCost.Add(sw.Elapsed.TotalMilliseconds);


//                sw.Restart();
//                // Process sensitivities
//                Ae = FltAe(_Filter, Ae);
//                if (iter > 0)
//                    for (int i = 0; i < Ae.Count; i++)
//                        Ae[i] = (Ae[i] + Ae_old[i]) * 0.5;

//                // Record the sensitiveies in each step
//                var raw = new double[Ae.Count];
//                Ae.CopyTo(raw);
//                Ae_old = raw.ToList();
//                sw.Stop();
//                timeCost.Add(sw.Elapsed.TotalMilliseconds);

//                // Run BESO
//                sw.Restart();
//                double sum = 0.0;
//                foreach (var elem in Model.Elements)
//                    sum += elem.Xe;

//                HistoryV.Add(sum / Model.Elements.Count);
//                double curV = Math.Max(VolumeFraction, HistoryV.Last() * (1.0 - EvolutionRate));
//                BESO_Core(curV, Ae);
//                sw.Stop();
//                timeCost.Add(sw.Elapsed.TotalMilliseconds);

//                sw.Restart();
//                iter += 1;
//                FEIO.WriteInvalidElements(iter, Path, Model.Elements);

//                Sensitivities = Ae_old;
//                System.Update();

//                // Check convergence 
//                if (iter >= 10)
//                {
//                    var newV = 0.0;
//                    var lastV = 0.0;
//                    for (int i = 1; i < 6; i++)
//                    {
//                        newV += HistoryC[HistoryC.Count - i];
//                        lastV += HistoryC[HistoryC.Count - 5 - i];
//                    }
//                    delta = Math.Abs((newV - lastV) / lastV);
//                }
//                sw.Stop();
//                timeCost.Add(sw.Elapsed.TotalMilliseconds);

//                solvingInfo += BESOInfo(iter - 1, HistoryC.Last(), HistoryV.Last(), timeCost);
//                WritePerformanceReport();
//            }

//            FEIO.WriteVertSensitivities(Path, ComputeVertSensitivities(Sensitivities), Model);
//        }
//        private void BESO_Core(double curV, List<double> Ae)
//        {
//            double lowest = Ae.Min();
//            double highest = Ae.Max();
//            double th = 0.0;
//            double tv = curV * Model.Elements.Count;

//            while (((highest - lowest) / highest) > 1.0e-5f)
//            {
//                th = (highest + lowest) * 0.5;
//                double sum = 0.0;
//                foreach (var elem in Model.Elements)
//                {
//                    var v = Ae[elem.ID] > th ? 1.0 : 0.001f;
//                    elem.Xe = v;
//                    elem.Exist = elem.Xe == 1.0 ? true : false;
//                    sum += v;
//                }
//                if (sum - tv > 0.0) lowest = th;
//                else highest = th;
//            }
//            isovalue = th;
//        }
//        private List<double> CalSensitivities()
//        {
//            double[] values = new double[Model.Elements.Count];
//            if (System.ParallelComputing)
//            {
//                Parallel.ForEach(Model.Elements, elem =>
//                {
//                    elem.ComputeUe();

//                    Matrix<double> Ke = null;
//                    if (elem.Exist == true)
//                        Ke = elem.Ke;
//                    else
//                        Ke = (Matrix)elem.Ke.Multiply((double)Math.Pow(0.001, (double)PenaltyExponent));

//                    var Ue = elem.Ue;
//                    if (elem.Exist != true)
//                        Ke.Multiply((double)Math.Pow(0.001, PenaltyExponent));

//                    elem.C = 0.5 * Ue.TransposeThisAndMultiply(Ke).Multiply(Ue)[0, 0];

//                    values[elem.ID] = elem.C / elem.Xe;
//                });
//            }
//            else
//            {
//                foreach (var elem in Model.Elements)
//                {
//                    elem.ComputeUe();

//                    Matrix<double> Ke = null;
//                    if (elem.Exist == true)
//                        Ke = elem.Ke;
//                    else
//                        Ke = (Matrix)elem.Ke.Multiply((double)Math.Pow(0.001, (double)PenaltyExponent));

//                    var Ue = elem.Ue;
//                    if (elem.Exist != true)
//                        Ke.Multiply((double)Math.Pow(0.001, PenaltyExponent));

//                    elem.C = 0.5 * Ue.TransposeThisAndMultiply(Ke).Multiply(Ue)[0, 0];

//                    values[elem.ID] = elem.C / elem.Xe;
//                }
//            }
//            return values.ToList();
//        }
//        private double CalGlobalCompliance()
//        {
//            double compliance = 0.0;
//            foreach (var elem in Model.Elements)
//                compliance += elem.C;
//            return compliance;
//        }
//        private List<double> FltAe(Filter filter, List<double> Ae)
//        {
//            var raw = new double[Ae.Count];

//            foreach (var elem in Model.Elements)
//            {
//                raw[elem.ID] = 0.0;
//                for (int i = 0; i < filter.FME[elem].Count; i++)
//                    raw[elem.ID] += Ae[filter.FME[elem][i].ID] * filter.FMW[elem][i];
//            }
//            return raw.ToList();
//        }

//        public List<double> ComputeVertSensitivities(List<double> elemSensitivities)
//        {
//            var Vert_Value = new List<double>();
//            var nodes = Model.Nodes;
//            foreach (var item in nodes)
//            {
//                double sensitivity = 0.0;
//                for (int i = 0; i < item.ElementID.Count; i++)
//                {
//                    sensitivity += elemSensitivities[item.ElementID[i]] * 0.125;
//                }
//                Vert_Value.Add(sensitivity);
//            }

//            // 映射到0-1
//            double max = Vert_Value.Max();
//            double min = Vert_Value.Min();
//            for (int i = 0; i < Vert_Value.Count; i++)
//            {
//                Vert_Value[i] = (Vert_Value[i] - min) / (max - min);
//            }
//            return Vert_Value;
//        }

//        public string PreprocessingInfo()
//        {
//            string info = "Project Path: " + Path.ToString();
//            info += '\n';
//            info += Model.ModelInfo();
//            info += System.MatrixInfo();

//            info += "------------------- Time Cost -------------------";
//            info += '\n';
//            info += "Computing Ke: " + System.TimeCost[0].ToString() + " ms";
//            info += '\n';
//            info += "Initializing KG: " + System.TimeCost[1].ToString() + " ms";
//            info += '\n';

//            return info;
//        }

//        public string BESOInfo(int iter, double gse, double vf, List<double> timeCost)
//        {
//            string info = "\n";
//            info += "################### Step: " + iter.ToString() + " #####################";
//            info += '\n';
//            info += "Compliance: " + gse.ToString();
//            info += '\n';
//            info += "Volume: " + vf.ToString();
//            info += '\n';

//            info += "------------------- Time Cost -------------------";
//            info += '\n';
//            info += "Assembling KG: " + timeCost[0].ToString() + " ms";
//            info += '\n';
//            info += "Solving: " + timeCost[1].ToString() + " ms";
//            info += '\n';
//            info += "Computing Sensitivity: " + timeCost[2].ToString() + " ms";
//            info += '\n';
//            info += "Fltering Sensitivity: " + timeCost[3].ToString() + " ms";
//            info += '\n';
//            info += "Marking Elements: " + timeCost[4].ToString() + " ms";
//            info += '\n';
//            info += "Checking Convergence: " + timeCost[5].ToString() + " ms";
//            info += '\n';

//            return info;
//        }

//        public void WritePerformanceReport()
//        {
//            string output = Path + "\\report.txt";
//            StreamWriter sw = new StreamWriter(output);

//            sw.Write(solvingInfo);
//            sw.Flush();
//            sw.Close();
//            sw.Dispose();
//        }
//    }
//}
//}
