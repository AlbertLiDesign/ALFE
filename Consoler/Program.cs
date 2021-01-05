using ALFE.FEModel;
using ALFE.FESystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALFE
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start to test, please wait a few seconds...");
            //TestMethods.TestPixels(1001,1001);
            string path = "box.vtk";
            var model = FEIO.ReadVTK(path);
            Console.WriteLine(model.Nodes.Count);
            Console.WriteLine(model.Elements.Count);
            model.Elements[0].ComputeKe();
            Console.WriteLine(model.Elements[0].Ke);
            Console.ReadKey();
        }
    }
}
