using ALFE.TopOpt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ALFE
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestProgram.Test()
            string path = @"E:\\dev\\SPBESO\\data\\testiBESOVR";
            SPBESO beso = FEIO.ReadSPBESO(path, "beso");
            //BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize(true);
            //Console.WriteLine(beso.Model.Elements[0].Ke);
            Console.ReadKey();
            //FEIO.WriteIsovalues(path, beso);
        }

        //public static void VerifyScoring()
        //{
        //    string path = @"E:\ALCoding\ALFE\topoptTest\Example2\Verify";
        //    BESO beso = FEIO.ReadBESO(path, "beso");
        //    ScoringSystem scoringSystem = new ScoringSystem(beso.Path, beso.System, 5.0, 4, beso.EvolutionRate,
        //        beso.PenaltyExponent, beso.VolumeFraction, beso.MaximumIteration, beso.HardKill, Solver.PARDISO);
        //    List<double> scores = new List<double>(4) {0.9, 0.2, 0.4, 0.1 };
        //    scoringSystem.Initialize(scores);
        //    scoringSystem.Optimize();
        //    //FEIO.WriteIsovalues(path, beso);
        //}
        public static void RunVerify2D()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest\Example2\Verify";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize();
            FEIO.WriteIsovalues(path, beso);
        }
        public static void RunVerify3D()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest\Example3\Verify";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize();
            FEIO.WriteIsovalues(path, beso);
        }
        public static void RunExample3D()
        {
            string path = @"E:\ALCoding\ALFE\topoptTest\Example3";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.Optimize();
            FEIO.WriteIsovalues(path, beso);
        }
        public static void RunChair2D()
        {
            string path = @"F:\OneDrive\OneDrive - RMIT University\Work\MyPaper\Extracting smooth design from topology optimisation results using nodal sensitivity\Numberical Experiments\2D Chair\TopologyOptimisation";
            BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            Console.WriteLine(beso.Model.Elements[0].Ke);
            beso.Optimize();
            FEIO.WriteIsovalues(path, beso);
        }
        public static void Verifying()
        {
            string path = @"E:\ALCoding\Verifying\cantileverModel.msh";
            var model = FEIO.ReadTetrahedras(path);

            var supportIDs = new List<int>(458)
            {
                5,6,7,8,64,65,66,67,112,113,114,115,116,117,124,177,178,181,183,
                189,190,191,228,229,230,231,238,239,242,243,244,284,285,286,287,
                356,357,358,359,417,418,420,421,422,423,424,431,432,433,434,435,
                643,644,645,646,649,650,651,653,659,660,661,662,777,778,779,780,
                781,784,795,798,799,800,801,802,805,806,807,818,819,823,824,825,
                826,868,879,904,914,943,975,976,999,1000,1020,1026,1027,1029,1030,
                1033,1036,1039,1041,1042,1257,1259,1260,1261,1263,1274,1278,1279,
                1280,1282,1285,1390,1391,1392,1416,1418,1419,1420,1421,1422,1423,
                1448,1452,1457,1458,1459,1460,1461,1462,1463,1464,1465,1466,1467,
                1468,1469,1470,1471,1472,1473,1474,1503,1519,1520,1522,1544,1545,
                1546,1558,1559,1571,1609,1610,1622,1638,1741
            };
            //var supportIDs = new List<int>(458)
            //{5,6,7,8,283,284,285,286,1510,1515,1567,1749,1863,1955,1956,5604,5622,5856,5857,6518,6863,6865,9097,
            //    64,65,66,67,371,372,373,374,1584,1585,1629,1837,1910,1993,1994,5899,6789,7031,7032,8925,
            //    7,8,116,117,229,284,285,286,464,465,466,467,468,469,1510,1511,1512,1513,1514,1515,1516,1517,1518,1519,1574,
            //    1955,1956,2070,2071,2094,2105,2106,5604,5605,5606,5607,5608,5609,5610,5611,5612,5613,5614,5615,5616,5617,
            //    5618,5619,5620,5621,5622,5857,5862,5867,5868,5869,5870,6290,6293,6294,6300,8878,8937,9038,9097,
            //    64,65,193,194,239,240,371,372,374,1170,1174,1175,1176,1177,1178,1577,1578,1579,1580,1581,1582,1583,1584,
            //    1585,1629,1632,1779,1993,1994,4495,4496,4497,4510,4511,5886,5887,5888,5889,5890,5891,5892,5893,5894,5895,
            //    5896,5897,5898,5899,5900,5901,6143,6145,6150,6152,6163,6178,6624,6628,6789,7149,8925,9414,
            //    112,113,124,181,182,185,230,231,244,245,449,450,451,452,453,454,462,486,487,488,489,490,491,492,493,494,495,
            //    1122,1123,1124,1125,1126,1127,1128,1129,1130,1131,1146,1147,1148,1149,1150,1151,1152,1520,1521,1522,1523,1525,
            //    1531,1591,1593,1594,1595,1596,1597,1645,1646,1647,1648,1649,1650,1651,1652,1653,1654,2030,2037,2042,2045,2122,2125,
            //    2143,2159,2160,4351,4373,4374,4377,4382,4402,4414,4443,4448,4449,4453,4455,4474,5623,5625,5627,5634,5903,5904,5906,
            //    5914,6196,6199,6200,6205,6208,6209,6210,6211,6212,6213,6214,6215,6216,6217,6218,6219,6220,6221,6222,6223,6224,6225,
            //    6226,6227,6228,6229,6230,6231,6232,6233,6234,6235,6236,6237,6238,6239,6240,6241,6242,6243,6244,6245,6246,6247,6248,
            //    6249,6250,6251,6252,6253,6254,6255,6256,6257,6258,6259,6260,6261,6262,6263,6264,6265,6266,6267,6268,6269,6270,6271,
            //    6272,6273,6274,6275,6276,6277,6278,6279,6280,6281,6282,6283,6284,6285,6286,6287,6288,6289,6602,6603,6604,6607,6608,
            //    6927,7096,9032,9143,9490,9530,5,6,8,115,116,283,284,285,286,463,464,468,1510,1863,1955,1956,2082,
            //    2083,2086,2105,5604,5622,6863,6864,6865,9097,5,6,114,115,283,284,286,463,464,485,1567,1749,1750,1863,
            //    1956,2082,2083,6518,6519,6863,6864,6865,65,66,67,194,195,371,372,373,374,1177,1178,1179,1585,1910,1993,
            //    1994,4496,4517,4520,4528,5887,5899,7031,7032,7033,8925,66,67,187,195,372,373,374,1161,1178,1179,1836,1837,
            //    1910,1993,4517,4520,6792,6794,7031,7032,7033};
            var disSup = supportIDs.Distinct().ToList();
            var supports = new List<Support>(458);
            for (int i = 0; i < disSup.Count; i++)
            {
                supports.Add(new Support(disSup[i], SupportType.Fixed));
            }
            model.SetSupports(supports);
            var loads = new List<Load>(1) { new Load(128, new Vector3D(0, 0, -1)) };
            //var loads = new List<Load>(1) { new Load(569, new Vector3D(0, 0, -1)) };
            model.SetLoads(loads);

            FESystem system = new FESystem(model, Solver.SimplicialLLT);
            system.Initialize();
            system.Solve();

            Console.WriteLine(model.Elements[0].Ke);
            Console.ReadKey();
        }
        public static void VerifyHex()
        {
            //var n0 = new Node(3, 0.0, 0.0, 0.0);
            //var n1 = new Node(3, 1.0, 0.0, 0.0);
            //var n2 = new Node(3, 1.0, 1.0, 0.0);
            //var n3 = new Node(3, 0.0, 1.0, 0.0);

            //var n4 = new Node(3, 0.0, 0.0, 1.0);
            //var n5 = new Node(3, 1.0, 0.0, 1.0);
            //var n6 = new Node(3, 1.0, 1.0, 1.0);
            //var n7 = new Node(3, 0.0, 1.0, 1.0);

            //var nds = new List<Node>(8) { n0, n1, n2, n3, n4, n5, n6, n7 };
            //var el = new Hexahedron(nds, new Material());
            //el.ComputeKe();
            //Console.WriteLine(el.Ke);

            var n0 = new Node(3, 0.0, 0.0, 0.0);
            var n1 = new Node(3, 1.0, 0.0, 0.0);
            var n2 = new Node(3, 1.0, 1.0, 0.0);
            var n3 = new Node(3, 1.0, 1.0, 1.0);

            var nds = new List<Node>(4) { n0, n1, n2, n3};
            var el = new Tetrahedron(nds, new Material());
            el.ComputeKe();
            Console.WriteLine(el.Ke);
        }
    }
}
