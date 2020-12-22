using ALFE.FEModel;
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
            string path = @"C:\Users\alber\Desktop\mat.txt";
            Model2D model2d = new Cantilever2D().Model;

            var Ke = model2d.ComputeUniformK();

            FESystem.System2D sys = new FESystem.System2D(model2d, Ke);
            FEIO.writeCooMatrix(sys.KG, path);
            Console.WriteLine("Successful output!");
            Console.ReadKey();
        }
    }
}
