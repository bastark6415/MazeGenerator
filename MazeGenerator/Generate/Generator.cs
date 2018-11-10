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
		public int[,] mapMatrix { get; protected set; }
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
			mapMatrix = new int[height, width];
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
			PixelFormat pf = PixelFormats.Pbgra32;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			Color color;
			for (int i = 0; i < height; ++i)
			{
				for (int j = 0; j < width; ++j)
				{
					switch (mapMatrix[i, j])
					{
						case 0:
							color = Colors.White;
							break;
						case -1:
							color = Colors.Black;
							break;
						default:
							color = Colors.White;
							break;
					}
					pixels[i * stride + j * 4] = color.B;
					pixels[i * stride + j * 4 + 1] = color.G;
					pixels[i * stride + j * 4 + 2] = color.R;
					pixels[i * stride + j * 4 + 3] = color.A;
				}
			}
			BitmapSource bitmap = BitmapSource.Create(width, height, 96.0, 96.0, pf, null, pixels, stride);
			return bitmap;
		}
	}
}
