using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
    public class Path
    {
		public LinkedList<Point> path { get; private set; }
		public Path()
		{
			path = new LinkedList<Point>();
		}
		public Path(Path p)
		{
			path = new LinkedList<Point>(p.path);
		}
		public void AddLast(Point p) => path.AddLast(p);
		public void RemoveLast() => path.RemoveLast();
		public bool ContainsPoint(Point p) => path.Contains(p);
		public void Clear() => path.Clear();
    }
}
