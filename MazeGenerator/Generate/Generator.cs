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
		private const ushort maxDimension = 8192;
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
		public virtual BitmapSource ToBitmap()
		{
			int wall = 1, cell = 8;
			int height = this.height * (cell + wall) + wall;
			int width = this.width * (cell + wall) + wall;
			PixelFormat pf = PixelFormats.Pbgra32;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			Color color;
			for (int i = 0; i < this.height; ++i)
				for (int j = 0; j < this.width; ++j)
					for (int y = 0; y < wall + cell; ++y)
						for (int x = 0; x < wall + cell; ++x)
						{
							if ((x < wall && mapMatrix[i, j].left) || (y < wall && mapMatrix[i, j].up))
								color = Colors.Black;
							else
								color = Colors.White;
							pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4] = color.B;
							pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4 + 1] = color.G;
							pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4 + 2] = color.R;
							pixels[(wall + cell) * (i * stride + j * 4) + y * stride + x * 4 + 3] = color.A;
						}
			for (int i = 0; i < this.height; ++i)
				for (int y = 0; y < wall + cell; ++y)
				{
					if (mapMatrix[i, this.width - 1].right)
						color = Colors.Black;
					else
						color = Colors.White;
					pixels[(wall + cell) * (i * stride + this.width * 4) + y * stride] = color.B;
					pixels[(wall + cell) * (i * stride + this.width * 4) + y * stride + 1] = color.G;
					pixels[(wall + cell) * (i * stride + this.width * 4) + y * stride + 2] = color.R;
					pixels[(wall + cell) * (i * stride + this.width * 4) + y * stride + 3] = color.A;
				}
			for (int j = 0; j < this.width; ++j)
				for (int x = 0; x < wall + cell; ++x)
				{
					if (mapMatrix[this.height - 1, j].down)
						color = Colors.Black;
					else
						color = Colors.White;
					pixels[(wall + cell) * (this.height * stride + j * 4) + x * 4] = color.B;
					pixels[(wall + cell) * (this.height * stride + j * 4) + x * 4 + 1] = color.G;
					pixels[(wall + cell) * (this.height * stride + j * 4) + x * 4 + 2] = color.R;
					pixels[(wall + cell) * (this.height * stride + j * 4) + x * 4 + 3] = color.A;
				}
			BitmapSource bitmap = BitmapSource.Create(width, height, 96.0, 96.0, pf, null, pixels, stride);
			return bitmap;
		}
	}
}
