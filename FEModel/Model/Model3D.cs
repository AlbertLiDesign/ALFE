using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE.FEModel
{
    public class Model3D
    {
        public List<Node3D> Nodes = new List<Node3D>();
        public List<Element> Elements = new List<Element>();
        public List<Load3D> Loads = new List<Load3D>();
        public List<Support3D> Supports = new List<Support3D>();
        public Model3D() { }
        public Model3D(List<Vector3D> nodes, List<Element> elements)
        {
            foreach (var item in nodes)
            {
                this.Nodes.Add(new Node3D(item));
            }
            this.Elements = elements;
        }
        public Model3D(List<Node3D> nodes, List<Element> elements)
        {
            this.Nodes = nodes;
            this.Elements = elements;
        }

        public Model3D(List<Vector3D> nodes, List<Element> elements, List<Load3D> loads, List<Support3D> supports)
        {
            foreach (var item in nodes)
            {
                this.Nodes.Add(new Node3D(item));
            }
            this.Elements = elements;
            this.Loads = loads;
            this.Supports = supports;
        }
        public Model3D(List<Node3D> nodes, List<Element> elements, List<Load3D> loads, List<Support3D> supports)
        {
            this.Nodes = nodes;
            this.Elements = elements;
            this.Loads = loads;
            this.Supports = supports;
        }
    }
}
