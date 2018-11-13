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
		public Path(Path p) : this()
		{
			foreach (Point pnt in p.path)
				AddPoint(pnt);
		}
		public void AddPoint(Point p) => path.Add(p);
		public bool RemovePoint(Point p) => path.Remove(p);
		public void Clear() => path.Clear();
    }
}
