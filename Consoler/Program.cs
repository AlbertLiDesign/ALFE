using ALFE.FEModel;
using ALFE.FESystem;
using ALFE.TopOpt;
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
            //TestMethods.TestPixels();
            //string path = "box.vtk";
            //var model = FEIO.ReadVTK(path);

            Model2D model2d = new Cantilever2D(ElementType.PixelElement).Model;
            System2D sys = new System2D(model2d, true);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);

            BESO beso = new BESO(sys);
            beso.CalSensitivity();
            Console.WriteLine(model2d.Elements[0].C);
            Console.ReadKey();
        }
    }
}
