﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using MazeGenerator.Types;
using System.Windows.Media;

namespace MazeGenerator.Searchers
{
    public abstract class Searcher : GeneratorDecorator
    {
		public List<Path> paths { get; protected set; }
		public Searcher(Generator generator) : base(generator)
		{
			paths = new List<Path>();
		}
		public override void Action(ref bool? canDoNextStep)
		{
			base.Action(ref canDoNextStep);
			Search(ref canDoNextStep);
		}
		public abstract void Search(ref bool? canDoNextStep);
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
			int height = generator.height * (cellPx + wallPx) + wallPx;
			int width = generator.width * (cellPx + wallPx) + wallPx;
			int stride = (width * pf.BitsPerPixel + 7) / 8;
			byte[] pixels = new byte[height * stride];
			source.CopyPixels(pixels, stride, 0);
			Color[] colors = new Color[pathsForShow.Length];
			Random rand = new Random();
			for (int i = 0; i < colors.Length; ++i)
			{
				colors[i].B = (byte)rand.Next(255);
				colors[i].G = (byte)rand.Next(255);
				colors[i].R = (byte)rand.Next(255);
				colors[i].A = 120;
			}
			for (int i = 0; i < pathsForShow.Length; ++i)
			{
				if (!pathsForShow[i])
					continue;
				foreach (Point p in paths[i].path)
				{
					for (int k = wallPx; k < wallPx + cellPx; ++k)
					{
						for (int l = wallPx; l < wallPx + cellPx; ++l)
						{
							Color prev = GetPixelColor(ref pixels, p.y, p.x, wallPx, cellPx, k, l, stride);
							prev = Color.Add(prev, Color.Multiply(Color.Subtract(colors[i], prev), colors[i].ScA));
							SetPixelColor(ref pixels, prev, p.y, p.x, wallPx, cellPx, k, l, stride);
						}
					}
				}
			}
			return BitmapSource.Create(width, height, 96, 96, pf, null, pixels, stride);
		}
	}
}
