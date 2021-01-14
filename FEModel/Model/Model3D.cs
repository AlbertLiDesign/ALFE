using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Model3D
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public List<Node> Nodes = new List<Node>();

        /// <summary>
        /// Elements
        /// </summary>
        public List<Element> Elements = new List<Element>();

        /// <summary>
        /// Loads
        /// </summary>
        public List<Load> Loads = new List<Load>();

        /// <summary>
        /// Supports
        /// </summary>
        public List<Support> Supports = new List<Support>();

        #region Construction
        public Model3D() { }
        public Model3D(List<Node> nodes, List<Element> elements)
        {
            int id = 0;
            foreach (var item in nodes)
            {
                if (item.Dof != 3)
                    throw new Exception("The dimension of all nodes must be 3.");

                if (item.hasID == false)
                    item.SetID(id);
                Nodes.Add(item);
                id++;
            }
            this.Elements = elements;

            int e = 0;
            foreach (var elem in elements)
            {
                elem.ID = e;
                e++;
            }
        }
        public Model3D(List<Node> nodes, List<Element> elements, List<Load> loads, List<Support> supports)
        {
            int id = 0;
            foreach (var item in nodes)
            {
                if (item.hasID == false)
                    item.SetID(id);
                Nodes.Add(item);
                id++;
            }
            this.Elements = elements;

            foreach (var item in loads)
            {
                if (item.Dof != 3)
                    throw new Exception("The dimension of all loads must be 3.");
                Loads.Add(item);
            }

            this.Supports = supports;

            int e = 0;
            foreach (var elem in elements)
            {
                elem.ID = e;
                e++;
            }
        }
        #endregion
    }
}
