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
            ImageHandler IH = new ImageHandler("Trial Maze.jpg");
            Console.WriteLine("Initiated");
            IH.Unpack();
            Console.WriteLine("Unpacked");
            IH.Grayscale();
            Console.WriteLine("Grayscaled");
            IH.Sharpen();
            Console.WriteLine("Sharpened");
            IH.Packup();
            Console.WriteLine("Packed");
            IH.Save();
            Console.WriteLine(IH._white);
            Console.WriteLine(IH._black);
            Console.ReadLine();
            //foreach (int i in IH._bitmaparray) { Console.WriteLine(i.ToString()); }
        }
    }
}
