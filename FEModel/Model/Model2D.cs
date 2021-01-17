using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Model2D
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
        public Model2D() { }
        public Model2D(List<Node> nodes, List<Element> elements)
        {
            int id = 0;
            foreach (var item in nodes)
            {
                if (item.Dof != 2)
                    throw new Exception("The dimension of all nodes must be 2.");

                if (item.hasID == false)
                    item.SetID(id);
                Nodes.Add(item);
                id++;
            }
            Elements = elements;

            int e = 0;
            foreach (var elem in elements)
            {
                elem.ID = e;
                e++;
            }
                
        }
        public Model2D(List<Node> nodes, List<Element> elements, List<Load> loads, List<Support> supports)
        {
            int id = 0;
            foreach (var item in nodes)
            {
                if (item.hasID == false)
                    item.SetID(id);
                Nodes.Add(item);
                id++;
            }
            Elements = elements;

            foreach (var item in loads)
            {
                if (item.Dof != 2)
                    throw new Exception("The dimension of all loads must be 2.");
                Loads.Add(item);
            }

            Supports = supports;

            int e = 0;
            foreach (var elem in elements)
            {
                elem.ID = e;
                e++;
            }
        }
        #endregion

        public void SetLoads(List<Load> loads)
        {
            foreach (var item in loads)
            {
                if (item.Dof != 2)
                    throw new Exception("The dimension of all loads must be 2.");
                Loads.Add(item);
            }
        }

        public void SetSupports(List<Support> supports)
        {
            Supports = supports;
        }
    }
}
