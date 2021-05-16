using System;
using System.Collections.Generic;

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
                Loads.Add(item);
            }
        }

        public void SetSupports(List<Support> supports)
        {
            Supports = supports;
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
