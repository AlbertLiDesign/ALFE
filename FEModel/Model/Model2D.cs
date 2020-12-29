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
            foreach (var item in nodes)
            {
                this.Nodes.Add(new Node2D(item));
            }
            this.Elements = elements;
        }
        public Model2D(List<Node2D> nodes, List<Element> elements)
        {
            this.Nodes = nodes;
            this.Elements = elements;
        }
        public Model2D(List<Vector2D> nodes, List<Element> elements, List<Load2D> loads, List<Support2D> supports)
        {
            foreach (var item in nodes)
            {
                this.Nodes.Add(new Node2D(item));
            }
            this.Elements = elements;
            this.Loads = loads;
            this.Supports = supports;
        }
        public Model2D(List<Node2D> nodes, List<Element> elements, List<Load2D> loads, List<Support2D> supports)
        {
            this.Nodes = nodes;
            this.Elements = elements;
            this.Loads = loads;
            this.Supports = supports;
        }
        #endregion

        /// <summary>
        /// Compute the uniform elementary stiffness matrix.
        /// </summary>
        /// <returns></returns>
        public float[,] ComputeUniformK()
        {
            var ele = this.Elements[0];
            ele.ComputeK();
            return ele.Ke;
        }
    }
}
