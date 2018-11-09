using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
    public class Path
    {
		public List<Point> path { get; private set; }
		public Path()
		{
			path = new List<Point>();
		}
		public void AddPoint(Point p) => path.Add(p);
		public void Clear() => path.Clear();
		public void Reverse() => path.Reverse();
    }
}
