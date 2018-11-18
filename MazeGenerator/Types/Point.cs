using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
    public struct Point
    {
		public int x, y;
		public Point(int x, int y) { this.x = x; this.y = y; }
		public static bool operator ==(Point p1, Point p2) => p1.x == p2.x && p1.y == p2.y;
		public static bool operator !=(Point p1, Point p2) => !(p1 == p2);
    }
}
