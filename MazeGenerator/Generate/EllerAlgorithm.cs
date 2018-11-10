using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Generate
{
    public class EllerAlgorithm : Generator
    {
		public EllerAlgorithm(ushort height, ushort width) : base(height, width) { }
		public override void Generate(bool showSteps, ref bool canDoNextStep)
		{
			Random random = new Random();
			for (int i = 0; i < height; ++i)
			{
				for (int j = 0; j < width; ++j)
				{
					mapMatrix[i, j].left = random.Next(2) == 1; 
					mapMatrix[i, j].right = random.Next(2) == 1;
					mapMatrix[i, j].up = random.Next(2) == 1;
					mapMatrix[i, j].down = random.Next(2) == 1;
				}
			}
		}
	}
}
