﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    public class FEIO
    {
        //public static void writeVTK(FESystem fes, BESO_TopOpt beso, Material material, string path)
        //{
        //    List<PlanktonXYZ> nodes = fes.model.nodes;
        //    List<Element> elements = fes.model.elements;
        //    List<Load> loads = fes.loads;
        //    List<Support> supports = fes.supports;

        //    int dim = fes.model.dim;

        //    StreamWriter sw = new StreamWriter(path);
        //    sw.WriteLine("# vtk DataFile Version 3.0");
        //    sw.WriteLine("Generated by ALG");
        //    sw.WriteLine("ASCII");
        //    sw.WriteLine("DATASET UNSTRUCTURED_GRID");

        //    var ci = System.Globalization.CultureInfo.InvariantCulture;

        //    // 2D case
        //    if (dim == 2)
        //    {
        //        sw.WriteLine("POINTS" + ' ' + nodes.Count.ToString() + "double");
        //        for (int i = 0; i < nodes.Count; i++)
        //        {
        //            var v = nodes[i];
        //            sw.WriteLine(v.X.ToString() + ' ' + v.Y.ToString());
        //        }
        //        sw.WriteLine("CELLS" + ' ' + '4' + ' ' + (4 * elements.Count).ToString());
        //        for (int i = 0; i < elements.Count; i++)
        //        {
        //            sw.WriteLine("4" + ' ' + elements[i].node[0].ToString() + ' ' + elements[i].node[1].ToString() + ' '
        //                + elements[i].node[2].ToString() + ' ' + elements[i].node[3].ToString());
        //        }

        //        //for (int i = 0; i < loads.Count; i++)
        //        //{
        //        //    sw.WriteLine("L," + loads[i].node.ToString() + ',' + loads[i].load[0].ToString() + ',' + loads[i].load[1].ToString());
        //        //}
        //        //for (int i = 0; i < supports.Count; i++)
        //        //{
        //        //    sw.WriteLine("S," + supports[i].node.ToString() + ',' + supports[i].support.ToString());
        //        //}
        //    }

        //    // 3D case
        //    if (dim == 3)
        //    {
        //        sw.WriteLine("POINTS" + ' ' + nodes.Count.ToString() + ' ' + "double");
        //        for (int i = 0; i < nodes.Count; i++)
        //        {
        //            var v = nodes[i];
        //            sw.WriteLine(v.X.ToString() + ' ' + v.Y.ToString() + ' ' + v.Z.ToString());
        //        }
        //        sw.WriteLine("CELLS" + ' ' + '8' + ' ' + (9 * elements.Count).ToString());
        //        for (int i = 0; i < elements.Count; i++)
        //        {
        //            sw.WriteLine("8" + ' ' + elements[i].node[0].ToString() + ' ' + elements[i].node[1].ToString() + ' '
        //                + elements[i].node[2].ToString() + ' ' + elements[i].node[3].ToString() + ' ' + elements[i].node[4].ToString() + ' '
        //                + elements[i].node[5].ToString() + ' ' + elements[i].node[7].ToString() + ' ' + elements[i].node[6].ToString());
        //        }

        //        sw.WriteLine("CELL_TYPES" + ' ' + elements.Count.ToString());
        //        for (int i = 0; i < elements.Count; i++)
        //        {
        //            sw.WriteLine("12");
        //        }

        //        sw.WriteLine("CELL_DATA" + ' ' + elements.Count.ToString());
        //        sw.WriteLine("SCALARS material int");
        //        sw.WriteLine("LOOKUP_TABLE default");
        //        for (int i = 0; i < elements.Count; i++)
        //        {
        //            sw.WriteLine("1");
        //        }
        //    }


        //    sw.Flush();
        //    sw.Close();
        //    sw.Dispose();
        //}
        //public static void writeVoxels(List<PlanktonXYZ> nodes, List<Element> elements, string path)
        //{
        //    StreamWriter sw = new StreamWriter(path);
        //    sw.WriteLine(nodes.Count.ToString() + '\t' + (elements.Count).ToString());
        //    var ci = System.Globalization.CultureInfo.InvariantCulture;

        //    sw.WriteLine("Nodes");
        //    for (int i = 0; i < nodes.Count; i++)
        //    {
        //        var v = nodes[i];
        //        sw.WriteLine('\t' + i.ToString() + ',' + '\t' +
        //            v.X.ToString() + ',' + '\t' +
        //            v.Y.ToString() + ',' + '\t' +
        //            v.Z.ToString());
        //    }

        //    sw.WriteLine("Elements");
        //    for (int i = 0; i < elements.Count; i++)
        //    {
        //        sw.WriteLine('\t' + i.ToString() + ',' + '\t' + elements[i].node[0].ToString() + ',' + '\t' + elements[i].node[1].ToString() + ',' + '\t'
        //            + elements[i].node[2].ToString() + ',' + '\t' + elements[i].node[3].ToString() + ',' + '\t' + elements[i].node[4].ToString() + ','
        //            + '\t' + elements[i].node[5].ToString() + ',' + '\t' + elements[i].node[7].ToString() + ',' + '\t' + elements[i].node[6].ToString());
        //    }

        //    sw.Flush();
        //    sw.Close();
        //    sw.Dispose();
        //}
        //public static void writeBESO(FESystem fes, BESO_TopOpt beso, Material material, string path)
        //{
        //    List<PlanktonXYZ> nodes = fes.model.nodes;
        //    List<Element> elements = fes.model.elements;
        //    List<Load> loads = fes.loads;
        //    List<Support> supports = fes.supports;

        //    int dim = fes.model.dim;

        //    StreamWriter sw = new StreamWriter(path);
        //    sw.WriteLine("NC," + nodes.Count.ToString() + ",EC," + (elements.Count).ToString());
        //    sw.WriteLine("BESO," + beso.VF.ToString() + ',' + beso.ER.ToString() + ',' + beso.RMin.ToString() + ',' + beso.Iter.ToString());
        //    sw.WriteLine("Material," + material.E.ToString() + ',' + material.u.ToString());

        //    var ci = System.Globalization.CultureInfo.InvariantCulture;

        //    // 2D case
        //    if (dim == 2)
        //    {
        //        for (int i = 0; i < nodes.Count; i++)
        //        {
        //            var v = nodes[i];
        //            sw.WriteLine("N," + i.ToString() + ',' +
        //                v.X.ToString() + ',' +
        //                v.Y.ToString());
        //        }

        //        for (int i = 0; i < elements.Count; i++)
        //        {
        //            sw.WriteLine("E," + i.ToString() + ',' + elements[i].node[0].ToString() + ',' + elements[i].node[1].ToString() + ','
        //                + elements[i].node[2].ToString() + ',' + elements[i].node[3].ToString());
        //        }

        //        for (int i = 0; i < loads.Count; i++)
        //        {
        //            sw.WriteLine("L," + loads[i].node.ToString() + ',' + loads[i].load[0].ToString() + ',' + loads[i].load[1].ToString());
        //        }
        //        for (int i = 0; i < supports.Count; i++)
        //        {
        //            sw.WriteLine("S," + supports[i].node.ToString() + ',' + supports[i].support.ToString());
        //        }
        //    }

        //    // 3D case
        //    if (dim == 3)
        //    {
        //        for (int i = 0; i < nodes.Count; i++)
        //        {
        //            var v = nodes[i];
        //            sw.WriteLine("N," + i.ToString() + ',' +
        //                v.X.ToString() + ',' +
        //                v.Y.ToString() + ',' +
        //                v.Z.ToString());
        //        }

        //        for (int i = 0; i < elements.Count; i++)
        //        {
        //            sw.WriteLine("E," + i.ToString() + ',' + elements[i].node[0].ToString() + ',' + elements[i].node[1].ToString() + ','
        //                + elements[i].node[2].ToString() + ',' + elements[i].node[3].ToString() + ',' + elements[i].node[4].ToString() + ','
        //                + elements[i].node[5].ToString() + ',' + elements[i].node[7].ToString() + ',' + elements[i].node[6].ToString());
        //        }

        //        for (int i = 0; i < loads.Count; i++)
        //        {
        //            sw.WriteLine("L," + loads[i].node.ToString() + ',' + loads[i].load[0].ToString() + ',' + loads[i].load[1].ToString() + ','
        //                + loads[i].load[2].ToString());
        //        }
        //        for (int i = 0; i < supports.Count; i++)
        //        {
        //            sw.WriteLine("S," + supports[i].node.ToString() + ',' + supports[i].support.ToString());
        //        }
        //    }


        //    sw.Flush();
        //    sw.Close();
        //    sw.Dispose();
        //}
        //public static void writeBESO_AMR(FESystem_AMR fes, BESO_TopOpt beso, Material material, string path)
        //{
        //    List<PlanktonXYZ> centers = fes.model.centers;
        //    double[] size = fes.model.size;
        //    List<Load> loads = fes.loads;
        //    List<Support> supports = fes.supports;
        //    int dim = fes.model.dim;

        //    StreamWriter sw = new StreamWriter(path);
        //    sw.WriteLine("EC," + centers.Count.ToString());
        //    sw.WriteLine("BESO," + beso.VF.ToString() + ',' + beso.ER.ToString() + ',' + beso.RMin.ToString() + ',' + beso.Iter.ToString());
        //    sw.WriteLine("Material," + material.E.ToString() + ',' + material.u.ToString());

        //    var ci = System.Globalization.CultureInfo.InvariantCulture;

        //    // 2D case
        //    if (dim == 2)
        //    {
        //        for (int i = 0; i < centers.Count; i++)
        //        {
        //            var v = centers[i];
        //            sw.WriteLine("C," + i.ToString() + ',' +
        //                v.X.ToString() + ',' +
        //                v.Y.ToString());
        //        }

        //        sw.WriteLine("Z," + size[0].ToString() + ',' + size[1].ToString());

        //        for (int i = 0; i < loads.Count; i++)
        //        {
        //            sw.WriteLine("L," + loads[i].node.ToString() + ',' + loads[i].load[0].ToString() + ',' + loads[i].load[1].ToString());
        //        }
        //        for (int i = 0; i < supports.Count; i++)
        //        {
        //            sw.WriteLine("S," + supports[i].node.ToString() + ',' + supports[i].support.ToString());
        //        }
        //    }

        //    // 3D case
        //    if (dim == 3)
        //    {
        //        for (int i = 0; i < centers.Count; i++)
        //        {
        //            var v = centers[i];
        //            sw.WriteLine("C," + i.ToString() + ',' +
        //                v.X.ToString() + ',' +
        //                v.Y.ToString() + ',' +
        //                v.Z.ToString());
        //        }

        //        sw.WriteLine("Z," + size[0].ToString() + ',' + size[1].ToString() + ',' + size[2].ToString());

        //        for (int i = 0; i < loads.Count; i++)
        //        {
        //            sw.WriteLine("L," + loads[i].node.ToString() + ',' + loads[i].load[0].ToString() + ',' + loads[i].load[1].ToString() + ','
        //                + loads[i].load[2].ToString());
        //        }
        //        for (int i = 0; i < supports.Count; i++)
        //        {
        //            sw.WriteLine("S," + supports[i].node.ToString() + ',' + supports[i].support.ToString());
        //        }
        //    }


        //    sw.Flush();
        //    sw.Close();
        //    sw.Dispose();
        //}
        //public static List<Mesh> readVoxels(string path)
        //{
        //    List<Mesh> meshes = new List<Mesh>();
        //    if (File.Exists(path))
        //    {
        //        List<Point3f> pts = new List<Point3f>();
        //        StreamReader SR = new StreamReader(path);
        //        int num = 0;
        //        while (!SR.EndOfStream)
        //        {
        //            string line = SR.ReadLine();//Read Line

        //            if (num > 1)
        //            {

        //                string[] tokens = line.Split(',');//Divide every line as name and values

        //                if (tokens.Length == 4)
        //                {
        //                    pts.Add(new Point3f(double.Parse(tokens[1]), double.Parse(tokens[2]), double.Parse(tokens[3])));
        //                }
        //                else if (tokens.Length == 9)
        //                {
        //                    Mesh voxel = new Mesh();
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[1])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[2])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[3])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[4])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[5])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[6])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[7])]);
        //                    voxel.Vertices.Add(pts[int.Parse(tokens[8])]);

        //                    voxel.Faces.AddFace(1, 0, 3, 2);
        //                    voxel.Faces.AddFace(0, 1, 5, 4);
        //                    voxel.Faces.AddFace(1, 2, 6, 5);
        //                    voxel.Faces.AddFace(6, 2, 3, 7);
        //                    voxel.Faces.AddFace(3, 0, 4, 7);
        //                    voxel.Faces.AddFace(6, 7, 4, 5);

        //                    meshes.Add(voxel);
        //                }
        //            }
        //            num++;
        //        }
        //        SR.Close();
        //        SR.Dispose();
        //    }

        //    return meshes;
        //}
    }
}