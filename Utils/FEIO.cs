using ALFE.TopOpt;
using System;
using System.Collections.Generic;
using System.IO;

namespace ALFE
{
    public class FEIO
    {
        public static void WriteIsovalues(string path, BESO beso)
        {
            string output = path + '\\' + "isovalues.txt";
            StreamWriter sw = new StreamWriter(output);
            for (int i = 0; i < beso.isovalues.Count; i++)
            {
                sw.WriteLine(beso.isovalues[i].ToString());
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteVerts(string path, Model model)
        {
            string output = path + '\\' + "ndoes.txt";
            StreamWriter sw = new StreamWriter(output);
            for (int i = 0; i < model.Nodes.Count; i++)
            {
                sw.Write(model.Nodes[i].Position.X.ToString() + ',' + model.Nodes[i].Position.Y.ToString() + ',' + model.Nodes[i].Position.Z.ToString());
                sw.Write("\n");
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteElemVerts(string path, Model model)
        {
            string output = path + '\\' + "elmverts.txt";
            StreamWriter sw = new StreamWriter(output);
            for (int i = 0; i < model.Elements.Count; i++)
            {
                var v_list = model.Elements[i].Nodes;
                for (int j = 0; j < v_list.Count; j++)
                {
                    sw.Write(v_list[j].ID.ToString());
                    if (j != v_list.Count - 1) sw.Write(',');
                }
                sw.Write("\n");
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteVertSensitivities(string path, List<double> Ae, Model model)
        {
            string output = path + '\\' + "VertSensitivities.txt";
            StreamWriter sw = new StreamWriter(output);
            foreach (var ae in Ae)
                sw.WriteLine(ae.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteSensitivities(string path, List<double> Ae)
        {
            StreamWriter sw = new StreamWriter(path);
            foreach (var ae in Ae)
                sw.WriteLine(ae.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteInvalidElements(int iter, string path, List<Element> elems)
        {
            string output = path + '\\' + iter.ToString() + ".txt";
            StreamWriter sw = new StreamWriter(output);
            foreach (var elem in elems)
                if (elem.Xe != 1.0)
                    sw.WriteLine(elem.ID.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteXe(string path, List<Element> elems)
        {
            string output = path + '\\' + "Xe.txt";
            StreamWriter sw = new StreamWriter(output);
            foreach (var elem in elems)
                sw.WriteLine(elem.Xe.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteCOOMatrix(COOMatrix mat, string path)
        {
            StreamWriter sw = new StreamWriter(path);
            sw.WriteLine("# ALFE COO Matrix");
            sw.WriteLine(mat.Rows.ToString() + ' ' + mat.Cols.ToString() + ' ' + mat.NNZ.ToString());
            for (int i = 0; i < mat.NNZ; i++)
            {
                sw.WriteLine((mat.RowArray[i] + 1).ToString() + ' ' + (mat.ColArray[i] + 1).ToString() + ' ' + mat.ValueArray[i].ToString());
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static void WriteKG(CSRMatrix csr, string path, bool symmetry)
        {
            //path += "/KG.mtx";
            StreamWriter sw = new StreamWriter(path);

            sw.WriteLine("%%MatrixMarket matrix coordinate real symmetric");
            var mat = csr.ToCOO(symmetry);
            sw.WriteLine(mat.Rows.ToString() + ' ' + mat.Cols.ToString() + ' ' + mat.NNZ.ToString());
            for (int i = 0; i < mat.NNZ; i++)
            {
                sw.WriteLine((mat.RowArray[i] + 1).ToString() + ' ' + (mat.ColArray[i] + 1).ToString() + ' ' + mat.ValueArray[i].ToString());
            }
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
        public static List<double> ReadXe(string path)
        {
            List<double> Xe = new List<double>();
            if (File.Exists(path))
            {
                StreamReader SR = new StreamReader(path);
                while (!SR.EndOfStream)
                {
                    string line = SR.ReadLine();
                    Xe.Add(double.Parse(line));
                }
            }
            return Xe;
        }
        public static Model ReadTetrahedras(string outputPath)
        {
            List<Node> nodes = new List<Node>();
            List<Element> elems = new List<Element>();
            if (File.Exists(outputPath))
            {
                StreamReader SR = new StreamReader(outputPath);
                while (!SR.EndOfStream)
                {
                    string line = SR.ReadLine();

                    if (line == "Vertices")
                    {
                        var vertNum = int.Parse(SR.ReadLine());
                        for (int i = 0; i < vertNum; i++)
                        {
                            line = SR.ReadLine();
                            string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
                            nodes.Add(new Node(double.Parse(tokens[0]), double.Parse(tokens[1]), double.Parse(tokens[2])));
                        }
                    }
                    if (line == "Tetrahedra")
                    {
                        var tetNum = int.Parse(SR.ReadLine());
                        for (int i = 0; i < tetNum; i++)
                        {
                            line = SR.ReadLine();
                            string[] tokens = line.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);

                            List<Node> verts = new List<Node>(4)
                            {
                                nodes[int.Parse(tokens[0]) - 1],
                                nodes[int.Parse(tokens[1]) - 1],
                                nodes[int.Parse(tokens[2]) - 1],
                                nodes[int.Parse(tokens[3]) - 1],
                            };
                            elems.Add(new Tetrahedron(verts, new Material()));
                        }
                    }
                }
                SR.Close();
                SR.Dispose();
            }

            return new Model(3, nodes, elems);
        }
        public static Model ReadVTK(string path)
        {
            List<Node> nodes = new List<Node>();
            List<Element> elems = new List<Element>();
            Dictionary<int, List<Node>> elemDir = new Dictionary<int, List<Node>>();

            if (File.Exists(path))
            {
                StreamReader SR = new StreamReader(path);
                while (!SR.EndOfStream)
                {
                    string line = SR.ReadLine();//Read Line

                    string[] tokens = line.Split(' ');

                    if (tokens[0] == "POINTS")
                    {
                        int nodeCount = int.Parse(tokens[1]);
                        nodes = new List<Node>(nodeCount);
                        for (int i = 0; i < nodeCount; i++)
                        {
                            string nodeLine = SR.ReadLine();
                            string[] parts = nodeLine.Split(' ');
                            nodes.Add(new Node(double.Parse(parts[0]), double.Parse(parts[1]), double.Parse(parts[2])));
                        }
                    }
                    else if (tokens[0] == "CELLS")
                    {
                        int elemCount = int.Parse(tokens[1]);
                        for (int i = 0; i < elemCount; i++)
                        {
                            string elemLine = SR.ReadLine();
                            string[] parts = elemLine.Split(' ');

                            // Tetrahedron or Quadrilateral
                            if (int.Parse(parts[0]) == 4)
                            {
                                List<Node> elemNodes = new List<Node>(4)
                                {
                                    nodes[int.Parse(parts[1])], nodes[int.Parse(parts[2])],
                                     nodes[int.Parse(parts[3])], nodes[int.Parse(parts[4])]
                                };
                                elemDir.Add(i, elemNodes);
                            }
                        }
                    }
                    else if (tokens[0] == "CELL_TYPES")
                    {
                        elems = new List<Element>(elemDir.Count);
                        int typeCount = int.Parse(tokens[1]);
                        for (int i = 0; i < typeCount; i++)
                        {
                            string typeLine = SR.ReadLine();

                            // Tetrahedron
                            if (int.Parse(typeLine) == 10 && elemDir[i].Count == 4)
                                elems.Add(new Tetrahedron(elemDir[i], new Material()));
                        }
                    }
                }
                SR.Close();
                SR.Dispose();
            }
            else
                Console.WriteLine("The file cannot be loaded.");

            return new Model(3, nodes, elems);
        }

        /// <summary>
        /// Write a finite element model into a .al file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="model">A finite element model.</param>
        public static void WriteFEModel(string path, Model model)
        {
            string output = path + ".txt";
            StreamWriter sw = new StreamWriter(output);
            sw.WriteLine("%This file is created by ALFE.");
            sw.WriteLine("FEA Parameters: ");
            sw.WriteLine("DOF: " + model.DOF.ToString());
            sw.WriteLine("Element Type: " + model.Elements[0].Type.ToString());
            sw.WriteLine("Node Count: " + model.Nodes.Count.ToString());
            sw.WriteLine("Element Count: " + model.Elements.Count.ToString());
            sw.WriteLine("Load Count: " + model.Loads.Count.ToString());
            sw.WriteLine("Support Count: " + model.Supports.Count.ToString());
            sw.WriteLine("Young's Modulus: " + model.Elements[0].Material.E.ToString());
            sw.WriteLine("Possion Rate: " + model.Elements[0].Material.nu.ToString());

            sw.WriteLine();

            sw.WriteLine("Model Info: ");
            foreach (var node in model.Nodes)
                sw.WriteLine("N," + node.Position.X.ToString() + ',' + node.Position.Y.ToString() + ',' + node.Position.Z.ToString());
            foreach (var elem in model.Elements)
            {
                sw.Write("E,");
                int n = 1;
                foreach (var node in elem.Nodes)
                {
                    sw.Write(node.ID.ToString());
                    if (n != elem.Nodes.Count)
                        sw.Write(',');
                    n++;
                }
                sw.Write("\n");
            }
            foreach (var item in model.Loads)
                sw.WriteLine("L," + item.NodeID.ToString() + ',' +
                    item.ForceVector.X.ToString() + ',' + item.ForceVector.Y.ToString() + ',' + item.ForceVector.Z.ToString());
            foreach (var item in model.Supports)
                sw.WriteLine("S," + item.NodeID.ToString() + ',' + item.Type.ToString());

            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// Read a finite element model with .al format
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Return a finite element model.</returns>
        public static Model ReadFEModel(string path)
        {
            Model model = new Model();
            if (File.Exists(path))
            {
                int dof = 0;
                ElementType elementType = ElementType.SquareElement;
                List<Node> nodes = new List<Node>();
                List<Element> elements = new List<Element>();
                List<Load> loads = new List<Load>();
                List<Support> supports = new List<Support>();
                Material material = new Material();

                StreamReader SR = new StreamReader(path);

                #region Read FE parameters
                bool readFEpara = false;
                while (readFEpara == false)
                {
                    string line = SR.ReadLine();

                    if (line == "FEA Parameters: ")
                    {
                        string[] value = SR.ReadLine().Split(':');
                        if (value[0] == "DOF")
                            dof = int.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Element Type")
                        {
                            string type = value[1].Split(' ')[1];
                            if (type == "SquareElement")
                                elementType = ElementType.SquareElement;
                            else if (type == "TriangleElement")
                                elementType = ElementType.TriangleElement;
                            else if (type == "QuadElement")
                                elementType = ElementType.QuadElement;
                            else if (type == "TetrahedronElement")
                                elementType = ElementType.TetrahedronElement;
                            else if (type == "VoxelElement")
                                elementType = ElementType.VoxelElement;
                            else if (type == "HexahedronElement")
                                elementType = ElementType.HexahedronElement;
                            else
                                throw new Exception("Unknown element type.");
                        }

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Node Count")
                            nodes = new List<Node>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Element Count")
                            elements = new List<Element>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Load Count")
                            loads = new List<Load>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Support Count")
                            supports = new List<Support>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Young's Modulus")
                            material.E = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Possion Rate")
                            material.nu = double.Parse(value[1].Split(' ')[1]);

                        readFEpara = true;
                    }
                }
                #endregion


                #region Read the model
                while (!SR.EndOfStream)
                {
                    string[] value = SR.ReadLine().Split(',');


                    if (value[0] == "N")
                        nodes.Add(new Node(dof, double.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3])));


                    if (value[0] == "E")
                    {
                        List<Node> elemNodes = new List<Node>();
                        for (int i = 1; i < value.Length; i++)
                        {
                            int id = int.Parse(value[i]);
                            elemNodes.Add(nodes[id]);
                        }
                        if (elementType == ElementType.SquareElement)
                            elements.Add(new Square(elemNodes, material));
                        else if (elementType == ElementType.TriangleElement)
                            elements.Add(new Triangle(elemNodes, material));
                        else
                            throw new Exception("Unknown element type.");
                    }

                    if (value[0] == "L")
                        loads.Add(new Load(dof, int.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3]), double.Parse(value[4])));

                    if (value[0] == "S")
                        if (value[2] == "Fixed")
                            supports.Add(new Support(int.Parse(value[1]), SupportType.Fixed));
                }
                #endregion

                model = new Model(2, nodes, elements, loads, supports);

                SR.Close();
                SR.Dispose();
            }
            return model;
        }


        /// <summary>
        /// Write a finite element model and the parameters of BESO topology optimization into a .al file.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="BESO">A finite element model for BESO topology optimization.</param>
        public static void WriteBESO(string path, BESO beso, int solver)
        {
            string output = path + "/beso.txt";
            StreamWriter sw = new StreamWriter(output);
            sw.WriteLine("%This file is created by ALFE.");

            Model model = beso.Model;
            sw.WriteLine("FEA Parameters: ");
            sw.WriteLine("DOF: " + model.DOF.ToString());
            sw.WriteLine("Element Type: " + model.Elements[0].Type.ToString());
            sw.WriteLine("Node Count: " + model.Nodes.Count.ToString());
            sw.WriteLine("Element Count: " + model.Elements.Count.ToString());
            sw.WriteLine("Load Count: " + model.Loads.Count.ToString());
            sw.WriteLine("Support Count: " + model.Supports.Count.ToString());
            sw.WriteLine("Young's Modulus: " + model.Elements[0].Material.E.ToString());
            sw.WriteLine("Possion Rate: " + model.Elements[0].Material.nu.ToString());

            sw.WriteLine();

            sw.WriteLine("BESO Parameters: ");
            sw.WriteLine("Volume Fraction: " + beso.VolumeFraction.ToString());
            sw.WriteLine("Evolution Rate: " + beso.EvolutionRate.ToString());
            sw.WriteLine("Filter Radius: " + beso.FilterRadius.ToString());
            sw.WriteLine("Penalty Exponent: " + beso.PenaltyExponent.ToString());
            sw.WriteLine("Maximum Iteration: " + beso.MaximumIteration.ToString());
            sw.WriteLine("Solver: " + solver.ToString());

            sw.WriteLine();

            sw.WriteLine("Model Info: ");
            foreach (var node in model.Nodes)
                sw.WriteLine("N," + node.Position.X.ToString() + ',' + node.Position.Y.ToString() + ',' + node.Position.Z.ToString());
            foreach (var elem in model.Elements)
            {
                if (elem.NonDesign) sw.Write("NE,");
                else sw.Write("E,");
                int n = 1;
                foreach (var node in elem.Nodes)
                {
                    sw.Write(node.ID.ToString());
                    if (n != elem.Nodes.Count)
                        sw.Write(',');
                    n++;
                }
                sw.Write("\n");
            }
            foreach (var item in model.Loads)
                sw.WriteLine("L," + item.NodeID.ToString() + ',' +
                    item.ForceVector.X.ToString() + ',' + item.ForceVector.Y.ToString() + ',' + item.ForceVector.Z.ToString());
            foreach (var item in model.Supports)
                sw.WriteLine("S," + item.NodeID.ToString() + ',' + item.Type.ToString());

            sw.Flush();
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// Read a finite element model and the parameters of BESO topology optimization with .al format
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Return a finite element model.</returns>
        public static BESO ReadBESO(string path, string name)
        {
            string besoPath = path + "\\" + name + ".txt";
            string projectPath = path + "\\solution";

            Model model = new Model();

            int dof = 0;
            ElementType elementType = ElementType.SquareElement;
            List<Node> nodes = new List<Node>();
            List<Element> elements = new List<Element>();
            List<Load> loads = new List<Load>();
            List<Support> supports = new List<Support>();
            Material material = new Material();
            double ert = 0.0;
            double rmin = 0.0;
            double vf = 0.0;
            double p = 0;
            int maxIter = 0;
            int solver = 0;
            bool parallel = false;

            if (File.Exists(besoPath))
            {
                StreamReader SR = new StreamReader(besoPath);

                #region Read FE parameters
                bool readFEpara = false;

                while (readFEpara == false)
                {
                    string line = SR.ReadLine();

                    if (line == "FEA Parameters: ")
                    {
                        string[] value = SR.ReadLine().Split(':');
                        if (value[0] == "DOF")
                            dof = int.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Element Type")
                        {
                            string type = value[1].Split(' ')[1];
                            switch (type)
                            {
                                case "SquareElement":
                                    elementType = ElementType.SquareElement;
                                    break;
                                case "TriangleElement":
                                    elementType = ElementType.TriangleElement;
                                    break;
                                case "QuadElement":
                                    elementType = ElementType.QuadElement;
                                    break;
                                case "TetrahedronElement":
                                    elementType = ElementType.TetrahedronElement;
                                    break;
                                case "VoxelElement":
                                    elementType = ElementType.VoxelElement;
                                    break;
                                case "HexahedronElement":
                                    elementType = ElementType.HexahedronElement;
                                    break;
                                default:
                                    throw new Exception("Unknown element type.");
                            }
                        }

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Node Count")
                            nodes = new List<Node>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Element Count")
                            elements = new List<Element>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Load Count")
                            loads = new List<Load>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Support Count")
                            supports = new List<Support>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Young's Modulus")
                            material.E = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Possion Rate")
                            material.nu = double.Parse(value[1].Split(' ')[1]);
                        if (value[0] == "Parallel Computing")
                            parallel = bool.Parse(value[1].Split(' ')[1]);

                        readFEpara = true;
                    }
                }
                #endregion

                #region Read BESO parameters
                bool readBESOpara = false;
                while (readBESOpara == false)
                {
                    string line = SR.ReadLine();

                    if (line == "BESO Parameters: ")
                    {
                        string[] value = SR.ReadLine().Split(':');
                        if (value[0] == "Volume Fraction")
                            vf = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Evolution Rate")
                            ert = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Filter Radius")
                            rmin = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Penalty Exponent")
                            p = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Maximum Iteration")
                            maxIter = int.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Solver")
                            solver = int.Parse(value[1].Split(' ')[1]);

                        readBESOpara = true;
                    }
                }
                #endregion

                #region Read the model
                while (!SR.EndOfStream)
                {
                    string[] value = SR.ReadLine().Split(',');


                    if (value[0] == "N")
                        nodes.Add(new Node(dof, double.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3])));


                    if (value[0] == "E" || value[0] == "NE")
                    {
                        bool nondesign = false;
                        if (value[0] == "NE") nondesign = true;
                        List<Node> elemNodes = new List<Node>();
                        for (int i = 1; i < value.Length; i++)
                        {
                            int id = int.Parse(value[i]);
                            elemNodes.Add(nodes[id]);
                        }

                        switch (elementType)
                        {
                            case ElementType.SquareElement:
                                elements.Add(new Square(elemNodes, material, nondesign));
                                break;
                            case ElementType.TriangleElement:
                                elements.Add(new Triangle(elemNodes, material,1.0, nondesign));
                                break;
                            case ElementType.QuadElement:
                                elements.Add(new Quadrilateral(elemNodes, material, 1.0, nondesign));
                                break;
                            case ElementType.TetrahedronElement:
                                elements.Add(new Tetrahedron(elemNodes, material, nondesign));
                                break;
                            case ElementType.HexahedronElement:
                                elements.Add(new Hexahedron(elemNodes, material, nondesign));
                                break;
                            case ElementType.VoxelElement:
                                elements.Add(new Voxel(elemNodes, material, nondesign));
                                break;
                            default:
                                throw new Exception("Unknown element type.");
                        }
                    }

                    if (value[0] == "L")
                        loads.Add(new Load(dof, int.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3]), double.Parse(value[4])));

                    if (value[0] == "S")
                        if (value[2] == "Fixed")
                            supports.Add(new Support(int.Parse(value[1]), SupportType.Fixed));
                }
                #endregion

                if (elementType == ElementType.SquareElement || elementType == ElementType.QuadElement || elementType == ElementType.TriangleElement)
                    model = new Model(2, nodes, elements, loads, supports);
                else
                    model = new Model(3, nodes, elements, loads, supports);

                SR.Close();
                SR.Dispose();
            }
            BESO beso = new BESO(projectPath, new FESystem(model), rmin, ert, p, vf, maxIter, (Solver)solver);
            return beso;
        }


        /// <summary>
        /// Read a finite element model and the parameters of BESO topology optimization with .al format
        /// </summary>
        /// <param name="path">File path.</param>
        /// <returns>Return a finite element model.</returns>
        public static SPBESO ReadSPBESO(string path, string name)
        {
            string besoPath = path + "\\" + name + ".txt";
            string projectPath = path + "\\solution";
            string omega_d_path = path + "\\omegaD.txt";

            Model model = new Model();

            int dof = 0;
            ElementType elementType = ElementType.SquareElement;
            List<Node> nodes = new List<Node>();
            List<Element> elements = new List<Element>();
            List<Load> loads = new List<Load>();
            List<Support> supports = new List<Support>();
            Material material = new Material();
            double ert = 0.0;
            double rmin = 0.0;
            double vf = 0.0;
            double p = 0;
            int maxIter = 0;
            int solver = 0;
            bool parallel = false;

            if (File.Exists(besoPath))
            {
                StreamReader SR = new StreamReader(besoPath);

                #region Read FE parameters
                bool readFEpara = false;

                while (readFEpara == false)
                {
                    string line = SR.ReadLine();

                    if (line == "FEA Parameters: ")
                    {
                        string[] value = SR.ReadLine().Split(':');
                        if (value[0] == "DOF")
                            dof = int.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Element Type")
                        {
                            string type = value[1].Split(' ')[1];
                            switch (type)
                            {
                                case "SquareElement":
                                    elementType = ElementType.SquareElement;
                                    break;
                                case "TriangleElement":
                                    elementType = ElementType.TriangleElement;
                                    break;
                                case "QuadElement":
                                    elementType = ElementType.QuadElement;
                                    break;
                                case "TetrahedronElement":
                                    elementType = ElementType.TetrahedronElement;
                                    break;
                                case "VoxelElement":
                                    elementType = ElementType.VoxelElement;
                                    break;
                                case "HexahedronElement":
                                    elementType = ElementType.HexahedronElement;
                                    break;
                                default:
                                    throw new Exception("Unknown element type.");
                            }
                        }

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Node Count")
                            nodes = new List<Node>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Element Count")
                            elements = new List<Element>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Load Count")
                            loads = new List<Load>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Support Count")
                            supports = new List<Support>(int.Parse(value[1].Split(' ')[1]));

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Young's Modulus")
                            material.E = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Possion Rate")
                            material.nu = double.Parse(value[1].Split(' ')[1]);
                        if (value[0] == "Parallel Computing")
                            parallel = bool.Parse(value[1].Split(' ')[1]);

                        readFEpara = true;
                    }
                }
                #endregion

                #region Read BESO parameters
                bool readBESOpara = false;
                while (readBESOpara == false)
                {
                    string line = SR.ReadLine();

                    if (line == "BESO Parameters: ")
                    {
                        string[] value = SR.ReadLine().Split(':');
                        if (value[0] == "Volume Fraction")
                            vf = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Evolution Rate")
                            ert = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Filter Radius")
                            rmin = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Penalty Exponent")
                            p = double.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Maximum Iteration")
                            maxIter = int.Parse(value[1].Split(' ')[1]);

                        value = SR.ReadLine().Split(':');
                        if (value[0] == "Solver")
                            solver = int.Parse(value[1].Split(' ')[1]);

                        readBESOpara = true;
                    }
                }
                #endregion

                #region Read the model
                while (!SR.EndOfStream)
                {
                    string[] value = SR.ReadLine().Split(',');


                    if (value[0] == "N")
                        nodes.Add(new Node(dof, double.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3])));


                    if (value[0] == "E" || value[0] == "NE")
                    {
                        bool nondesign = false;
                        if (value[0] == "NE") nondesign = true;
                        List<Node> elemNodes = new List<Node>();
                        for (int i = 1; i < value.Length; i++)
                        {
                            int id = int.Parse(value[i]);
                            elemNodes.Add(nodes[id]);
                        }

                        switch (elementType)
                        {
                            case ElementType.SquareElement:
                                elements.Add(new Square(elemNodes, material, nondesign));
                                break;
                            case ElementType.TriangleElement:
                                elements.Add(new Triangle(elemNodes, material, 1.0, nondesign));
                                break;
                            case ElementType.QuadElement:
                                elements.Add(new Quadrilateral(elemNodes, material, 1.0, nondesign));
                                break;
                            case ElementType.TetrahedronElement:
                                elements.Add(new Tetrahedron(elemNodes, material, nondesign));
                                break;
                            case ElementType.HexahedronElement:
                                elements.Add(new Hexahedron(elemNodes, material, nondesign));
                                break;
                            case ElementType.VoxelElement:
                                elements.Add(new Voxel(elemNodes, material, nondesign));
                                break;
                            default:
                                throw new Exception("Unknown element type.");
                        }
                    }

                    if (value[0] == "L")
                        loads.Add(new Load(dof, int.Parse(value[1]), double.Parse(value[2]), double.Parse(value[3]), double.Parse(value[4])));

                    if (value[0] == "S")
                        if (value[2] == "Fixed")
                            supports.Add(new Support(int.Parse(value[1]), SupportType.Fixed));
                }
                #endregion

                if (elementType == ElementType.SquareElement || elementType == ElementType.QuadElement || elementType == ElementType.TriangleElement)
                    model = new Model(2, nodes, elements, loads, supports);
                else
                    model = new Model(3, nodes, elements, loads, supports);

                SR.Close();
                SR.Dispose();
            }

            var omega_d = new double[model.Elements.Count];
            if (File.Exists(omega_d_path))
            {
                StreamReader SR2 = new StreamReader(omega_d_path);
                for (int i = 0; i < model.Elements.Count; i++)
                {
                    omega_d[i] = double.Parse(SR2.ReadLine());
                }
                SR2.Close();
                SR2.Dispose();
            }
            SPBESO spbeso = new SPBESO(projectPath, new FESystem(model), rmin, omega_d, ert, p, vf, maxIter, (Solver)solver);
            return spbeso;
        }
    }
}
