using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Types;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Threading;

namespace MazeGenerator.Generate
{
	/// <summary>
	/// <c>Generator</c> is a base abstract class.
	/// </summary>
	/// <remarks>
	/// <para>This <c>Generator</c> has a map of maze and two points (start, finish).</para>
	/// <para>
	///	This class doesn't have an implementation Generate.
	///	When you inherit the <c>Generator</c> you have to impletment Generate mehod,
	///	to prevent new class become abstrack
	///	</para>
	/// </remarks>
	public abstract class Generator
	{
		/// <summary>
		/// A maximum of maze size
		/// </summary>
		private const int maxDimension = 512;
		/// <summary>
		/// All possible diractions in maze
		/// </summary>
		protected enum Direction { left, up, right, down}
		/// <value>
		///		Gets mapMatrix
		/// </value>
		/// <remarks>
		///	mapMatrix is a two-dimansional array of maze
		/// </remarks>
		public Cell[,] mapMatrix { get; protected set; }
		/// <value>
		///		Gets start point
		/// </value>
		/// <remarks>
		///	start point has a point of maze enter
		/// </remarks>
		public Point start { get; protected set; }
		/// <value>
		///		Gets finish point
		/// </value>
		/// <remarks>
		///	finish point has a point of maze exit
		/// </remarks>
		public Point finish { get; protected set; }
		/// <value>
		///		Gets height
		/// </value>
		/// <remarks>
		///	Height of maze
		/// </remarks>
		public int height { get; private set; }
		/// <value>
		///		Gets width
		/// </value>
		/// <remarks>
		///	Width of maze
		/// </remarks>
		public int width { get; private set; }
		/// <summary>
		/// Constructor of <c>Generator</c>
		/// </summary>
		/// <remarks>
		/// Fills fields of <c>Generator</c>
		///	</remarks>
		/// <param name="height">An integer value</param>
		/// <param name="width"> An integer value></param>
		/// <exception cref="System.ArgumentNullException">Throws if sizes are </exception>
		public Generator(int height, int width)
		{
			if (height < 0 || width < 0 || height > maxDimension || width > maxDimension)
				throw new ArgumentOutOfRangeException("height or width",
					$"Dimention must be greater than 0 and less than {maxDimension + 1}");
			this.width = width;
			this.height = height;
			mapMatrix = new Cell[height, width];
		}
		/// <summary>
		/// Start Generate Async
		/// </summary>
		/// <remarks>
		///		This function creates a new task with ayck thread
		///	</remarks>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Protress<string> for showing step by step te</string></param>
		/// <param name="signal">Variable for sogne</param>
		/// <returns>Returns new Task for this open</returns>
		public virtual Task Generate(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			Task.Run(() => GenerateAsync(token, progress, signal));
		/// <summary>
		///		GenerateAsync. This function is execute in separate thread).
		/// </summary>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Protress<string> for showing step by step te</string></param>
		/// <param name="signal">Variable for sogne</param>
		/// <exception cref="System.ObjectDisposedException">Throws when token disposed</exception>
		protected abstract void GenerateAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal);
		/// <summary>
		///		Function set wall in maze by coordinates <paramref name="y"/>;<paramref name="x"/>.
		/// </summary>
		/// <remarks>
		///		Function set <paramref name="value"/> to wall in coordinates of cell <paramref name="y"/>;<paramref name="x"/>
		/// according to <paramref name="dir"/>.
		///		If value is false, wall will dissappear.
		/// </remarks>
		/// <param name="dir">Direction to wall that is need</param>
		/// <param name="value">True of False. Value for wall</param>
		/// <param name="y">Y coordinate</param>
		/// <param name="x">X coordinate</param>
		/// <exception cref="System.ArgumentOutOfRangeException">Throws when coordinates not suitable for this maze</exception>
		protected void SetWall(Direction dir, bool value, int y, int x)
		{
			if (y >= height || x >= width || y < 0 || x < 0)
				throw new ArgumentOutOfRangeException("x or y", "Coordinate must be not negative and less than size of maze");
			switch (dir)
			{
				case Direction.left:
					mapMatrix[y, x].left = value;
					if (x != 0)
						mapMatrix[y, x - 1].right = value;
					break;
				case Direction.up:
					mapMatrix[y, x].up = value;
					if (y != 0)
						mapMatrix[y - 1, x].down = value;
					break;
				case Direction.right:
					mapMatrix[y, x].right = value;
					if (x != width - 1)
						mapMatrix[y, x + 1].left = value;
					break;
				case Direction.down:
					mapMatrix[y, x].down = value;
					if (y != height - 1)
						mapMatrix[y + 1, x].up = value;
					break;
			}
		}
	}
}
