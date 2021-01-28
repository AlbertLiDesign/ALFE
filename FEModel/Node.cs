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

        /// <summary>
        /// Whether the node has been applied a boundary condition
        /// </summary>
        public bool Active = true;

        public List<int> ElementID = new List<int>();

        public int ID;
        public bool hasID = false;

        /// <summary>
        /// If the node is active, it will get an ID.
        /// </summary>
        public int ActiveID;

        public int row_nnz;
        public SortedList<int, int> PositionKG;

        /// <summary>
        /// Degree of freedom
        /// </summary>
        public int DOF;

        #region Constructor
        public Node(Node node)
        {
            Position = node.Position;
            ID = node.ID;
            hasID = true;
            DOF = node.DOF;
        }
        public Node(Node node, int index)
        {
            Position = node.Position;
            ID = index;
            hasID = true;
            DOF = node.DOF;
        }
        public Node(double x, double y)
        {
            Position = new Vector3D((double)x, (double)y, 0.0);
            DOF = 2;
        }
        public Node(double x, double y, bool active)
        {
            Position = new Vector3D(x, y, 0.0);
            Active = active;
            DOF = 2;
        }
        public Node(double x, double y, int index)
        {
            Position = new Vector3D(x, y, 0.0);
            ID = index;
            hasID = true;
            DOF = 2;
        }
        public Node(Vector2D position, int index)
        {
            Position = new Vector3D(position.X, position.Y, 0.0);
            ID = index;
            hasID = true;
            DOF = 2;
        }
        public Node(Vector2D position, int index, bool active)
        {
            Position = new Vector3D(position.X, position.Y, 0.0);
            ID = index;
            Active = active;
            hasID = true;
            DOF = 2;
        }
        public Node(double x, double y, double z)
        {
            Position = new Vector3D((double)x, (double)y, (double)z);
            DOF = 3;
        }
        public Node(int dof, double x, double y, double z)
        {
            Position = new Vector3D((double)x, (double)y, (double)z);
            DOF = dof;
        }

        public Node(double x, double y, double z, bool active)
        {
            Position = new Vector3D(x, y, z);
            Active = active;
            DOF = 3;
        }
        public Node(double x, double y, double z, int index)
        {
            Position = new Vector3D(x, y, z);
            ID = index;
            hasID = true;
            DOF = 3;
        }
        public Node(int dof, double x, double y, double z, int index)
        {
            Position = new Vector3D(x, y, z);
            ID = index;
            hasID = true;
            DOF = dof;
        }
        public Node(Vector3D position, int index)
        {
            Position = position;
            ID = index;
            hasID = true;
            DOF = 3;
        }
        public Node(Vector3D position, int index, bool active)
        {
            Position = position;
            ID = index;
            Active = active;
            hasID = true;
            DOF = 3;
        }
        #endregion
        public void SetID(int index)
        {
            ID = index;
            hasID = true;
        }

        /// <summary>
        /// This function is used in the process of constructing the structure of CSR.
        /// It identifies the locations in the matrix where non-zero values will be present.
        /// This function is executed for each node whose displacement is variable (non-anchored node),
        /// in sequential order starting from 0.
        /// Each execution of CreateCSRIndices adds 2 row entries to CSR, corresponding to (x,y).
        /// In each row, every non-zero entry corresponds to a neighbor of current node. 
        /// The non-zero entries in each row also come in (x,y).
        /// </summary>
        /// <param name="startIndex">number of non-zero elements in the CSR that have been already initialized</param>
        /// <param name="cols">column structure of CSR that is being created</param>
        /// <returns></returns>
        public int ComputePositionInKG(int startIndex, int[] cols)
        {
            // number of non-zero entries (nnz) in this row is (number of neighbors)*(2 coordinates)
            // we save this value, because it will be used to look up indices in CSR.vals[] at assembly stage
            row_nnz = Neighbours.Count * DOF;

            // In each row of the CSR matrix, non-zero values are stored left-to-write,
            // therefore we transfer neighboring nodes from HashSet to SortedList (sort by altId before inserting into CSR).
            // Creating SortedSet from HashSet should speed up insertions into SortedList (not benchmarked)
            SortedSet<int> sortedNeighbors = new SortedSet<int>(Neighbours);

            // Position_KG stands for "Position in the CSR matrix value array".
            // In Position_KG, "key" is the altId of the neighbor node (self included in neighbor list), and
            // "value" is the index in CSR.vals[] where the stiffness value will go.
            // If idx is such index, then the values will also go to (idx+1) and (idx+2), but
            // we do not store (idx+1) and (idx+2) explicitly in Position_KG - only idx is stored.
            // The stiffness values will also be written to (idx + row_nnz), (idx + 2*row_nnz), (idx+row_nnz+1),...,(idx+2*row_nnz+1).
            PositionKG = new SortedList<int, int>(Neighbours.Count);

            // for each neighbor of the current node, a 3x3 entry is created in the CSR matrix
            // the number of each new entry is stored in pcsr, where key=altId, value=index
            foreach (int _altId in sortedNeighbors)
            {
                PositionKG.Add(_altId, startIndex);
                for (int i = 0; i < DOF; i++)
                {
                    cols[startIndex + i] =
                    cols[startIndex + i + row_nnz] = _altId * DOF + i;
                    if (DOF == 3)
                        cols[startIndex + i + row_nnz * 2] = _altId * DOF + i;
                }
                startIndex += DOF;
            }
            return row_nnz;
        }

    }
}
