﻿using ALFE.FEModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ALFE.FESystem
{
    public class System2D
    {
        /// <summary>
        /// Time cost in each step: 0 = Computing Ke, 1 = Assembling KG, 2 = Solving
        /// </summary>
        public List<double> TimeCost = new List<double>(3);

        public List<Node2D> ActiveNodes = new List<Node2D>();

        /// <summary>
        /// Finite element model
        /// </summary>
        public Model2D Model;

        /// <summary>
        /// Dimensions of the global stiffness matrix
        /// </summary>
        public int Dim;

        /// <summary>
        /// Degree of freedom
        /// </summary>
        public int DOF = 2;

        /// <summary>
        /// If all elements are the same, It should be set True for reducing workload.
        /// </summary>
        public bool Unify = false;

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
        private float[] F;

        /// <summary>
        /// Displacement vector
        /// </summary>
        private float[] X;

        /// <summary>
        /// Initialize the finite element system.
        /// </summary>
        /// <param name="model"> A finite element model</param>
        public System2D(Model2D model, bool unify = false)
        {
            Model = model;
            Unify = unify;

            ApplySupports2D();
            Dim = (Model.Nodes.Count - FixedID.Count) * DOF;

            F = new float[Dim * DOF];
            X = new float[Dim * DOF];
        }

        /// <summary>
        /// Assemble the force vector.
        /// </summary>
        private void AssembleF()
        {
            for (int i = 0; i < Model.Loads.Count; i++)
            {
                F[Model.Loads[i].NodeID * DOF + 0] = Model.Loads[i].Load.X;
                F[Model.Loads[i].NodeID * DOF + 1] = Model.Loads[i].Load.Y;
            }
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        private void AssembleKG()
        {
            InitialzeKG();
            KG.Clear();
            float[] v = KG.Vals;

            foreach (Element elem in Model.Elements)
            {
                float[,] K = elem.Ke;
                for (int i = 0; i < 4; i++)
                    for (int j = 0; j < 4; j++)
                    {
                        Node2D ni = Model.Nodes[elem.NodeID[i]];
                        Node2D nj = Model.Nodes[elem.NodeID[j]];
                        if (ni.Active)
                        {
                            if (nj.Active)
                            {
                                // write the corresponding 2x2 fragment to CSR
                                int idx1 = ni.Position_KG[nj.ActiveID]; // there is a room for optimization here
                                for (int m = 0; m < 2; m++) for (int n = 0; n < 2; n++) 
                                        v[idx1 + ni.row_nnz * n + m] += K[i * 2 + n, j * 2 + m];
                            }
                        }
                    }
            }
        }

        /// <summary>
        /// Assemble the global stiffness matrix
        /// </summary>
        /// <param name="Ke">Input an elementary stiffness matrix.</param>
        private void AssembleKG(float[,] Ke)
        {
            InitialzeKG();
            KG.Clear();

            Parallel.For(0, Model.Elements.Count, e =>
            {
                var elem = Model.Elements[e];
                for (int i = 0; i < 4; i++)
                {
                    Node2D ni = Model.Nodes[elem.NodeID[i]];
                    if (ni.Active)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            Node2D nj = Model.Nodes[elem.NodeID[j]];

                            if (nj.Active && ni.Neighbours.Contains(nj.ActiveID))
                            {
                                // write the corresponding 2x2 fragment to CSR
                                int idx1 = ni.Position_KG[nj.ActiveID]; // there is a room for optimization here
                                for (int m = 0; m < 2; m++) for (int n = 0; n < 2; n++)
                                        KG.Vals[idx1 + ni.row_nnz * n + m] += Ke[i * 2 + n, j * 2 + m];
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Solve the finite element system. You can get the displacement vector after running this function.
        /// </summary>
        public void Solve()
        {
            Stopwatch sw = new Stopwatch();

            if (Unify == true)
            {
                sw.Start();
                var Ke = Model.ComputeUniformK();
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();
                AssembleKG(Ke);
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);
            }
            else
            {
                sw.Start();
                ComputeAllKe();
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);

                sw.Restart();
                AssembleKG();
                sw.Stop();
                TimeCost.Add(sw.Elapsed.TotalMilliseconds);
            }

            AssembleF();

            sw.Restart();
            var KG_COO = KG.ToCOO();
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);

            sw.Restart();
            SolveFE(KG_COO.RowArray, KG_COO.ColArray, KG_COO.ValueArray, F, Dim, DOF, KG_COO.NNZ, X);
            sw.Stop();
            TimeCost.Add(sw.Elapsed.TotalMilliseconds);

            int id = 0;

            foreach (var item in Model.Nodes)
            {
                if (item.Active == true)
                {
                    item.Displacement = new Vector2D(X[id * DOF + 0], X[id * DOF + 1]);
                    id++;
                }
            }
        }

        /// <summary>
        /// Get the global stiffness matrix.
        /// </summary>
        /// <returns> Return the global stiffness matrix.</returns>
        public CSRMatrix getKG()
        {
            return KG;
        }

        /// <summary>
        /// Get the displacement vector.
        /// </summary>
        /// <returns> Return the displacement vector.</returns>
        public double[,] getDisplacement()
        {
            double[,] displacement = new double[Dim, DOF];
            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                displacement[i, 0] = Model.Nodes[i].Displacement.X;
                displacement[i, 1] = Model.Nodes[i].Displacement.Y;
            }
            return displacement;
        }

        /// <summary>
        /// Print the time cost in each part.
        /// </summary>
        public void PrintTime()
        {
            FEPrint.PrintTimeCost(TimeCost);
        }

        /// <summary>
        /// Get the neighbours of each node.
        /// </summary>
        private void GetNeighbours()
        {
            for (int i = 0; i < Model.Nodes.Count; i++)
            {
                foreach (var item in Model.Nodes[i].ElementID)
                    foreach (var neighbour in Model.Elements[item].NodeID)
                        if (Model.Nodes[neighbour].Active)
                            if ((Model.Nodes[i].Position - Model.Nodes[neighbour].Position).Length < 1.4)
                                Model.Nodes[i].Neighbours.Add(Model.Nodes[neighbour].ActiveID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitialzeKG()
        {
            // list non-anchored nodes and give them sequential ids
            ActiveNodes = Model.Nodes.FindAll(nd => nd.Active);
            int id = 0;
            foreach (Node nd in ActiveNodes) { nd.ActiveID = id++; }

            // in each node make a list of elements to which it belongs
            for (int i = 0; i < Model.Elements.Count; i++)
            {
                foreach (int nd in Model.Elements[i].NodeID)
                    Model.Nodes[nd].ElementID.Add(i);
            }

            GetNeighbours();

            // count total number of neighbours in all nodes 
            int count = 0;
            foreach (Node nd in ActiveNodes) count += nd.Neighbours.Count;

            // allocate CSR
            // each neighbor contributes 2 rows and 2 columns to CSR matrix, so NNZ = count * 4
            // the size of the matrix is (number of active nodes)*(2 coordinates)
            KG = new CSRMatrix(ActiveNodes.Count * 2, count * 4);

            // 3) create CSR indices
            count = 0;
            foreach (Node nd in ActiveNodes)
            {
                int row_nnz = nd.ComputePositionInKG(count, KG.Cols);
                KG.Rows[nd.ActiveID * 2 + 0] = count;
                KG.Rows[nd.ActiveID * 2 + 1] = count + row_nnz;
                count += row_nnz * 2;
            }
        }

        /// <summary>
        /// Apply the boundary conditions to nodes.
        /// </summary>
        /// <returns></returns>
        private void ApplySupports2D()
        {
            var nodes = Model.Nodes;
            var supports = Model.Supports;

            List<int> ids = new List<int>(supports.Count);
            for (int i = 0; i < supports.Count; i++)
            {
                int id = supports[i].NodeID;
                ids.Add(id);
                nodes[id].Active = false;
                if (supports[i].Type == SupportType.Fixed)
                {
                    nodes[id].Displacement.X = 0.0f;
                    nodes[id].Displacement.Y = 0.0f;
                }
            }
            FixedID = ids;
        }

        /// <summary>
        /// Compute all elementary stiffness matrices.
        /// </summary>
        private void ComputeAllKe()
        {
            Parallel.For(0, Model.Elements.Count, i =>
            {
                Model.Elements[i].ComputeK();
            });
        }

        [DllImport("ALSolver.dll", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true, SetLastError = false)]
        private static extern void SolveFE(int[] rowA, int[] colA, float[] valA, float[] F, int dim, int dof, int nnzA, float[] X);

    }
}