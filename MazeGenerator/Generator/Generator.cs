using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Types;

namespace MazeGenerator.Generator
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
		public static Generator Parse(string s);
		public static bool TryParse(string s, out Generator result);
		public void Action(bool showSteps, bool canDoNextStep);
		public abstract void Generate(bool showSteps, bool canDoNextStep);
		//To be continue...
	}
}
