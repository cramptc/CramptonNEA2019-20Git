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
            //ImageHandler IHA = new ImageHandler("Edge Testing Average Light.jpg");
            //ImageHandler IHB = new ImageHandler("Edge Testing Low Light.jpg");
            //ImageHandler IHC = new ImageHandler("Edge Testing Flash.jpg");
            ImageHandler IHD = new ImageHandler("Trial Maze1.jpg");
            List<ImageHandler> IHL = new List<ImageHandler> {IHD};
            foreach (ImageHandler IH in IHL)
            {
                IH.Unpack();
                Console.WriteLine("Ya");
                IH.FindStartStop();
                IH.FindEdges();
                IH.Grayscale();
                IH.Sharpen();
                IH.Redify();
                IH.LowRes(3);
                IH.ShowEndPoints();
                IH.Drawbounds();
                IH.Packup();
                IH.Save();
                Console.WriteLine("Done");
            }
            Console.WriteLine("Actually done");
            Console.ReadLine();
        }
    }
}
