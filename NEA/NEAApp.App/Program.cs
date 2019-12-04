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
            IH.check();
            //foreach (int i in IH._bitmaparray) { Console.WriteLine(i.ToString()); }
        }
    }
}
