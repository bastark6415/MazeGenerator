using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
    public struct Point
    {
		public ushort x, y;
		public Point(ushort x, ushort y) { this.x = x; this.y = y; }
    }
}
