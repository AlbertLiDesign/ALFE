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
            Model2D model2d = new Cantilever2D().Model;

            var Ke = model2d.ComputeUniformK();

            Console.WriteLine(Ke);
            Console.ReadKey();
        }
    }
}
