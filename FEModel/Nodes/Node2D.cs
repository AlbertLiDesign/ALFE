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

        public Node2D(Vector2D position)
        {
            this.Position = position;
        }
        public Node2D(Vector2D position, bool anchored)
        {
            this.Position = position;
            this.Anchored = anchored;
        }
    }
}
