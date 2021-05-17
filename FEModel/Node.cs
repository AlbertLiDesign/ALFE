using System.Collections.Generic;

namespace ALFE
{
    public class Node
    {
        /// <summary>
        /// Nodal Position
        /// </summary>
        public Vector3D Position { get; set; }

        /// <summary>
        /// Nodal Displacement
        /// </summary>
        public Vector3D Displacement = new Vector3D();

        /// <summary>
        /// The neighbours of the nodes
        /// </summary>
        public HashSet<int> Neighbours = new HashSet<int>();

        public List<int> ElementID = new List<int>();

        public int ID;
        public bool hasID = false;

        public DOF DofX;
        public DOF DofY;
        public DOF DofZ;

        /// <summary>
        /// Degree of freedom
        /// </summary>
        public int Dim;

        #region Constructor
        public Node(Node node)
        {
            Position = node.Position;
            ID = node.ID;
            hasID = true;
            Dim = node.Dim;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
            if (Dim == 3) DofZ = new DOF(false, ID);
        }
        public Node(Node node, int index)
        {
            Position = node.Position;
            ID = index;
            hasID = true;
            Dim = node.Dim;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
            if(Dim == 3) DofZ = new DOF(false, ID);
        }
        public Node(double x, double y)
        {
            Position = new Vector3D((double)x, (double)y, 0.0);
            Dim = 2;
        }
        public Node(double x, double y, int index)
        {
            Position = new Vector3D(x, y, 0.0);
            ID = index;
            hasID = true;
            Dim = 2;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
        }
        public Node(Vector2D position, int index)
        {
            Position = new Vector3D(position.X, position.Y, 0.0);
            ID = index;
            hasID = true;
            Dim = 2;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
        }
        public Node(double x, double y, double z)
        {
            Position = new Vector3D((double)x, (double)y, (double)z);
            Dim = 3;
        }
        public Node(int dim, double x, double y, double z)
        {
            Position = new Vector3D((double)x, (double)y, (double)z);
            Dim = dim;
        }

        public Node(double x, double y, double z, int index)
        {
            Position = new Vector3D(x, y, z);
            ID = index;
            hasID = true;
            Dim = 3;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
            DofZ = new DOF(false, ID);
        }
        public Node(int dim, double x, double y, double z, int index)
        {
            Position = new Vector3D(x, y, z);
            ID = index;
            hasID = true;
            Dim = dim;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
            DofZ = new DOF(false, ID);
        }
        public Node(Vector3D position, int index)
        {
            Position = position;
            ID = index;
            hasID = true;
            Dim = 3;
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
            DofZ = new DOF(false, ID);
        }
        #endregion
        public void SetID(int index)
        {
            ID = index;
            hasID = true;
            InitDOF();
        }

        public void InitDOF()
        {
            DofX = new DOF(false, ID);
            DofY = new DOF(false, ID);
            if (Dim == 3) DofZ = new DOF(false, ID);
        }
    }
}
