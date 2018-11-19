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
	public abstract class Generator
	{
		private const int maxDimension = 512;
		protected enum Direction { left, up, right, down}
		public Cell[,] mapMatrix { get; protected set; }
		public Point start { get; protected set; }
		public Point finish { get; protected set; }
		public int height { get; private set; }
		public int width { get; private set; }
		public Generator(int height, int width)
		{
			if (height < 0 || width < 0 || height > maxDimension || width > maxDimension)
				throw new ArgumentOutOfRangeException("height or width",
					$"Dimention must be greater than 0 and less than {maxDimension + 1}");
			this.width = width;
			this.height = height;
			mapMatrix = new Cell[height, width];
		}
		public virtual Task Generate(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			Task.Run(() => GenerateAsync(token, progress, signal));
		protected abstract void GenerateAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal);
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
