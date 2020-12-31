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

        public Node2D(float x, float y, int index)
        {
            this.Position = new Vector2D(x,y);
            this.ID = index;
        }
        public Node2D(Vector2D position, int index)
        {
            this.Position = position;
            this.ID = index;
        }
        public Node2D(Vector2D position, int index, bool active)
        {
            this.Position = position;
            this.ID = index;
            this.Active = active;
        }
    }
}
