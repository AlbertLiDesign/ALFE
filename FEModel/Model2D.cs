using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Model2D
    {
        public List<Node2D> Nodes = new List<Node2D>();
        public List<Element> Elements = new List<Element>();
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
    }
}
