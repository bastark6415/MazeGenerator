using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Types;
using System.Windows.Media.Imaging;

namespace MazeGenerator.Generate
{
    public abstract class Generator
    {
		public int[,] mapMatrix { get; protected set; }
		public Point start { get; protected set; }
		public Point finish { get; protected set; }
		public ushort height { get; protected set; }
		public ushort width { get; protected set; }
		public Generator(ushort height, ushort width)
		{
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
		public virtual void Action(bool showSteps, bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
		public abstract void Generate(bool showSteps, bool canDoNextStep);
		public override string ToString()
		{
			throw new NotImplementedException();
		}
		public virtual BitmapSource ToBitmap()
		{
			throw new NotImplementedException();
		}
	}
}
