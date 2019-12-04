using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace MazeClasses
{
    public class ImageHandler
    {
        static string _path = @"C:\Users\Chris Crampton\Documents\GitHub\CramptonNEA2019-20Git\App Resources\";
        Bitmap _mazebtmp;
        int _height;
        int _width;
        int _bytelength;
        public byte[] _bitmaparray;
        public byte[] _bitmappixelarray;
        public byte[] _bitmapheader;
        string _outputpath = _path + "outputmaze.bmp";
        string _imagedir;
        string _bmpimagedir;
        byte _dataoffset;
        public ImageHandler(string Imagename)
        {
            _imagedir = _path + Imagename;
            _bmpimagedir = _path + "Output.bmp";
            using (Stream bmpStream = File.Open(_imagedir, FileMode.Open))
            {
                Image image = Image.FromStream(bmpStream);
                _mazebtmp = new Bitmap(image);
                _height = _mazebtmp.Height;
                _width = _mazebtmp.Width;
                _mazebtmp.Save(_bmpimagedir,ImageFormat.Bmp);
            }
            using (Stream bmpStream = File.Open(_bmpimagedir, FileMode.Open))
            {
                byte[] buffer = new byte[16 * 1024];
                bmpStream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = bmpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }
                    _bitmaparray = ms.ToArray();
                }
                _bytelength = _bitmaparray.Length;
                _dataoffset = _bitmaparray[10];
                byte[] headerbuffer = new byte[_dataoffset];
                bmpStream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    int read = bmpStream.Read(headerbuffer,0,headerbuffer.Length);
                    ms.Write(headerbuffer, 0, read);
                    _bitmapheader = ms.ToArray();
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    int reada;
                    while ((reada = bmpStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, reada);
                    }
                    _bitmappixelarray = ms.ToArray();
                }
            }

        }
        public void check()
        {
            byte[] outputstream = _bitmappixelarray.Reverse().ToArray();
            for (int count = 0; count < outputstream.Length; count +=4)
            {
                outputstream[count] = (byte)(255-(int)outputstream[count]);
                outputstream[count + 1] = (byte)(255- outputstream[count+1]);
                outputstream[count + 2] = (byte)(255- outputstream[count+2]);
                outputstream[count + 3] = (byte)(255- outputstream[count+3]);
            }
            File.Delete(_outputpath);
            var finalarray = _bitmapheader.Concat(outputstream.Reverse().ToArray());
            var finalbytearray = finalarray.ToArray();
            File.WriteAllBytes(_outputpath, finalbytearray);     
        } 
        public List<int> FindStartStop()
        {
            using (Stream bmpStream = File.Open(_imagedir, FileMode.Open))
            {

            }
            return new List<int>{ };
        }
    }
}
