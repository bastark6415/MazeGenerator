using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Types
{
	/// <summary>
	/// The main item of <c>Generator</c> class
	/// </summary>
	/// <remarks>
	/// Contain four boolean variables for each wall(left, up, down, right)
	/// If variable is true, it is mean that on this side <c>Cell</c> have wall.
	/// </remarks>
	public struct Cell
	{
		public bool left;
		public bool up;
		public bool down;
		public bool right;
	}
}
