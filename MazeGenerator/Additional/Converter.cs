using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Searchers;
using MazeGenerator.Generate;
using System.Windows.Media;
using MazeGenerator.Types;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MazeGenerator.Additional
{
	/// <summary>
	/// <c>ConverterToBitmap</c> convert <c>Generator</c> map to <c>BitmapSource</c>.
	/// </summary>
	/// <remarks>
	/// Has a colors array for paths from <c>Searcher</c>.
	/// Has a PropertyChanged Event that called always when bitmap changing.
	/// </remarks>
	public class ConverterToBitmap : INotifyPropertyChanged
	{
		private BitmapSource bitmap;
		/// <value>
		/// Get Bitmap. If set call OnPropertyChancged.
		/// </value>
		public BitmapSource Bitmap 
		{ 
			get { return bitmap; }
			protected set
			{
				if (bitmap != value)
				{
					bitmap = value;
					OnPropertyChanged("Bitmap");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public int wallPx { get; protected set; }
		public int cellPx { get; protected set; }
		protected readonly Color[] colors = new Color[] {Colors.Red, Colors.Green, Colors.Violet, Colors.Orange,
			Colors.GreenYellow, Color.FromRgb(255, 216, 0), Color.FromRgb(154, 159, 182),
			Color.FromRgb(100, 65, 164), Color.FromRgb(0, 43, 82), Colors.Blue, Colors.Brown};
		/// <summary>
		///		InitializeCovered Colors
		/// </summary>
		/// <param name="wallPx">As int</param>
		/// <param name="cellPx">As int</param>
		/// <exception cref="System.ArgumentOutOfRangeException";
		public ConverterToBitmap(int wallPx, int cellPx) 
		{
			if (wallPx <= 0 || cellPx <= 0)
				throw new ArgumentOutOfRangeException("wallPx or cellPx", "sizes of wall and cell must be greater than 0");
			this.wallPx = wallPx; 
			this.cellPx = cellPx;
			for (int i = 0; i < colors.Length; ++i)
				colors[i].A = 200;
		}
		/// <summary>
		/// Update binding to this object
		/// </summary>
		/// <param name="prop">PropertyEvent</param>
		public void OnPropertyChanged(string prop)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
		}
		/// <summary>
		/// SetPixelColor in input parameters
		/// </summary>
		/// <param name="pixels">Input colors</param>
		/// <param name="c">Color to set</param>
		/// <param name="yOfCell">y in maze map</param>s
		/// <param name="xOfCell">x in maze map</param>
		/// <param name="yInCell">y in cell</param>
		/// <param name="xInCell">x in cell</param>
		/// <param name="stride">stride</param>
		/// <exception cref="System.ArgumentNullException";
		protected void SetPixelColor(ref byte[] pixels, Color c, int yOfCell, int xOfCell, int yInCell, int xInCell, int stride)
		{
			int colorIndex = (wallPx + cellPx) * (yOfCell * stride + xOfCell * 4) + yInCell * stride + xInCell * 4;
			if (colorIndex < 0 || colorIndex > pixels.Length - 4)
				throw new ArgumentNullException("parameters", "Invalid input parameters");
			pixels[colorIndex] = c.B;
			pixels[colorIndex + 1] = c.G;
			pixels[colorIndex + 2] = c.R;
			pixels[colorIndex + 3] = c.A;
		}
		/// <summary>
		/// SetPixelColor in input parameters
		/// </summary>
		/// <param name="pixels">Input colors</param>
		/// <param name="yOfCell">y in maze map</param>s
		/// <param name="xOfCell">x in maze map</param>
		/// <param name="yInCell">y in cell</param>
		/// <param name="xInCell">x in cell</param>
		/// <param name="stride">stride</param>
		/// <returns>Color of pixel</returns>
		/// <exception cref="System.ArgumentNullException";
		protected Color GetPixelColor(ref byte[] pixels, int yOfCell, int xOfCell, int yInCell, int xInCell, int stride)
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
		/// <summary>
		/// Sets Bitmap by converting <c>Generator</c> to Bitmap
		/// </summary>
		/// <param name="generator">Generator to Bitmap</param>
		/// <exception cref="System.ArgumentNullException">Trow if generator null</exception>
		public void Convert(Generator generator)
		{
			if (generator == null)
				throw new ArgumentNullException("generator", "Argument can't be null");
			int height = generator.height * (cellPx + wallPx) + wallPx;
			int width = generator.width * (cellPx + wallPx) + wallPx;
			PixelFormat pf = PixelFormats.Pbgra32;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			Color color;
			for (int i = 0; i < generator.height; ++i)
				for (int j = 0; j < generator.width; ++j)
				{
					//drawing left wall
					color = generator.mapMatrix[i, j].left ? Colors.Black : Colors.White;
					for (int y = 1; y < cellPx + wallPx; ++y)
						SetPixelColor(ref pixels, color, i, j, y, 0, stride);
					//drawing up wall
					color = generator.mapMatrix[i, j].up ? Colors.Black : Colors.White;
					for (int x = 1; x < cellPx + wallPx; ++x)
						SetPixelColor(ref pixels, color, i, j, 0, x, stride);
					//drawing cellPx
					color = Colors.White;
					for (int y = 1; y < cellPx + wallPx; ++y)
						for (int x = 1; x < cellPx + wallPx; ++x)
							SetPixelColor(ref pixels, color, i, j, y, x, stride);
				}
			//drawing down wall
			for (int j = 0; j < generator.width; ++j)
			{
				color = generator.mapMatrix[generator.height - 1, j].down ? Colors.Black : Colors.White;
				for (int x = 1; x < cellPx + wallPx; ++x)
					SetPixelColor(ref pixels, color, generator.height - 1, j, wallPx + cellPx, x, stride);
			}
			//drawing right wall
			for (int i = 0; i < generator.height; ++i)
			{
				color = generator.mapMatrix[i, generator.width - 1].right ? Colors.Black : Colors.White;
				for (int y = 1; y < cellPx + wallPx; ++y)
					SetPixelColor(ref pixels, color, i, generator.width - 1, y, wallPx + cellPx, stride);
			}
			//corners
			color = Colors.Black;
			int sumWallCell = wallPx + cellPx;
			for (int i = 0; i < generator.height; ++i)
				for (int j = 0; j < generator.width; ++j)
				{
					SetPixelColor(ref pixels, color, i, j, 0, 0, stride);
					SetPixelColor(ref pixels, color, i, j, 0, sumWallCell, stride);
					SetPixelColor(ref pixels, color, i, j, sumWallCell, 0, stride);
					SetPixelColor(ref pixels, color, i, j, sumWallCell, sumWallCell, stride);
				}
			Bitmap = BitmapSource.Create(width, height, 96.0, 96.0, pf, null, pixels, stride);
		}
		/// <summary>
		/// Sets Bitmap by converting <c>Searcher</c> to Bitmap
		/// </summary>
		/// <param name="searcher"><c>Searcher</c> to Bitmap</param>
		/// <exception cref="System.ArgumentNullException">Trow if searcher null</exception>
		public void Convert(Searcher searcher)
		{
			if (searcher == null)
				throw new ArgumentNullException("searcher", "Argument can't be null");
			bool[] allPaths = new bool[searcher.paths.Count];
			for (int i = 0; i < allPaths.Length; ++i)
				allPaths[i] = true;
			Convert(searcher, allPaths);
		}
		/// <summary>
		/// Sets Bitmap by converting <c>Searcher</c> to Bitmap
		/// </summary>
		/// <param name="searcher"><c>Searcher</c> to Bitmap</param>
		/// <param name="pathsForShow"
		/// <exception cref="System.ArgumentNullException">Trow if searcher null</exception>
		public void Convert(Searcher searcher, bool[] pathsForShow)
		{
			if (searcher == null || pathsForShow == null)
				throw new ArgumentNullException("searcher", "Argument can't be null");
			Convert(searcher.generator);
			PixelFormat pf = PixelFormats.Bgra32;
			int height = searcher.height * (cellPx + wallPx) + wallPx;
			int width = searcher.width * (cellPx + wallPx) + wallPx;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			Bitmap.CopyPixels(pixels, stride, 0);
			Color c = new Color();
			Color bg;
			Color fg;
			for (int i = 0; i < pathsForShow.Length; ++i)
			{
				if (!pathsForShow[i])
					continue;
				fg = colors[i % colors.Length];
				foreach (Point p in searcher.paths.ElementAt(i).path)
				{
					for (int k = wallPx; k < wallPx + cellPx; ++k)
					{
						for (int l = wallPx; l < wallPx + cellPx; ++l)
						{
							bg = GetPixelColor(ref pixels, p.y, p.x, k, l, stride);
							c = Color.Add(bg, Color.Multiply(Color.Subtract(fg, bg), fg.ScA));
							SetPixelColor(ref pixels, c, p.y, p.x, k, l, stride);
						}
					}
				}
			}
			Bitmap = BitmapSource.Create(width, height, 96, 96, pf, null, pixels, stride);
		}
		/// <summary>
		/// Save Bitmap
		/// </summary>
		/// <param name="path">Path to save</param>
		/// <exception cref="System.ArgumentException"></exception>
		/// <exception cref="System.NotSupportedException"></exception>
		/// <exception cref="System.ArgumentNullException"></exception>
		/// <exception cref="System.Security.SecurityException"></exception>
		/// <exception cref="System.IO.FileNotFoundException"></exception>
		/// <exception cref="System.IO.IOException"></exception>
		/// <exception cref="System.IO.DirectoryNotFoundException"></exception>
		/// <exception cref="System.IO.PathTooLongException"></exception>
		/// <exception cref="System.ArgumentOutOfRangeException"></exception>
		/// <exception cref="System.InvalidOperationException"></exception>
		public void SaveToFile(string path)
		{
			BmpBitmapEncoder encoder = new BmpBitmapEncoder();
			encoder.Frames.Add(BitmapFrame.Create(Bitmap));
			using (FileStream stream = new FileStream(path, FileMode.Create))
				encoder.Save(stream);
		}
	}
}
