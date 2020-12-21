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
        public Model2D Model { get; }
        public Cantilever2D(int xnum = 7, int ynum = 5)
        {
            // Create a cantilever with unit quads
            /*
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----# | F
                ^ |           |           |          |           |          |          |  V
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----#
                ^
             */

            // Add all nodes
            List<Vector2D> nodes = new List<Vector2D>(xnum * ynum);
            List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1));

            for (int i = 0; i < xnum; i++)
            {
                for (int j = 0; j < ynum; j++)
                {
                    nodes.Add(new Vector2D(i, j));
                }
            }

            for (int i = 0; i < xnum - 1; i++)
            {
                for (int j = 0; j < ynum - 1; j++)
                {
                    int[] ids = new int[4]
                    {
                        i, i +1, i + j * (xnum-1), (i+1) + j*(xnum -1)
                    };
                    Node2D[] eleNodes = new Node2D[4]
                    {
                        new Node2D(nodes[ids[0]]), new Node2D(nodes[ids[1]]),
                        new Node2D(nodes[ids[2]]), new Node2D(nodes[ids[3]]),
                    };

                    elems.Add(new UnitQuad(ids, eleNodes, new Material()));
                }
            }

            Model = new Model2D(nodes, elems);

        }
    }
}
