using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ALFE.FEModel;

namespace ALFE
{
    public class Cantilever2D
    {
        public int Xnum { get; set; }
        public int Ynum { get; set; }
        public Model Model { get; set; }
        public Cantilever2D(ElementType type, int xnum = 7, int ynum = 5)
        {
            // Create a cantilever with unit quads
            if (type == ElementType.PixelElement)
                PixelType(xnum, ynum);
            else if (type == ElementType.TriangleElement)
                TriangleType(xnum, ynum);
            else if (type == ElementType.QuadElement)
                QuadType(xnum, ynum);

        }
       private void PixelType(int xnum, int ynum)
        {
             /*
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----# | F (node 32)
                ^ |           |           |          |           |          |          |  V
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----#
                ^
             */
            List<Node> nodes = new List<Node>(xnum * ynum);
            List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1));
            List<Load> loads = new List<Load>(1);
            List<Support> supports = new List<Support>(ynum);

            // Add all nodes
            for (int i = 0; i < xnum; i++)
            {
                for (int j = 0; j < ynum; j++)
                {
                    nodes.Add(new Node(i, j));
                    if (i == 0)
                        supports.Add(new Support(j, SupportType.Fixed));
                }
            }

            // Add all elements
            for (int i = 0; i < xnum - 1; i++)
            {
                for (int j = 0; j < ynum - 1; j++)
                {
                    List<Node> nodesElem = new List<Node>(4)
                    {
                        nodes[i * ynum + j],
                        nodes[(i + 1) * ynum + j],
                        nodes[(i+1) * ynum+ (j+1)],
                        nodes[i * ynum + (j+1)]
                    };
                    elems.Add(new Pixel(nodesElem, new Material(1.0f, 0.3f)));
                }
            }

            // Apply the load
            loads.Add(new Load(nodes.Count - (int)Math.Ceiling(ynum / 2.0), new Vector2D(0.0f, -1.0f)));

            Model = new Model(2, nodes, elems, loads, supports);
        }
        private void QuadType(int xnum, int ynum)
        {
            /*
               > #-----#-----#-----#-----#-----#-----#
               ^ |           |           |          |           |          |          |
               > #-----#-----#-----#-----#-----#-----#
               ^ |           |           |          |           |          |          |
               > #-----#-----#-----#-----#-----#-----# | F (node 32)
               ^ |           |           |          |           |          |          |  V
               > #-----#-----#-----#-----#-----#-----#
               ^ |           |           |          |           |          |          |
               > #-----#-----#-----#-----#-----#-----#
               ^
            */
            List<Node> nodes = new List<Node>(xnum * ynum);
            List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1));
            List<Load> loads = new List<Load>(1);
            List<Support> supports = new List<Support>(ynum);

            // Add all nodes
            for (int i = 0; i < xnum; i++)
            {
                for (int j = 0; j < ynum; j++)
                {
                    nodes.Add(new Node(i, j));
                    if (i == 0)
                        supports.Add(new Support(j, SupportType.Fixed));
                }
            }

            // Add all elements
            for (int i = 0; i < xnum - 1; i++)
            {
                for (int j = 0; j < ynum - 1; j++)
                {
                    List<Node> nodesElem = new List<Node>(4)
                    {
                        nodes[i * ynum + j],
                        nodes[(i + 1) * ynum + j],
                        nodes[(i+1) * ynum+ (j+1)],
                        nodes[i * ynum + (j+1)]
                    };
                    elems.Add(new Quadrilateral(nodesElem, new Material(1.0f, 0.3f)));
                }
            }

            // Apply the load
            loads.Add(new Load(nodes.Count - (int)Math.Ceiling(ynum / 2.0), new Vector2D(0.0f, -1.0f)));

            Model = new Model(2, nodes, elems, loads, supports);
        }
        private void TriangleType(int xnum, int ynum)
        {
            /*
               > #-----#-----#-----#-----#-----#-----#
               ^ |     /     |     /    |    /     |     /    |     /    |     /    |
               > #-----#-----#-----#-----#-----#-----#
               ^ |     /     |     /    |    /     |     /    |     /    |     /    |
               > #-----#-----#-----#-----#-----#-----# | F (node 32)
               ^ |     /     |     /    |    /     |     /    |     /    |     /    |  V
               > #-----#-----#-----#-----#-----#-----#
               ^ |     /     |     /    |    /     |     /    |     /    |     /    |
               > #-----#-----#-----#-----#-----#-----#
               ^
            */

            List<Node> nodes = new List<Node>(xnum * ynum);
            List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1)*2);
            List<Load> loads = new List<Load>(1);
            List<Support> supports = new List<Support>(ynum);

            // Add all nodes
            for (int i = 0; i < xnum; i++)
            {
                for (int j = 0; j < ynum; j++)
                {
                    nodes.Add(new Node(i,j));
                    if (i == 0)
                        supports.Add(new Support(j, SupportType.Fixed));
                }
            }

            // Add all elements
            for (int i = 0; i < xnum - 1; i++)
            {
                for (int j = 0; j < ynum - 1; j++)
                {
                    List<Node> nodesIDL = new List<Node>(3)
                    {
                        nodes[i * ynum + j],
                        nodes[(i + 1) * ynum + j],
                        nodes[(i+1) * ynum+ (j+1)]
                    };
                    List<Node> nodesIDU = new List<Node>(3)
                    {
                        nodes[i * ynum + j],
                        nodes[(i+1) * ynum+ (j+1)],
                        nodes[i * ynum+ (j+1)]
                    };
                    elems.Add(new Triangle(nodesIDL, new Material(1.0f, 0.3f)));
                    elems.Add(new Triangle(nodesIDU, new Material(1.0f, 0.3f)));
                }
            }

            // Apply the load
            loads.Add(new Load(nodes.Count - (int)Math.Ceiling(ynum / 2.0), new Vector2D(0.0f, -1.0f)));

            Model = new Model(2, nodes, elems, loads, supports);
        }
    }
}
