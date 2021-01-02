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
        public List<Node2D> Nodes = new List<Node2D>();

        /// <summary>
        /// Elements
        /// </summary>
        public List<Element> Elements = new List<Element>();

        /// <summary>
        /// Loads
        /// </summary>
        public List<Load2D> Loads = new List<Load2D>();

        /// <summary>
        /// Supports
        /// </summary>
        public List<Support2D> Supports = new List<Support2D>();

        #region Construction
        public Model2D() { }
        public Model2D(List<Vector2D> nodes, List<Element> elements)
        {
            int id = 0;
            foreach (var item in nodes)
            {
                this.Nodes.Add(new Node2D(item, id));
                id++;
            }
            this.Elements = elements;

            foreach (var elem in elements)
                foreach (var item in elem.Nodes)
                    elem.NodeID.Add(item.ID);
        }
        public Model2D(List<Node2D> nodes, List<Element> elements)
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

            foreach (var elem in elements)
                foreach (var item in elem.Nodes)
                    elem.NodeID.Add(item.ID);
        }
        public Model2D(List<Vector2D> nodes, List<Element> elements, List<Load2D> loads, List<Support2D> supports)
        {
            int id = 0;
            foreach (var item in nodes)
            {
                this.Nodes.Add(new Node2D(item,id));
                id++;
            }
            this.Elements = elements;
            this.Loads = loads;
            this.Supports = supports;

            foreach (var elem in elements)
                foreach (var item in elem.Nodes)
                    elem.NodeID.Add(item.ID);
        }
        public Model2D(List<Node2D> nodes, List<Element> elements, List<Load2D> loads, List<Support2D> supports)
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
            this.Loads = loads;
            this.Supports = supports;

            foreach (var elem in elements)
                foreach (var item in elem.Nodes)
                    elem.NodeID.Add(item.ID);
        }
        #endregion
    }
}
