﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Load
    {
        /// <summary>
        /// The index of the node, which has been applied a load.
        /// </summary>
        public int NodeID { get; set; }

        public Vector3D ForceVector { get; set; }

        public int Dof { get; set; }

        #region Constructor
        public Load() { }
        public Load(int node, Vector2D load)
        {
            NodeID = node;
            ForceVector = new Vector3D(load.X, load.Y, 0.0f);
            Dof = 2;
        }
        public Load(int node, float x, float y)
        {
            NodeID = node;
            ForceVector = new Vector3D(x, y, 0.0f);
            Dof = 2;
        }
        public Load(int node, double x, double y)
        {
            NodeID = node;
            ForceVector = new Vector3D((float)x, (float)y, 0.0f);
            Dof = 2;
        }
        public Load(int node, Vector3D load)
        {
            NodeID = node;
            ForceVector = new Vector3D(load.X, load.Y, load.Z);
            Dof = 3;
        }
        public Load(int node, float x, float y, float z)
        {
            NodeID = node;
            ForceVector = new Vector3D(x, y, z);
            Dof = 3;
        }
        public Load(int node, double x, double y, double z)
        {
            NodeID = node;
            ForceVector = new Vector3D((float)x, (float)y, (float)z);
            Dof = 3;
        }
        #endregion
    }
}