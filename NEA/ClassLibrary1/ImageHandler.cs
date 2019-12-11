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
        public int _white;
        public int _black;
        int _bwthreshhold = 100;
        List<List<byte[]>> _packedarray = new List<List<byte[]>>();

        //Outputs a bitmap header byte array, a bitmap pixel data byte array, the height and width of the bitmap
        //Useful variable names: _height,_width,_bitmappixelarray,_bitmapheader
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
        public void Save()
        {
            File.Delete(_outputpath);
            var finalarray = _bitmapheader.Concat(_bitmappixelarray);
            var finalbytearray = finalarray.ToArray();
            File.WriteAllBytes(_outputpath, finalbytearray);
        }
        public void Unpack()
        {
            List<byte[]> rawpixels = new List<byte[]>();
            byte[] outputstream = _bitmappixelarray.Reverse().ToArray();
            for (int count = 0; count < _bitmappixelarray.Length; count += 4)
            {
                    byte[] chunk = new byte[4];
                    chunk[0] = (outputstream[count]);
                    chunk[1] = outputstream[count + 1];
                    chunk[2] = outputstream[count + 2];
                    chunk[3] = outputstream[count + 3];
                    rawpixels.Add(chunk);
            }
            for (int height = 0; height<_height;height++)
            {
                List<byte[]> row = new List<byte[]>();
                for (int width = 0; width < _width; width++)
                {
                    row.Add(rawpixels[width + (height * _width)]);
                }
                _packedarray.Add(row);
            }
        }
        public void Packup()
        {
            byte[] outputstream = new byte[_bitmappixelarray.Length];
            int count = 0;
            foreach (List<byte[]> row in _packedarray)
            {
                foreach (byte[] pixel in row)
                { 
                outputstream.SetValue(pixel[0], 0 + count);
                outputstream.SetValue(pixel[1], 1 + count);
                outputstream.SetValue(pixel[2], 2 + count);
                outputstream.SetValue(pixel[3], 3 + count);
                count += 4;
                }
            }
            _bitmappixelarray = outputstream.Reverse().ToArray();
        }
        public void Grayscale()
        {
            foreach (List<byte[]> row in _packedarray)
            { 
                foreach (byte[] pxl in row)
                {
                    double Cred = pxl[1]/2.55;
                    if (Cred> 4.045) { Cred = Math.Pow((Cred + 5.5) / 105.5, 2.4); }
                    else { Cred = Cred / 1292; }
                    double Cgreen = pxl[2] / 2.55;
                    if (Cgreen > 0.04045) { Cgreen = Math.Pow((Cgreen + 5.5) / 105.5, 2.4); }
                    else { Cgreen = Cgreen / 1292; }
                    double Cblue = pxl[3]/2.55;
                    if (Cblue > 0.04045) { Cblue = Math.Pow((Cblue + 5.5) / 105.5, 2.4); }
                    else { Cblue = Cblue / 1292; }
                    double final = 0.2126*Cred + 0.7152*Cgreen + 0.0722*Cblue;
                    if (final>0.0031308) { final = Math.Pow(final * 1.055,1/2.4)-0.055; }
                    else { final = final * 12.92; }
                    byte grayscale = (byte)Math.Round(final*255);
                    pxl[1] = grayscale;
                    pxl[2] = grayscale;
                    pxl[3] = grayscale;
                }
            }
        }

        public void Sharpen()
        {
            int white = 0;
            int black = 0;
            foreach (List<byte[]> row in _packedarray)
            {
                foreach (byte[] pxl in row)
                {
                    if (pxl[1] > _bwthreshhold)
                    {
                        white += 1;
                        pxl[1] = 255;
                        pxl[2] = 255;
                        pxl[3] = 255;
                    }
                    else
                    {
                        black += 1;
                        pxl[1] = 0;
                        pxl[2] = 0;
                        pxl[3] = 0;
                    }
                }
            }
            _white = white;
            _black = black;
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
