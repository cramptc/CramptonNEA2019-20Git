using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeClasses;

namespace NEAApp.App
{
    class Program
    {
        static void Main(string[] args)
        {
            ImageHandler IHA = new ImageHandler("Trial Maze1.jpg",-30,110);
            ImageHandler IHB = new ImageHandler("Trial Maze2.jpg",-20,120);
            ImageHandler IHC = new ImageHandler("Trial Maze3.jpg",-30,100);
            ImageHandler IHD = new ImageHandler("Trial Maze4.jpg",-30,90);
            ImageHandler IHE = new ImageHandler("Trial Maze5.jpg",0,90);
            List<ImageHandler> IHL = new List<ImageHandler> {IHB};
            foreach (ImageHandler IH in IHL)
            {
                IH.Unpack();
                Console.WriteLine("Ya");
                IH.LowRes(3);
                IH.Packup();
                IH.Save();
                Console.WriteLine("Done");
            }
            Console.WriteLine("Actually done");
            Console.ReadLine();
        }
    }
}
