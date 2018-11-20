using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
	/// <summary>
	/// Class <c>Point</c> is a point (y;x) in maze"
	/// </summary>
	/// <remarks>
	///	Creates a new <c>Point</c> structure
	/// </remarks>
    public struct Point
    {
		public int x, y;
		/// <summary>
		///   Create a new <c>Point</c> from coordinates (<paramref name="x", <paramref name="y">).
		/// </summary>
		/// <param name="x">An int Value</param>
		/// <param name="y">An int Value</param>
		public Point(int x, int y) { this.x = x; this.y = y; }
		/// <summary>
		/// Comparison of two points.
		/// </summary>
		/// <param name="p1">A point</param>
		/// <param name="p2">A point</param>
		/// <returns>True if coordinates are equal else false.</returns>
		public static bool operator ==(Point p1, Point p2) => p1.x == p2.x && p1.y == p2.y;
		/// <summary>
		/// Comparison of two points.
		/// </summary>
		/// <param name="p1">A point</param>
		/// <param name="p2">A point</param>
		/// <returns>False if coordinates are equal else true</returns>
		public static bool operator !=(Point p1, Point p2) => !(p1 == p2);
    }
}
