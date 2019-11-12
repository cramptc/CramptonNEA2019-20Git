using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor;
using System.IO;
using ImageProcessor.Imaging.Filters.EdgeDetection;

namespace MazeClasses
{
    public class ImageHandler
    {
        public string _path;
        public byte[] _image;
        public string olddir;
        public string _name;

        public ImageHandler(string name) {
            _path = @"C:\Users\Chris Crampton\Documents\GitHub\CramptonNEA2019-20Git\App Resources\";
            olddir = _path+name;
            _image = File.ReadAllBytes(olddir);
            _name = name;
        }
        public void ShowEdges(byte[] img)
        {
            LaplacianOfGaussianEdgeFilter filter = new LaplacianOfGaussianEdgeFilter();
            PrewittEdgeFilter pfilter = new PrewittEdgeFilter();
            ScharrEdgeFilter sfilter = new ScharrEdgeFilter();
            RobertsCrossEdgeFilter rfilter = new RobertsCrossEdgeFilter();
            SobelEdgeFilter sobfilter = new SobelEdgeFilter();
            Laplacian5X5EdgeFilter Lfilter = new Laplacian5X5EdgeFilter();
            KirschEdgeFilter kfilter = new KirschEdgeFilter();
            List<IEdgeFilter> FilterList = new List<IEdgeFilter> { filter, pfilter, sfilter, rfilter, sobfilter, Lfilter, kfilter };
            for (int i = 0; i < 7; i++)
            {
                using (MemoryStream inStream = new MemoryStream(img))
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                        using (ImageFactory IF = new ImageFactory(preserveExifData: true))
                        {
                            IEdgeFilter currentfilter = FilterList[i];
                            IF.Load(inStream);
                            IF.DetectEdges(currentfilter, false);
                            IF.Save(outStream);
                            byte[] output = outStream.ToArray();
                            string filterstring = currentfilter.ToString();
                            string filtername = filterstring.Split('.').Last();
                            string newdir = _path + filtername+_name;
                            if (File.Exists(newdir)) { File.Delete(newdir); }
                            File.WriteAllBytes(newdir, output);
                        }
                    }
                }
            }
        }
    }
}
