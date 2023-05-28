using System;
using System.Collections.Generic;
using System.Linq;

namespace ALFE
{
    public class Utils
    {
        public static List<double> PowerTransformation(List<double> data, double a)
        {
            return data.Select(x => Math.Pow(x, a)).ToList();
        }
        public static List<double> SigmoidTransformation(List<double> data)
        {
            return data.Select(x => 1 / (1 + Math.Exp(-x))).ToList();
        }
        public static List<double> LogTransformation(List<double> data)
        {
            return data.Select(x => Math.Log(x + 1, 2)).ToList();
        }
        public static List<double> Z_Score_Normalization(List<double> data)
        {
            double mean = data.Average(); // 计算均值

            double sumOfSquaresOfDifferences = data.Select(val => (val - mean) * (val - mean)).Sum();
            double sd = Math.Sqrt(sumOfSquaresOfDifferences / data.Count); // 计算标准差

            // 使用Z-score进行标准化
            return data.Select(x => (x - mean) / sd).ToList();
        }
        public static List<double> Min_Max_Normalization(List<double> data)
        {
            double min = data.Min(); // 计算最小值
            double max = data.Max(); // 计算最大值

            // 使用最小-最大规范化
            return data.Select(x => (x - min) / (max - min)).ToList();
        }
        public static List<double> Min_Max_Normalization(List<double> data, double max, double min)
        {
            double a, b = 0;
            if (max < min) { b = min; a = max; }
            else { b = max; a = min; }

            double data_min = data.Min(); // 计算最小值
            double data_max = data.Max(); // 计算最大值

            // 使用最小-最大规范化
            return data.Select(x => (b - a) * (x - data_min) / (data_max - data_min) + a).ToList();
        }
        /// <summary>
        /// Return a scan dictionary <index, difference>
        /// </summary>
        /// <param name="nodesNum"></param>
        /// <param name="fixedNodesID"></param>
        /// <returns></returns>
        public static Dictionary<int, int[]> Scan(int nodesNum, List<int> fixedNodesID)
        {
            fixedNodesID.Sort();

            var scan = new Dictionary<int, int[]>();
            int t = 0, v = 0;

            for (int i = 0; i < nodesNum; i++)
            {
                for (int j = 0; j < fixedNodesID.Count - v; j++)
                {
                    if (i >= fixedNodesID[j + v])
                    {
                        t = j + v;
                        v += 1;
                        break;
                    }
                }
                if (i == fixedNodesID[t])
                    scan.Add(i, new int[2] { v, 0 });
                else
                    scan.Add(i, new int[2] { v, 1 });
            }
            return scan;
        }
    }
}
