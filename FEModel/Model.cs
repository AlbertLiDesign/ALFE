using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ALFE
{
    public class Model
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

        /// <summary>
        /// Degree of freedom
        /// </summary>
        public int DOF;

        #region Construction
        public Model() { }
        public Model(int dof, List<Node> nodes, List<Element> elements)
        {
            int id = 0;
            int e = 0;
            DOF = dof;

            foreach (var item in nodes)
            {
                if (item.Dim != DOF)
                    throw new Exception("The KG_Dim of the model fails to match the KG_Dim of the nodes.");

                if (item.hasID == false)
                    item.SetID(id);
                Nodes.Add(item);
                id++;
            }


            Elements = elements;

            foreach (var elem in elements)
            {
                if (elem.Dim != DOF)
                    throw new Exception("The KG_Dim of the model fails to match the KG_Dim of the elements.");

                elem.ID = e;
                e++;
            }
        }
        public Model(int dof, List<Node> nodes, List<Element> elements, List<Load> loads, List<Support> supports)
        {
            DOF = dof;

            int id = 0;
            foreach (var item in nodes)
            {
                if (item.Dim != DOF)
                    throw new Exception("The KG_Dim of the model fails to match the KG_Dim of the nodes.");

                if (item.hasID == false)
                    item.SetID(id);
                Nodes.Add(item);
                id++;
            }
            Elements = elements;

            foreach (var item in loads)
            {
                if (item.DOF != DOF)
                    throw new Exception("The KG_Dim of the model fails to match the KG_Dim of the loads.");

                // Load area should be signed as a non-design domain
                Nodes[item.NodeID].NonDesign = true;
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
                if (item.DOF != DOF)
                    throw new Exception("The KG_Dim of the model fails to match the KG_Dim of the loads.");

                // Load area should be signed as a non-design domain
                Nodes[item.NodeID].NonDesign = true;
                Loads.Add(item);
            }
        }

        public void SetSupports(List<Support> supports)
        {
            Supports = supports;
        }

        public List<double> ComputeVertSensitivities(List<double> elemSensitivities)
        {
            GetAdjacentNodes();
            GetConnectedElements();
            var Vert_Value = new List<double>();
            foreach (var item in Nodes)
            {
                double sensitivity = 0.0;
                for (int i = 0; i < item.ElementID.Count; i++)
                {
                    sensitivity += elemSensitivities[item.ElementID[i]] / item.ElementID.Count;
                }
                Vert_Value.Add(sensitivity);
            }

            // 映射到0-1
            double max = Vert_Value.Max();
            double min = Vert_Value.Min();
            for (int i = 0; i < Vert_Value.Count; i++)
            {
                Vert_Value[i] = (Vert_Value[i] - min) / (max - min);
            }
            return Vert_Value;
        }

        /// <summary>
        /// Get the neighbours of each node.
        /// </summary>
        private void GetAdjacentNodes()
        {
            Parallel.ForEach(Nodes, node =>
            {
                foreach (var item in node.ElementID)
                foreach (var neighbour in Elements[item].Nodes)
                    if (neighbour.Active)
                        lock (node.Neighbours)
                            node.Neighbours.Add(neighbour.ActiveID);
            });
        }

        /// <summary>
        /// Get the connected elements of each node.
        /// </summary>
        private void GetConnectedElements()
        {
            // in each node make a list of elements to which it belongs
            foreach (var elem in Elements)
                foreach (var node in elem.Nodes)
                {
                    // Sign non-design domain
                    if (node.NonDesign)
                    {
                        elem.NonDesign = true;
                    }
                    node.ElementID.Add(elem.ID);
                }
                
        }
        public string ModelInfo()
        {
            string info = "------------------- Model Info -------------------";
            info += '\n';
            info += "Nodes: " + Nodes.Count.ToString();
            info += '\n';
            info += "Degree-of-freedom: " + (Nodes.Count * DOF).ToString();
            info += '\n';
            info += "Elements: " + Elements.Count.ToString();
            info += '\n';
            info += "Type: " + Elements[0].Type.ToString();
            info += '\n';

            return info;
        }
    }
}
