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
                Console.WriteLine("Edit Main.cs string name to decide which picture to use");
                string name = "Edge Testing Flash.jpg";
                ImageHandler IH = new ImageHandler(name);
                IH.ShowEdges(IH._image);
                Console.WriteLine("Finished");
        }
    }
}
