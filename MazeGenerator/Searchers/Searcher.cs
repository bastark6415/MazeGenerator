using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using MazeGenerator.Types;
using System.Windows.Media;
using System.Threading;

namespace MazeGenerator.Searchers
{
    public abstract class Searcher : GeneratorDecorator
    {
		public LinkedList<Path> paths { get; protected set; }
		protected bool[,] visited;
		public Searcher(Generator generator) : base(generator)
		{
			paths = new LinkedList<Path>();
			visited = new bool[height, width];
			SetDeadBlocks();
		}
		public virtual Task Search(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			Task.Run(() => SearchAsync(progress, signal), token);
		protected abstract void SearchAsync(IProgress<string> progress, ManualResetEvent signal);
		public override BitmapSource ToBitmap(int wallPx, int cellPx)
		{
			bool[] allPaths = new bool[paths.Count];
			for (int i = 0; i < allPaths.Length; ++i)
				allPaths[i] = true;
			return ToBitmap(wallPx, cellPx, allPaths);
		}
		public virtual BitmapSource ToBitmap(int wallPx, int cellPx, bool[] pathsForShow)
		{
			BitmapSource source = base.ToBitmap(wallPx, cellPx);
			PixelFormat pf = PixelFormats.Bgra32;
			int height = this.height * (cellPx + wallPx) + wallPx;
			int width = this.width * (cellPx + wallPx) + wallPx;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			source.CopyPixels(pixels, stride, 0);
			Color[] colors = new Color[] {Colors.Blue, Colors.Yellow, Colors.Coral, Colors.Cyan,
				Colors.Green, Colors.Red, Colors.Violet, Colors.Orange, Colors.Pink,
				Colors.OrangeRed, Colors.Salmon, Colors.Tomato, Colors.Silver, Colors.PeachPuff,
				Colors.Navy, Colors.BurlyWood };
			for (int i = 0; i < colors.Length; ++i)
				colors[i].A = 200;
			Color c = new Color();
			Color bg;
			Color fg;
			for (int i = 0; i < pathsForShow.Length; ++i)
			{
				if (!pathsForShow[i])
					continue;
				fg = colors[i % colors.Length];
				foreach (Point p in paths.ElementAt(i).path)
				{
					for (int k = wallPx; k < wallPx + cellPx; ++k)
					{
						for (int l = wallPx; l < wallPx + cellPx; ++l)
						{
							bg = GetPixelColor(ref pixels, p.y, p.x, wallPx, cellPx, k, l, stride);
							c = Color.Add(bg, Color.Multiply(Color.Subtract(fg, bg), fg.ScA));
							SetPixelColor(ref pixels, c, p.y, p.x, wallPx, cellPx, k, l, stride);
						}
					}
				}
			}
			return BitmapSource.Create(width, height, 96, 96, pf, null, pixels, stride);
		}
		protected void SetBlankAsDeadBlock(int y, int x)
		{
			if (visited[y, x])
				return;
			int k = 0;
			if (mapMatrix[y, x].left  || (x > 0 && visited[y, x - 1])) ++k;
			if (mapMatrix[y, x].up	  || (y > 0 && visited[y - 1, x])) ++k;
			if (mapMatrix[y, x].right || (x < width - 1 && visited[y, x + 1])) ++k;
			if (mapMatrix[y, x].down  || (y < height - 1 && visited[y + 1, x])) ++k;
			if (k >= 3)
			{
				visited[y, x] = true;
				if (!mapMatrix[y, x].left	&& x > 0)			SetBlankAsDeadBlock(y, x - 1);
				if (!mapMatrix[y, x].up		&& y > 0)			SetBlankAsDeadBlock(y - 1, x);
				if (!mapMatrix[y, x].right	&& x < width - 1)	SetBlankAsDeadBlock(y, x + 1);
				if (!mapMatrix[y, x].down	&& y < height - 1)	SetBlankAsDeadBlock(y + 1, x);
			}
		}
		protected void SetDeadBlocks()
		{
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					SetBlankAsDeadBlock(i, j);
		}
	}
}
