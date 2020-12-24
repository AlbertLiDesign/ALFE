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

            System2D sys = new System2D(model2d);
            sys.AssembleKG(Ke);
            sys.Solve();

            FEPrint.PrintMatrix(sys.X,sys.Dim,sys.Dof);
            //FEIO.writeCooMatrix(sys.KG, path);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
