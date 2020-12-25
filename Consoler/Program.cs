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
            string path = @"C:\Users\alber\Desktop\mat.txt";
            Model2D model2d = new Cantilever2D().Model;

            var Ke = model2d.ComputeUniformK();
            //FEPrint.PrintMatrix(Ke);

            System2D sys = new System2D(model2d);
            sys.AssembleKG(Ke);
            sys.Solve();
            FEPrint.PrintDisplacement(sys);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
