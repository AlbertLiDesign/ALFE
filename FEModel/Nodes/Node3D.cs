using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Node3D : Node
    {
        public Vector3D Position { get; set; }
        public Vector3D Displacement { get; set; }

        public Node3D(Vector3D position)
        {
            this.Position = position;
        }
        public Node3D(Vector3D position, bool active)
        {
            this.Position = position;
            this.Active = active;
        }

    }
}
