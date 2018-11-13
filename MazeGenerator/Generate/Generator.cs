using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Types;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace MazeGenerator.Generate
{
    public abstract class Generator
    {
		private const ushort maxDimension = 1024;
		protected enum Direction { left, up, right, down}
		public Cell[,] mapMatrix { get; protected set; }
		public Point start { get; protected set; }
		public Point finish { get; protected set; }
		public ushort height { get; protected set; }
		public ushort width { get; protected set; }
		public Generator(ushort height, ushort width)
		{
			if (height > maxDimension || width > maxDimension)
				throw new ArgumentOutOfRangeException("height or width", 
					$"Dimentions must not be more than {maxDimension}");
			this.width = width;
			this.height = height;
			mapMatrix = new Cell[height, width];
		}
		public static Generator Parse(string s)
		{
			throw new NotImplementedException();
		}
		public static bool TryParse(string s, out Generator result)
		{
			throw new NotImplementedException();
		}
		public virtual void Action(bool showSteps, ref bool canDoNextStep)
		{
			Generate(showSteps, ref canDoNextStep);
		}
		public abstract void Generate(bool showSteps, ref bool canDoNextStep);
		public override string ToString()
		{
			throw new NotImplementedException();
		}
		protected void SetPixelColor(ref byte[]pixels, Color c, int i, int j, int wall, int cell, int y, int x, int stride)
		{
			pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4] = c.B;
			pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4 + 1] = c.G;
			pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4 + 2] = c.R;
			pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4 + 3] = c.A;
		}
		public virtual BitmapSource ToBitmap(int wallPx, int cellPx)
		{
			if (wallPx <= 0 || cellPx <= 0)
				throw new ArgumentOutOfRangeException("wallPx or cellPx", "sizes of wall and cell must be more than 0");
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
			for (int i = 0; i < this.height; ++i)
				for (int j = 0; j < this.width; ++j)
				{
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, 0, 0, stride);
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, 0, wallPx + cellPx, stride);
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, wallPx + cellPx, 0, stride);
					SetPixelColor(ref pixels, color, i, j, wallPx, cellPx, wallPx + cellPx, wallPx + cellPx, stride);
				}
			BitmapSource bitmap = BitmapSource.Create(width, height, 96.0, 96.0, pf, null, pixels, stride);
			return bitmap;
		}
		protected void SetWall(Direction dir, bool value, ushort y, ushort x)
		{
			if (y >= height || x >= width)
				throw new ArgumentOutOfRangeException("x or y", "Coordinate must be less the size of maze");
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
