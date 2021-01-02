using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Node2D : Node
    {
        public Vector2D Position { get; set; }

        public Vector2D Displacement = new Vector2D();

        public Node2D(Node2D node)
        {
            this.Position = node.Position;
            this.ID = node.ID;
            hasID = true;
        }
        public Node2D(Node2D node, int index)
        {
            this.Position = node.Position;
            this.ID = index;
            hasID = true;
        }
        public Node2D(float x, float y)
        {
            this.Position = new Vector2D(x, y);
        }
        public Node2D(float x, float y, bool active)
        {
            this.Position = new Vector2D(x, y);
            this.Active = active;
        }
        public Node2D(float x, float y, int index)
        {
            this.Position = new Vector2D(x,y);
            this.ID = index;
            hasID = true;
        }
        public Node2D(Vector2D position, int index)
        {
            this.Position = position;
            this.ID = index;
            hasID = true;
        }
        public Node2D(Vector2D position, int index, bool active)
        {
            this.Position = position;
            this.ID = index;
            this.Active = active;
            hasID = true;
        }
        public void SetID(int index)
        {
            this.ID = index;
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
            row_nnz = Neighbours.Count * 2;

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
                for (int i = 0; i < 2; i++)
                {
                    cols[startIndex + i] =
                    cols[startIndex + i + row_nnz] = _altId * 2 + i;
                }
                startIndex += 2;
            }
            return row_nnz;
        }
    }
}
