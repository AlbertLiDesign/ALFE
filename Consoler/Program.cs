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
            Model2D model2d = new Cantilever2D(200,200).Model;
            System2D sys = new System2D(model2d, true);

            sys.Solve();
            //FEPrint.PrintDisplacement(sys);
            //var disp = sys.GetDisplacement();
            //FEPrint.PrintCSR(KG);
            //FEIO.WriteCOOMatrix(KG.ToCOO(), "C:/Users/alber/Desktop/matA.mtx");

            Console.WriteLine(sys.GetDisplacement()[9999, 1]);
            FEPrint.PrintSystemInfo(sys);
            //FEPrint.PrintDisplacement(sys);

            Console.ReadKey();
        }
    }
}
