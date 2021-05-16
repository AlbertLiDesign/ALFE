using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class DOF
    {
        public int ID;
        public int ActiveID;
        public int NodeID;
        public bool isFixed = false;

        public DOF(int nodeID)
        {
            NodeID = nodeID;
        }
        public DOF(bool isFixed, int nodeID)
        {
            this.isFixed = isFixed;
            this.NodeID = nodeID;
        }

        public void SetID(int id)
        {
            ID = id;
        }
        public void SetActiveID(int id)
        {
            ActiveID = id;
        }
        public void SetFixed(bool state)
        {
            isFixed = state;
        }
    }
}
