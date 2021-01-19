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


            Model model2d = new Cantilever2D(ElementType.PixelElement, 1001, 1001).Model;
            FESystem.System sys = new FESystem.System(model2d, true);
            sys.Solve();
            FEPrint.PrintSystemInfo(sys);

            //BESO beso = new BESO(sys, 3.0f);
            //Console.WriteLine(model2d.Elements[0].C);
            Console.ReadKey();
        }
        public void Test()
        {
            TestMethods.TestPixels();
            string path = "box.vtk";
            var model = FEIO.ReadVTK(path);
            
        }
    }
}
