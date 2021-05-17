using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ALFE
{
    public enum Solver
    {
        SimplicialLLT = 0,
        CholmodSimplicialLLT = 1,
        PARDISOSingle = 2,
        CG = 3,
        PARDISO = 4,
        CholmodSuperNodalLLT = 5,
        AMGCL = 6
    }
    public class FESystem
    {
        public Solver _Solver;
        /// <summary>
        /// Time cost in each step: 0 = Computing Ke, 1 = Initializing KG, 2 = Assembling KG, 3 = Solving
        /// </summary>
        public List<double> TimeCost = new List<double>(4);

        /// <summary>
        /// Finite element model
        /// </summary>
        public Model Model;

        /// <summary>
        /// Dimensions of the global stiffness matrix
        /// </summary>
        public int KG_Dim;

        /// <summary>
        /// The dimension of the problem
        /// </summary>
        public int Dim;

        /// <summary>
        /// If the system has been solved.
        /// </summary>
        public bool Solved = false;

        /// <summary>
        /// An index list of anchored nodes.
        /// </summary>
        private List<int> FixedID;

        /// <summary>
        /// Global stiffness matrix
        /// </summary>
        private CSRMatrix KG;

        /// <summary>
        /// Force vector
        /// </summary>
        private double[] F;

        /// <summary>
        /// Displacement vector
        /// </summary>
        private double[] X;

        public bool HardKill = false;

        public bool ParallelComputing = true;

        public List<DOF> ActiveDofs = new List<DOF>();
        /// <summary>
        /// Initialize the finite element system.
        /// </summary>
        /// <param name="model"> A finite element model</param>
        public FESystem(Model model, Solver solver = Solver.SimplicialLLT, bool parallel = true, bool hardKill = false)
        {
            Model = model;
            Dim = model.DOF;
            _Solver = solver;
            HardKill = hardKill;
            ParallelComputing = parallel;

            ApplySupports();

            GetConnectedElements();
            GetAdjacentNodes();
            SetDOFs();
            ComputeElementDOFs();
            AllocateKG();

            Stopwatch sw = new Stopwatch();

            sw.Start();
            ComputeAllKe();
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            InitialzeKG();
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);
        }

        /// <summary>
        /// Update the system
        /// </summary>
        public void Update()
        {
            F = new double[KG_Dim];
            X = new double[KG_Dim];

            double KeTime = TimeCost[0];
            double KGTime = TimeCost[1];
            TimeCost = new List<double>(3);
            TimeCost.Add(KeTime);
            TimeCost.Add(KGTime);
        }

        /// <summary>
        /// Assemble the force vector.
        /// </summary>
        private void AssembleF()
        {
            foreach (var item in Model.Loads)
            {
                F[Model.Nodes[item.NodeID].DofX.ActiveID] = item.ForceVector.X;
                F[Model.Nodes[item.NodeID].DofY.ActiveID] = item.ForceVector.Y;
                if (Dim == 3) F[Model.Nodes[item.NodeID].DofZ.ActiveID] = item.ForceVector.Z;
            }
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        private void AssembleKG(int P = 3)
        {
            KG.Clear();

            //foreach (var dof in ActiveDofs)
            //{
            //    foreach (var elemID in Model.Nodes[dof.NodeID].ElementID)
            //    {
            //        var elem = Model.Elements[elemID];

            //        for (int i = 0; i < dof.RowNNZ; i++)
            //        {
            //            int a = elem.DOF_ID[dof.ActiveID];
            //            int b = elem.DOF_ID[ActiveDofs[dof.PositionKG.Keys[i]].ActiveID];
            //            KG.Vals[dof.PositionKG[i] + i] += elem.Ke[a, b];
            //        }
            //    }
            //}
            DOF a = new DOF();
            DOF b = new DOF();
            int start = 0;
            foreach (var elem in Model.Elements)
            {
                //Console.WriteLine(elem.Ke);
                for (int i = 0; i < elem.DOF_ID.Count; i++)
                {
                    for (int j = 0; j < elem.DOF_ID.Count; j++)
                    {
                        a = ActiveDofs[elem.DOF_ID.Keys.ToArray()[i]];
                        b = ActiveDofs[elem.DOF_ID.Keys.ToArray()[j]];
                        start =a.PositionKG[b.ActiveID];
                        KG.Vals[start + b.ActiveID] += elem.Ke[elem.DOF_ID[a.ActiveID], elem.DOF_ID[b.ActiveID]];
                    }
                }
            }
            FEIO.WriteKG(KG, "E:\\ttKG.mtx");
            //    foreach (var elem in Model.Elements)
                //    {
                //        for (int i = 0; i < elem.Nodes.Count; i++)
                //        {
                //            Node ni = elem.Nodes[i];
                //            for (int j = 0; j < elem.Nodes.Count; j++)
                //            {
                //                Node nj = elem.Nodes[j];
                //                int idx1 = ni.PositionKG[nj.ActiveID];
                //                for (int m = 0; m < Dim; m++)
                //                {
                //                    for (int n = 0; n < Dim; n++)
                //                    {
                //                        if (HardKill == false)
                //                        {
                //                            if (elem.Exist == true)
                //                                KG.Vals[idx1 + ni.row_nnz * n + m] += elem.Ke[i * Dim + n, j * Dim + m];
                //                            else
                //                                KG.Vals[idx1 + ni.row_nnz * n + m] += elem.Ke[i * Dim + n, j * Dim + m] * Math.Pow(0.001, P);
                //                        }
                //                        else
                //                        {
                //                            if (elem.Exist == true)
                //                                KG.Vals[idx1 + ni.row_nnz * n + m] += elem.Ke[i * Dim + n, j * Dim + m];
                //                            else
                //                                KG.Vals[idx1 + ni.row_nnz * n + m] /= KG.Vals[idx1 + ni.row_nnz * n + m];
                //                        }

                //                    }
                //                }
                //            }
                //        }
                //    }
            }

        /// <summary>
        /// Initialize the system for solving
        /// </summary>
        public void Initialize()
        {
            Stopwatch sw = new Stopwatch();

            sw.Start();
            AssembleKG();
            AssembleF();
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);
        }
        /// <summary>
        /// Solve the finite element system. You can get the displacement vector after running this function.
        /// </summary>
        public void Solve()
        {
            //Stopwatch sw = new Stopwatch();

            //sw.Start();
            //Solved = SolveSystem((int)_Solver, KG.Rows, KG.Cols, KG.Vals, F, KG_Dim, KG.NNZ, X) == 1 ? true : false;
            //sw.Stop();
            //TimeCost.Add(sw.Elapsed.TotalMilliseconds);

            //int id = 0;
            //if (Solved)
            //{
            //    foreach (var item in Model.Nodes)
            //    {
            //        if (item.Active == true)
            //        {
            //            if (Dim == 2) item.Displacement = new Vector3D(X[id * Dim + 0], X[id * Dim + 1], 0.0);
            //            if (Dim == 3) item.Displacement = new Vector3D(X[id * Dim + 0], X[id * Dim + 1], X[id * Dim + 2]);
            //            id++;
            //        }
            //    }
            //}
            //else
            //{
            //    throw new Exception("Calculation failure.");
            //}
        }

        /// <summary>
        /// Get the global stiffness matrix.
        /// </summary>
        /// <returns> Return the global stiffness matrix.</returns>
        public CSRMatrix GetKG() { return KG; }

        /// <summary>
        /// Get the displacement vector.
        /// </summary>
        /// <returns> Return the displacement vector.</returns>
        public double[,] GetDisplacement()
        {
            double[,] displacement = new double[Model.Nodes.Count, Dim];
            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                displacement[i, 0] = Model.Nodes[i].Displacement.X;
                displacement[i, 1] = Model.Nodes[i].Displacement.Y;
                if (Dim == 3) displacement[i, 2] = Model.Nodes[i].Displacement.Z;
            }
            return displacement;
        }

        /// <summary>
        /// Initialize the global stiffness matrix
        /// </summary>
        private void InitialzeKG()
        {
            // create CSR indices
            int row_id = 0;
            int col_id = 0;
            foreach (DOF dof in ActiveDofs)
            {
                KG.Rows[dof.ActiveID] = row_id;

                SortedSet<int> sortedNeighbors = new SortedSet<int>(Model.Nodes[dof.NodeID].Neighbours);

                foreach (int neighbours in sortedNeighbors)
                {
                    if (!Model.Nodes[neighbours].DofX.isFixed)
                    {
                        KG.Cols[col_id] = Model.Nodes[neighbours].DofX.ActiveID;
                        dof.PositionKG.Add(Model.Nodes[neighbours].DofX.ActiveID, row_id);
                        col_id++;
                    }

                    if (!Model.Nodes[neighbours].DofY.isFixed)
                    {
                        KG.Cols[col_id] = Model.Nodes[neighbours].DofY.ActiveID;
                        dof.PositionKG.Add(Model.Nodes[neighbours].DofY.ActiveID, row_id);
                        col_id++;
                    }
                    if (Dim == 3)
                        if (!Model.Nodes[neighbours].DofZ.isFixed)
                        {
                            KG.Cols[col_id] = Model.Nodes[neighbours].DofZ.ActiveID;
                            dof.PositionKG.Add(Model.Nodes[neighbours].DofZ.ActiveID, row_id);
                            col_id++;
                        }
                }

                row_id += dof.RowNNZ;
            }
        }

        /// <summary>
        /// Get the neighbours of each node.
        /// </summary>
        private void GetAdjacentNodes()
        {
            if (ParallelComputing)
            {
                Parallel.ForEach(Model.Nodes, node =>
                {
                    foreach (var item in node.ElementID)
                        foreach (var neighbour in Model.Elements[item].Nodes)
                            //if(neighbour.ID != node.ID)
                                lock (node.Neighbours)
                                    node.Neighbours.Add(neighbour.ID);
                });
            }
            else
            {
                foreach (var node in Model.Nodes)
                    foreach (var item in node.ElementID)
                        foreach (var neighbour in Model.Elements[item].Nodes)
                            //if (neighbour.ID != node.ID)
                                node.Neighbours.Add(neighbour.ID);
            }
        }

        /// <summary>
        /// Get the connected elements of each node.
        /// </summary>
        private void GetConnectedElements()
        {
            // in each node make a list of elements to which it belongs
            foreach (var elem in Model.Elements)
                foreach (var node in elem.Nodes)
                    node.ElementID.Add(elem.ID);
        }

        private void ComputeElementDOFs()
        {
            foreach (var elem in Model.Elements)
            {
                int num = 0;
                foreach (var node in elem.Nodes)
                {
                    if (!node.DofX.isFixed)
                    {
                        elem.DOF_ID.Add(node.DofX.ActiveID, num);
                        num++;
                    }
                    
                    if (!node.DofY.isFixed)
                    {
                        elem.DOF_ID.Add(node.DofY.ActiveID, num);
                        num++;
                    }
                    if (Dim == 3 && !node.DofZ.isFixed)
                    {
                        elem.DOF_ID.Add(node.DofZ.ActiveID, num);
                        num++;
                    }
                }
            }
        }

        private void SetDOFs()
        {
            int id = 0;
            foreach (var nd in Model.Nodes)
            {
                if (!nd.DofX.isFixed)
                {
                    nd.DofX.SetActiveID(id);
                    ActiveDofs.Add(nd.DofX);
                    id++;
                }

                if (!nd.DofY.isFixed)
                {
                    nd.DofY.SetActiveID(id);
                    ActiveDofs.Add(nd.DofY);
                    id++;
                }

                if (nd.Dim == 3 && !nd.DofZ.isFixed)
                {
                    nd.DofZ.SetActiveID(id);
                    ActiveDofs.Add(nd.DofZ);
                    id++;
                }
            }
        }

        private void AllocateKG()
        {
            // count total number of non-zero items.
            int nnzCount = 0;
            foreach (var dof in ActiveDofs)
            {
                int num = 0;
                foreach (var neighbours in Model.Nodes[dof.NodeID].Neighbours)
                {
                    if (!Model.Nodes[neighbours].DofX.isFixed) num++;
                    if (!Model.Nodes[neighbours].DofY.isFixed) num++;
                    if (Dim == 3) if (!Model.Nodes[neighbours].DofZ.isFixed) num++;
                }

                dof.RowNNZ = num;
                nnzCount += num;
            }

            // allocate CSR
            KG = new CSRMatrix(ActiveDofs.Count, nnzCount + ActiveDofs.Count);
            F = new double[ActiveDofs.Count];
            X = new double[ActiveDofs.Count];
        }

        /// <summary>
        /// Apply the boundary conditions to nodes.
        /// </summary>
        /// <returns></returns>
        public void ApplySupports()
        {
            var nodes = Model.Nodes;
            var supports = Model.Supports;

            List<int> ids = new List<int>(supports.Count);
            for (int i = 0; i < supports.Count; i++)
            {
                int id = supports[i].NodeID;
                ids.Add(id);

                if (supports[i].FixedX)
                {
                    nodes[id].Displacement.X = 0.0;
                    nodes[id].DofX.SetFixed(true);
                }

                if (supports[i].FixedY)
                {
                    nodes[id].Displacement.Y = 0.0;
                    nodes[id].DofY.SetFixed(true);
                }

                if (Dim == 3 && supports[i].FixedZ)
                {
                    nodes[id].Displacement.Z = 0.0;
                    nodes[id].DofZ.SetFixed(true);
                }
            }
            FixedID = ids;
        }

        /// <summary>
        /// Compute all elementary stiffness matrices.
        /// </summary>
        private void ComputeAllKe()
        {
            if (Model.Elements[0].Type == ElementType.PixelElement || Model.Elements[0].Type == ElementType.VoxelElement)
            {
                var elem0 = Model.Elements[0];
                elem0.ComputeKe();
                var K = elem0.Ke;
                foreach (var elem in Model.Elements)
                {
                    elem.Ke = K;
                }
            }
            else
            {
                foreach (var item in Model.Elements)
                    item.ComputeKe();
            }
        }
        [DllImport("ALSolver.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = false)]
        private static extern int SolveSystem(int solver, int[] rows_offset, int[] cols, double[] vals, double[] F, int dim, int nnz, double[] X);


        public string SolvingInfo()
        {
            string info = "------------------- Time Cost -------------------";
            info += '\n';
            info += "Solver: " + _Solver.ToString();
            info += '\n';
            info += "Computing Ke: " + TimeCost[0].ToString() + " ms";
            info += '\n';
            info += "Initializing KG: " + TimeCost[1].ToString() + " ms";
            info += '\n';
            info += "Assembling KG: " + TimeCost[2].ToString() + " ms";
            info += '\n';
            info += "Solving: " + TimeCost[3].ToString() + " ms";
            info += '\n';

            return info;
        }

        public string MatrixInfo()
        {
            string info = "------------------- Matrix Info -------------------";
            info += '\n';
            info += "Rows: " + KG.N.ToString();
            info += '\n';
            info += "Cols: " + KG.N.ToString();
            info += '\n';
            info += "NNZ: " + KG.NNZ.ToString();
            info += '\n';

            return info;
        }

        public string DisplacementInfo()
        {
            string info = "------------------- Displacement Info -------------------";
            info += '\n';

            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                info += Model.Nodes[i].Displacement.X;
                info += '\t';
                info += Model.Nodes[i].Displacement.Y;
                info += '\n';
            }

            return info;
        }
    }
}
