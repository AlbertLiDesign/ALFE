using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Node3D
    {
        public Vector3D Position { get; set; }
        public Node3D(Vector3D position)
        {
            this.Position = position;
        }
    }
}
