using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
    public class Path
    {
		public Stack<Point> path { get; private set; }
		public Path()
		{
			path = new Stack<Point>();
		}
		public Path(Path p) : this()
		{
			foreach (Point pnt in p.path)
				AddPoint(pnt);
		}
		public void AddPoint(Point p) => path.Push(p);
		public Point RemovePoint() => path.Pop();
		public void Clear() => path.Clear();
    }
}
