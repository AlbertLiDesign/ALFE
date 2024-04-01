using ALFE.TopOpt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ALFE
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestProgram.Test()
            //string path = @"F:\\Teaching\\TO_animation";
            string path = @"E:\dev\ALCoding\ALFE\topoptTest\cantilever_gpu";
            //string path = @"F:\OneDrive - RMIT University\Work\AResearch\SPBESO_VR\figs\fig4\DrawnPattern";
            BESO beso = FEIO.ReadBESO(path, "beso");
            //BESO beso = FEIO.ReadBESO(path, "beso");
            beso.Initialize();
            beso.RunTopOpt();
            //Console.WriteLine(beso.Model.Elements[0].Ke);
            Console.ReadKey();
            //FEIO.WriteIsovalues(path, beso);
        }
    }
}
