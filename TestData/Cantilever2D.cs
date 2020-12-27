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
                > #-----#-----#-----#-----#-----#-----# | F (node 20)
                ^ |           |           |          |           |          |          |  V
                > #-----#-----#-----#-----#-----#-----#
                ^ |           |           |          |           |          |          |
                > #-----#-----#-----#-----#-----#-----#
                ^
             */
            
            List<Vector2D> nodes = new List<Vector2D>(xnum * ynum);
            List<Element> elems = new List<Element>((xnum - 1) * (ynum - 1));
            List<Load2D> loads = new List<Load2D>(1);
            List<Support2D> supports = new List<Support2D>(ynum);

            // Add all nodes
            for (int i = 0; i < xnum; i++)
            {
                for (int j = 0; j < ynum; j++)
                {
                    nodes.Add(new Vector2D(i, j));
                    if (i == 0)
                    {
                        supports.Add(new Support2D(j, SupportType.Fixed));
                    }
                }
            }

            // Add all elements
            for (int i = 0; i < xnum - 1; i++)
            {
                for (int j = 0; j < ynum - 1; j++)
                {
                    List<int> nodesID = new List<int>(4)
                    {
                        i * ynum + j, (i + 1) * ynum + j, (i+1) * ynum+ (j+1) , i * ynum + (j+1) 
                    };
                    elems.Add(new UnitQuad(nodesID, new Material()));
                }
            }

            // Apply the load
            loads.Add(new Load2D(nodes.Count - (int)Math.Ceiling(ynum / 2.0), new Vector2D(0.0f, -1.0f)));

            Model = new Model2D(nodes, elems, loads, supports);
        }
       
    }
}
