using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Node2D
    {
        public int ID { get; set; }
        public Vector2D Position { get; set; }
        public Vector2D Displacement { get; set; }
        public Node2D(Vector2D position)
        {
            this.Position = position;
        }
    }
}
