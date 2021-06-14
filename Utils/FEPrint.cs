using ALFE.TopOpt;
using System;
using System.Collections.Generic;
using System.Management;

namespace ALFE
{
    public class FEPrint
    {
        public static void PrintPreprocessing(BESO beso)
        {
            PrintDeviceInfo();
            PrintModelInfo(beso.System);
            PrintMatrixInfo(beso.System.GetKG());

            Console.WriteLine("------------------- Time Cost -------------------");
            Console.WriteLine("Computing Ke: " + beso.System.TimeCost[0].ToString() + " ms");
            Console.WriteLine("Initializing KG: " + beso.System.TimeCost[1].ToString() + " ms");
        }
        public static void PrintBESOInfo(BESO beso, int iter, double gse, double vf, List<double> timeCost)
        {
            Console.WriteLine("################### Step: " + iter.ToString() + " #####################");
            Console.WriteLine("Compliance: " + gse.ToString());
            Console.WriteLine("Volume: " + vf.ToString());

            Console.WriteLine("------------------- Time Cost -------------------");
            Console.WriteLine("Assembling KG: " + timeCost[0].ToString() + " ms");
            Console.WriteLine("Solving: " + timeCost[1].ToString() + " ms");
            Console.WriteLine("Computing Sensitivity: " + timeCost[2].ToString() + " ms");
            Console.WriteLine("Fltering Sensitivity: " + timeCost[3].ToString() + " ms");
            Console.WriteLine("Marking Elements: " + timeCost[4].ToString() + " ms");
            Console.WriteLine("Checking Convergence: " + timeCost[5].ToString() + " ms");

            Console.WriteLine();
        }
        public static void PrintSystemInfo(FESystem sys)
        {
            Console.Write(sys.Model.ModelInfo());
            Console.Write(sys.MatrixInfo());
            Console.Write(sys.SolvingInfo());
            Console.WriteLine();
        }
        public static void PrintDeviceInfo()
        {
            Console.WriteLine("------------------- Device Info -------------------");
            Console.Write("CPU: ");
            GetComponent("Win32_Processor", "Name");
            int coreCount = 0;
            foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
            {
                coreCount += int.Parse(item["NumberOfCores"].ToString());
            }
            Console.WriteLine("Number Of Cores: {0}", coreCount);
        }
        public static void PrintModelInfo(FESystem sys)
        {
            Console.WriteLine("------------------- Model Info -------------------");
            Console.WriteLine("Nodes: " + sys.Model.Nodes.Count.ToString());
            Console.WriteLine("Elements: " + sys.Model.Elements.Count.ToString());
            Console.WriteLine("Type: " + sys.Model.Elements[0].Type.ToString());
        }
        public static void PrintCOO(COOMatrix coo)
        {
            for (int i = 0; i < coo.NNZ; i++)
            {
                Console.WriteLine(coo.RowArray[i].ToString() + '\t'
                        + coo.ColArray[i].ToString() + '\t'
                        + coo.ValueArray[i].ToString());
            }
        }
        public static void PrintMatrixInfo(CSRMatrix csr)
        {
            Console.WriteLine("------------------- Matrix Info -------------------");
            Console.WriteLine("Rows: " + csr.N.ToString());
            Console.WriteLine("Cols: " + csr.N.ToString());
            Console.WriteLine("NNZ: " + csr.NNZ.ToString());
        }
        public static void PrintMatrixInfo(COOMatrix coo)
        {
            Console.WriteLine("------------------- Matrix Info -------------------");
            Console.WriteLine("Rows: " + coo.Rows.ToString());
            Console.WriteLine("Cols: " + coo.Cols.ToString());
            Console.WriteLine("NNZ: " + coo.NNZ.ToString());
        }
        public static void PrintTimeCost(List<double> timeCost)
        {
            Console.WriteLine("------------------- Time Cost -------------------");
            Console.WriteLine("Computing Ke: " + timeCost[0].ToString() + " ms");
            Console.WriteLine("Initializing KG: " + timeCost[1].ToString() + " ms");
            Console.WriteLine("Assembling KG: " + timeCost[2].ToString() + " ms");
            Console.WriteLine("Solving: " + timeCost[3].ToString() + " ms");
        }
        public static void PrintDisplacement(FESystem sys, int i)
        {
            Console.WriteLine("Node [" + i.ToString() + "] Displement is " + sys.Model.Nodes[i].Displacement.X.ToString() +
                              '\t' + sys.Model.Nodes[i].Displacement.Y.ToString() +
                              '\t' + sys.Model.Nodes[i].Displacement.Z.ToString());
        }
        public static void PrintDisplacement(FESystem sys)
        {
            foreach (var item in sys.Model.Nodes)
            {
                Console.WriteLine(item.Displacement.X.ToString() + '\t'
                  + item.Displacement.Y.ToString());
            }
        }
        private static void GetComponent(string hwclass, string syntax)
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM " + hwclass);
            foreach (ManagementObject mj in mos.Get())
            {
                if (Convert.ToString(mj[syntax]) != "")
                    Console.WriteLine(Convert.ToString(mj[syntax]));
            }
        }
    }
}
