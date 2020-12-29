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
            Model2D model2d = new Cantilever2D().Model;
            System2D sys = new System2D(model2d, true);

            sys.Solve();
            var KG = sys.GetKG();
            FEPrint.PrintDisplacement(sys);
            //var disp = sys.GetDisplacement();
            //FEPrint.PrintCSR(KG);
            //FEIO.WriteCOOMatrix(KG.ToCOO(), "C:/Users/alber/Desktop/matA.mtx");

            sys.PrintTime();
            //FEPrint.PrintDisplacement(sys);

            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
