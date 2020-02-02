using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using NodeClasses;

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
        string _outputpath;
        string _imagedir;
        string _bmpimagedir;
        int _dataoffset;
        int _newwidth;
        int _newheight;
        //Lower is more lenient
        int _const;
        //High is White
        int _bwthreshhold;
        List<int> _start = new List<int>();
        List<int> _stop = new List<int>();
        List<int> _cstart = new List<int>();
        List<int> _cstop = new List<int>();
        List<List<byte[]>> _packedarray = new List<List<byte[]>>();
        List<List<int>> _reds = new List<List<int>>();
        int _maxheight;
        int _minheight;
        int _maxwidth;
        int _minwidth;
        string _imagename;
        Node Begin;
        Node End;

        //Outputs a bitmap header byte array, a bitmap pixel data byte array, the height and width of the bitmap
        //Useful variable names: _height,_width,_bitmappixelarray,_bitmapheader
        public ImageHandler(string Imagename,int cons, int bw)
        {
            _bwthreshhold = bw;
            _const = cons;
            _imagename = Imagename;
            _outputpath = _path + Imagename.Split('.')[0] + "outputmaze.bmp";
            _imagedir = _path + Imagename;
            _bmpimagedir = _path + Imagename.Split('.')[0] + "++Temp.bmp";
            using (Stream _bmpStream = File.Open(_imagedir, FileMode.Open))
            {
                Image image = Image.FromStream(_bmpStream);
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
                byte[] bmpsize = new byte[4];
                bmpsize[0] = _bitmaparray[10];
                _dataoffset = 54;
                byte[] headerbuffer = new byte[_dataoffset];
                bmpStream.Position = 0;
                using (MemoryStream ms = new MemoryStream())
                {
                    int read = bmpStream.Read(headerbuffer,0,headerbuffer.Length);
                    ms.Write(headerbuffer, 0, read);
                    _bitmapheader = ms.ToArray();
                }
                //To increment the actual start of the data
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











        public void LowRes(int sf)
        {
            int consistentsw = _packedarray[0].Count;
            int consistentsh = _packedarray.Count;
            List<List<byte[]>> temppackedarray= _packedarray;
            //Iterates number of times required to remove
            for (int i = sf; i > 0;i --)
            {
                //Halves Width`
                foreach (List<byte[]> row in temppackedarray)
                {
                    for(int c = row.Count-1; c >=0; c -=2)
                    {
                        row.Remove(row[c]);
                    }
                }
                //Halves Height
                for (int c = temppackedarray.Count - 1; c >= 0; c -= 2)
                {
                    temppackedarray.Remove(temppackedarray[c]);
                }
                Console.WriteLine("Made Small");
            }
            FindEdges(temppackedarray);
            FindStartStop(temppackedarray);
            Grayscale();
            Sharpen();
            temppackedarray = Solve(temppackedarray);
            //Redify();
            //ShowEndPoints();
            //Drawbounds();
            foreach (List<byte[]> row in temppackedarray) { if (row.Count != temppackedarray[0].Count) { Console.WriteLine("Not uniform"); } }
            //Iterates number of times required to replenish
            for (int i = sf; i > 0; i--)
            {
                //Doubles Width
                for (int j = 0; j < temppackedarray.Count; j++)
                {
                    List<byte[]> temp = new List<byte[]> { };
                    foreach (byte[] pxl in temppackedarray[j]) { temp.Add(pxl); temp.Add(pxl); }
                    temppackedarray[j] = temp;
                }
                //Doubles Height
                int l = temppackedarray.Count;
                for (int j = l - 1; j >= 0; j--)
                {
                    temppackedarray.Insert(j + 1, temppackedarray[j]);
                }
                Console.WriteLine("Made Big");
            }
            _newwidth = temppackedarray[0].Count;
            _newheight = temppackedarray.Count;
            _packedarray = temppackedarray;
        }










        public void Blur(int offset)
        {
            int total=0;
            byte avg=0;
            for (int colour = 1; colour<4;colour++)
            {
                for (int row = 1+offset; row < _height - 2; row += 3)
                {
                    for (int column = 1+offset; column < _width - 2; column += 3)
                    {
                        total += _packedarray[row+1][column][colour];
                        total += _packedarray[row][column][colour];
                        total += _packedarray[row-1][column][colour];
                        total += _packedarray[row+1][column+1][colour];
                        total += _packedarray[row][column+1][colour];
                        total += _packedarray[row-1][column+1][colour];
                        total += _packedarray[row+1][column-1][colour];
                        total += _packedarray[row][column-1][colour];
                        total += _packedarray[row-1][column-1][colour];
                        avg = (byte)(total / 9);
                        _packedarray[row + 1][column][colour]=avg;
                        _packedarray[row][column][colour]=avg;
                        _packedarray[row - 1][column][colour]=avg;
                        _packedarray[row + 1][column + 1][colour]=avg;
                        _packedarray[row][column + 1][colour]=avg;
                        _packedarray[row - 1][column + 1][colour]=avg;
                        _packedarray[row + 1][column - 1][colour]=avg;
                        _packedarray[row][column - 1][colour]=avg;
                        _packedarray[row - 1][column - 1][colour]=avg;
                        total = 0;
                        avg = 0;
                    }
                }
            }
        }
        public void FindEdges(List<List<byte[]>> array)
        {
            //Each max/min parameter starts at opposite side and moves towards where its meant to be, each max/min should cross eachother
            int rowcount = 0;
            int columncount = 0;
            int minheight = _height;
            int maxheight = 0;
            int minwidth = _width;
            int maxwidth = 0;
            foreach (List<byte[]> row in array)
            {
                foreach (byte[] pxl in row)
                {
                    if ((pxl[3] > (pxl[1] + pxl[2])+_const)|| (pxl[1]>(pxl[2]+pxl[3])+_const))
                    {
                        if (rowcount > maxheight) { maxheight = rowcount; }
                        if (rowcount < minheight) { minheight = rowcount; }
                        if (columncount > maxwidth) { maxwidth = columncount; }
                        if (columncount < minwidth) { minwidth = columncount; }
                    }
                    columncount += 1;
                }
                columncount = 0;
                rowcount += 1;
            }
            _maxheight = maxheight;
            _minheight = minheight;
            _maxwidth = maxwidth;
            _minwidth = minwidth;
        }
        public void Drawbounds()
        {
            foreach (byte[] pxl in _packedarray[_maxheight])
            {
                pxl[1] = 0;
                pxl[2] = 0;
                pxl[3] = 255;
            }
            foreach (byte[] pxl in _packedarray[_minheight])
            {
                pxl[1] = 0;
                pxl[2] = 0;
                pxl[3] = 255;
            }
            for (int i = 0; i<_packedarray.Count;i++)
            {
                _packedarray[i][_minwidth][1] = 0;
                _packedarray[i][_minwidth][2] = 0;
                _packedarray[i][_minwidth][3] = 255;
                _packedarray[i][_maxwidth][1] = 0;
                _packedarray[i][_maxwidth][2] = 0;
                _packedarray[i][_maxwidth][3] = 255;
            }
        }
        public void Save()
        {
            File.Delete(_outputpath);
            _bitmapheader[19] = (byte)(_newwidth / 256);
            _bitmapheader[18] = (byte)(_newwidth % 256);
            _bitmapheader[23] = (byte)(_newheight / 256);
            _bitmapheader[22] = (byte)(_newheight % 256);
            var finalarray = _bitmapheader.Concat(_bitmappixelarray);
            var finalbytearray = finalarray.ToArray();
            File.WriteAllBytes(_outputpath, finalbytearray);
        }
        public void Unpack()
        { 
            List<byte[]> rawpixels = new List<byte[]>();
            byte[] outputstream = _bitmappixelarray.Reverse().ToArray();
			for (int count = 0; count<_bitmappixelarray.Length; count += 4)
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
                for (int width = 0; width<_width; width++)
                {
                    row.Add(rawpixels[width + (height * _width)]);
                }
                _packedarray.Add(row);
            }
        }
        public void Packup()
        {
            byte[] outputstream = new byte[_packedarray.Count * 4 * _packedarray[0].Count];
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
        public void FindStartStop(List<List<byte[]>> array)
        {
            int rowcount = 0;
            int columncount = 0;
            //array to _packedarray
            foreach (List<byte[]> row in array)
            {
                foreach (byte[] pxl in row)
                {
                    if (pxl[1] > (pxl[2] + pxl[3]+_const))
                    {
                        List<int> pxlid = new List<int> { rowcount, columncount };
                        _reds.Add(pxlid);
                    }
                    columncount += 1;
                }
                columncount = 0;
                rowcount += 1;
            }
            if (_reds.Count() > 0)
            {
                List<int> start = _reds[0];
                List<int> stop = _reds[0];
                double maxdistance = 0;
                double distance;
                foreach (List<int> location in _reds)
                {
                    distance = Math.Sqrt(Math.Pow(Math.Abs(location[0]-start[0]), 2) + Math.Pow(Math.Abs(location[1]-start[1]), 2));
                    if (distance > maxdistance)
                    {
                        maxdistance = distance;
                        stop = location;
                    }
                }
                _start = start;
                _stop = stop;
            }
            foreach (List<int> place in _reds)
            {
                _packedarray[place[0]][place[1]][1] = 255;
                _packedarray[place[0]][place[1]][2] = 255;
                _packedarray[place[0]][place[1]][3] = 255;
            }
        }
        public void ShowEndPoints ()
        {
            int rowcount = 0;
            foreach (List<byte[]> row in _packedarray)
            {
                if (rowcount == _start[0] | rowcount == _stop[0])
                    foreach (byte[] pxl in row)
                    {
                        pxl[1] = 0;
                        pxl[2] = 255;
                        pxl[3] = 0;
                    }
                row[_start[1]][1] = 0;
                row[_start[1]][2] = 255;
                row[_start[1]][3] = 0;
                row[_stop[1]][1] = 0;
                row[_stop[1]][2] = 255;
                row[_stop[1]][3] = 0;
                rowcount++;
            }
        }
        public void Redify()
        { 
            foreach (List<int> place in _reds)
            {
                _packedarray[place[0]][place[1]][1] = 255;
                _packedarray[place[0]][place[1]][2] = 0;
                _packedarray[place[0]][place[1]][3] = 0;
            }
        }

        public void Sharpen()
        {
            foreach (List<byte[]> row in _packedarray)
            {
                foreach (byte[] pxl in row)
                {
                    if (pxl[1] > _bwthreshhold)
                    {
                        pxl[1] = 255;
                        pxl[2] = 255;
                        pxl[3] = 255;
                    }
                    else
                    {
                        pxl[1] = 0;
                        pxl[2] = 0;
                        pxl[3] = 0;
                    }
                }
            }
        }
        public int HCost(int[] place, List<int> stop)
        {
            int H = 0;
            int deltax = Math.Abs(place[1] - stop[1]);
            int deltay = Math.Abs(place[0] - stop[0]);
            if (deltax > deltay) { H += (deltax - deltay) * 10; H += (deltay * 14); }
            if (deltax < deltay) { H += (deltay - deltax) * 10; H += (deltax * 14); }
            else { H += deltay * 14; }
            return H;
        }




        public List<List<byte[]>> Solve(List<List<byte[]>> input)
        {
            //Assemble list of nodes
            List<Node> Unchecked = new List<Node> { };
            List<Node> Viable = new List<Node> { };
            List<Node> Checked = new List<Node> { };
            IDictionary<int[],Node> NodeLocations = new Dictionary<int[],Node>(new CheckEquality()) { };
            int counter = 0;
            foreach (List<byte[]> row in input)
            {
                if (input.IndexOf(row) < _maxheight | input.IndexOf(row) > _minwidth)
                {
                    foreach (byte[] pxl in row)
                    {
                        if (row.IndexOf(pxl) < _maxwidth | row.IndexOf(pxl) > _minwidth)
                        {
                            int[] location = new int[2] { input.IndexOf(row), row.IndexOf(pxl) };
                            if (location[0] == _start[0] && location[1] == _start[1])
                            {
                                Begin = new Node(location, HCost(location, _stop), pxl[1] + pxl[2] + pxl[3],counter);
                                Console.WriteLine("Found Start");
                                Begin._G = 0;
                                Begin.GenerateF();
                                Viable.Add(Begin);
                            }
                            if (location[0] == _stop[0] && location[1] == _stop[1])
                            {
                                End = new Node(location, HCost(location, _stop), pxl[1] + pxl[2] + pxl[3],counter);
                                    Console.WriteLine("Found End");
                            }
                            Node tempnode = new Node(location, HCost(location, _stop), pxl[1] + pxl[2] + pxl[3],counter);
                            Unchecked.Add(tempnode);
                            NodeLocations.Add(location,tempnode);
                        }
                        counter++;
                    }
                }
            }
            input[Begin._y][Begin._x][1] = 150;
            input[Begin._y][Begin._x][2] = 150;
            input[Begin._y][Begin._x][3] = 150;
            input[End._y][End._x][1] = 150;
            input[End._y][End._x][2] = 150;
            input[End._y][End._x][3] = 150;
            Console.WriteLine("Declared All the Nodes");
            Node current = Begin;
            int c = 0;
            int found = 0;
            int colour = 0;
            while (found == 0)
            {
                Viable.Sort();
                if (Viable.Count == 0) { Console.WriteLine("Viable empty abort"); break; }
                current = Viable[0];
                if (current.Equals(End)) { Console.WriteLine("I've found it using equals"); found = 1; break; }
                if (current._x == End._x && current._y == End._y) { Console.WriteLine("I've found it"); found = 1; break; }
                //Generate Neighbours
                List<Node> neighbours = new List<Node> { };
                int x = current._x;
                int y = current._y;
                if (x != _minwidth)
                { 
                    Node newn = NodeLocations[new int[2]{ y, x - 1 }];
                    newn.distance = 10;
                    if (newn._canwalk)
                    { neighbours.Add(newn); }
                }
                if (x != _maxwidth)
                {
                    Node newn = NodeLocations[new int[2]{ y, x + 1 }];
                    newn.distance = 10;
                    if (newn._canwalk)
                    { neighbours.Add(newn); }
                }
                if (y != _minheight)
                {
                    Node newn = NodeLocations[new int[2]{ y - 1, x }];
                    newn.distance = 10;
                    if (newn._canwalk)
                    { neighbours.Add(newn); }
                }
                if (y != _maxheight)
                {
                    Node newn = NodeLocations[new int[2] { y + 1, x }];
                    newn.distance = 10;
                    if (newn._canwalk)
                    { neighbours.Add(newn); }
                }

                if (x != _minwidth && y != _minheight)
                {
                    Node newn = NodeLocations[new int[2]{ y - 1, x - 1 }];
                    newn.distance = 14;
                    if (newn._canwalk)
                    { neighbours.Add(newn); }
                }
                if (x != _minwidth && y != _maxheight)
                {
                    Node newn = NodeLocations[new int[2] { y + 1, x - 1 }];
                    newn.distance = 14;
                    if (newn._canwalk)
                    { neighbours.Add(newn); }
                }
                if (x != _maxwidth && y != _maxheight)
                {
                    Node newn = NodeLocations[new int[2]{ y + 1, x + 1 }];
                    newn.distance = 14;
                    if (newn._canwalk) { neighbours.Add(newn); }
                }
                if (x != _maxwidth && y != _minheight)
                {
                    Node newn = NodeLocations[new int[2]{ y - 1, x + 1 }];
                    newn.distance = 14;
                    if (newn._canwalk) { neighbours.Add(newn); }
                }
                foreach (Node n in neighbours)
                {
                    if (n == End) { break; }
                    if (Checked.Contains(n)) { continue; }
                    if (n._canwalk != true) { Checked.Add(n); continue; }
                    if (n._G == null)
                    {
                        n.G = current.G + n.distance;
                        n._Parent = current;
                    }
                    if (n.G > (current.G + n.distance))
                    {
                        n.G = current.G + n.distance;
                        n._Parent = current;
                    }
                    if (Viable.Contains(n) == false) { Viable.Add(n); }
                }
                //input[current._y][current._x][3] = (byte)colour;
                Viable.Remove(current);
                Checked.Add(current);
                c++;
                if (c % 300==0) { colour += 1; }
                if (c % 1000 == 0) { Console.WriteLine(c);Console.WriteLine(current._H + " " + current._F + " " + current._G + " " +Viable.Count); }
                if (c % 75000 == 0) { break; }
            }
            //if (Viable.Count > 0)
            //{
            //    foreach (Node n in Viable)
            //    {
            //        input[n._y][n._x][1] = 255;
            //        input[n._y][n._x][2] = 0;
            //        input[n._y][n._x][3] = 255;
            //    }
            //}
            List<int[]> Path = new List<int[]> { };
            while (current._Parent != null)
            {
                Path.Add(current._location);
                current = current._Parent;
            }
            Console.WriteLine(Path.Count);
            foreach (int[] i in Path)
            {
                int x = i[1];
                int y = i[0];
                input[y][x][3] = 0;
                input[y][x][2] = 0;
                input[y][x][1] = 255;

            }
            return input;
        }
    }
}
