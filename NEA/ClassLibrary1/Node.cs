using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// F = sum of two costs, G is distance from start, H is distance from End
namespace NodeClasses
{
    class Node : IComparable<Node>
    {
        public int? _G;
        public int? _F;
        public int ID;
        public int _H;
        public int _x;
        public int _y;
        public Node _Parent;
        public bool _canwalk;
        public int distance;
        public int[] _location;
        public Node(int[] xy, int H, int sum, int id)
        {
            _H = H;
            ID = id;
            _location = xy;
            _x = _location[1];
            _y = _location[0];
            if (sum == 3 * 255) { _canwalk = true; }
            else { _canwalk = false; }
        }
        public override string ToString()
        {

            return ID.ToString();
        }
        public int G
        {
            get{ return _G.Value; }
            set{ _G = value; _F = _G + _H; }
        }

        public int CompareTo(Node other)
        {
            if (this._F == null && other._F == null) { return 0; }
            if (this._F == null) { return 1; }
            if (other._F == null) { return 0; }
            if (this._F.Value > other._F.Value) { return 1; }
            if (this._F.Value < other._F.Value) { return -1; }
            if (this._F.Value == other._F.Value) { return 0; }
            else { return 0; }
        }

        public void GenerateF() { _F = _G + _H; }
        public bool Equals(Node obj)
        {
            if (this._x == obj._x && this._y == obj._y) { return true; }
            return false;
        }
    }

    public class CheckEquality : IEqualityComparer<int[]>
    {
        public bool Equals(int[] x, int[] y)
        {
            for (int i = 0; i<x.Length;i++) { if (x[i] != y[i]) { return false; } }
                return true;
        }

        public int GetHashCode(int[] obj)
        {
            int hash = 17;
            for (int i = 0; i < obj.Length; i++)
            {
                unchecked
                {
                    hash = hash * 23 + obj[i];
                }
            }
            return hash;
        }
    }
}
