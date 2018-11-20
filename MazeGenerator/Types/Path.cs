using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
	/// <summary>
	/// Class <c>Path</c> is the path in maze.
	/// </summary>
	/// <remarks>
	/// Class contains a LinkedList of <c>Point</c> inside.
	/// Points one by one means path in maze.
	/// We can add or remove last point.
	/// Either we can check if the path contains <c>Point</c>.
	/// Or we can clear path (delete all points).
	/// </remarks>
    public class Path
    {
		/// <value>
		///	 <c>Linked List</c> contains points of path inside.
		/// </value>
		public LinkedList<Point> path { get; private set; }
		/// <summary>
		/// Constructor of <c>Path</c>.
		/// </summary>
		public Path()
		{
			path = new LinkedList<Point>();
		}
		/// <summary>
		/// Copy constructor of <c>Path</c>.
		/// </summary>
		/// <remarks>
		/// Creates new path from <paramref name="p"/> path.
		/// </remarks>
		/// <param name="p">An another path.</param>
		/// <exception cref="System.ArgumentNullException">Throws if <paramref name="p"/> is null.</exception>
		public Path(Path p)
		{
			path = new LinkedList<Point>(p?.path);
		}
		/// <summary>
		/// Adds new point to path.
		/// </summary>
		/// <remarks>
		/// Adds new point <paramref name="p"/> in the end of path.
		///	</remarks>
		/// <param name="p">Point that needed to add.</param>
		public void AddLast(Point p) => path.AddLast(p);
		/// <summary>
		/// Removes last point from path.
		/// </summary>
		/// <remarks>
		///	Removes the last point from the path.
		///	</remarks>
		///	<exception cref="System.ArgumentNullException"></exception>
		public void RemoveLast() => path.RemoveLast();
		/// <summary>
		/// Check if point <paramref name="p"/> belongs to path.
		/// </summary>
		/// <remarks>
		/// <paramref name="p"/> compared for each point in path.
		/// </remarks>
		/// <param name="p">Point that should be search.</param>
		/// <returns>Return true if path contains<paramref name="p"/> </returns>
		public bool ContainsPoint(Point p) => path.Contains(p);
		/// <summary>
		/// Clear.
		/// </summary>
		/// <remarks>
		/// Removes all points from path.
		/// </remarks>
		public void Clear() => path.Clear();
    }
}
