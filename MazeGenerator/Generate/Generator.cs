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
		protected void SetPixelColor(ref byte[]pixels, Color c, int yOfCell, int xOfCell, int wallPx, int cellPx, int yInCell, int xInCell, int stride)
		{
			int colorIndex = (wallPx + cellPx) * (yOfCell * stride + xOfCell * 4) + yInCell * stride + xInCell * 4;
			if (colorIndex < 0 || colorIndex > pixels.Length - 4)
				throw new ArgumentNullException("parameters", "Invalid input parameters");
			pixels[colorIndex] = c.B;
			pixels[colorIndex + 1] = c.G;
			pixels[colorIndex + 2] = c.R;
			pixels[colorIndex + 3] = c.A;
		}
		protected Color GetPixelColor(ref byte[] pixels, int yOfCell, int xOfCell, int wallPx, int cellPx, int yInCell, int xInCell, int stride)
		{
			int colorIndex = (wallPx + cellPx) * (yOfCell * stride + xOfCell * 4) + yInCell * stride + xInCell * 4;
			if (colorIndex < 0 || colorIndex > pixels.Length - 4)
				throw new ArgumentNullException("parameters", "Invalid input parameters");
			Color c = new Color();
			c.B = pixels[colorIndex];
			c.G = pixels[colorIndex + 1];
			c.R = pixels[colorIndex + 2];
			c.A = pixels[colorIndex + 3];
			return c;
		}
		public virtual BitmapSource ToBitmap(int wallPx, int cellPx)
		{
			if (wallPx <= 0 || cellPx <= 0)
				throw new ArgumentOutOfRangeException("wallPx or cellPx", "sizes of wall and cell must be greater than 0");
			int height = this.height * (cellPx + wallPx) + wallPx;
			int width = this.width * (cellPx + wallPx) + wallPx;
			PixelFormat pf = PixelFormats.Pbgra32;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			Color color;
			for (int i = 0; i < this.height; ++i)
				for (int j = 0; j < this.width; ++j)
				{	
					//drawing left wall
					color = mapMatrix[i, j].left ? Colors.Black : Colors.White;
					for (int y = 1; y < cellPx + wallPx; ++y)
						SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, y, 0, stride);
					//drawing up wall
					color = mapMatrix[i, j].up ? Colors.Black : Colors.White;
					for (int x = 1; x < cellPx + wallPx; ++x)
						SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, 0, x, stride);
					//drawing cellPx
					color = Colors.White;
					for (int y = 1; y < cellPx + wallPx; ++y)
						for (int x = 1; x < cellPx + wallPx; ++x)
							SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, y, x, stride);
				}
			//drawing down wall
			for (int j = 0; j < this.width; ++j)
			{
				color = mapMatrix[this.height - 1, j].down ? Colors.Black : Colors.White;
				for (int x = 1; x < cellPx + wallPx; ++x)
					SetPixelColor(ref pixels, color, this.height - 1, j, wallPx, cellPx, wallPx + cellPx, x, stride);
			}
			//drawing right wall
			for (int i = 0; i < this.height; ++i)
			{
				color = mapMatrix[i, this.width - 1].right ? Colors.Black : Colors.White;
				for (int y = 1; y < cellPx + wallPx; ++y)
					SetPixelColor(ref pixels, color, i, this.width - 1, wallPx, cellPx, y, wallPx + cellPx, stride);
			}
			//corners
			color = Colors.Black;
			int sumWallCell = wallPx + cellPx;
			for (int i = 0; i < this.height; ++i)
				for (int j = 0; j < this.width; ++j)
				{
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, 0, 0, stride);
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, 0, sumWallCell, stride);
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, sumWallCell, 0, stride);
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, sumWallCell, sumWallCell, stride);
				}
			BitmapSource bitmap = BitmapSource.Create(width, height, 96.0, 96.0, pf, null, pixels, stride);
			return bitmap;
		}
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
