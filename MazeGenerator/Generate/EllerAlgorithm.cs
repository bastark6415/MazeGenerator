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
			ushort[] currRow = new ushort[width];
			bool[] usedSet = new bool[width + 1];
			for (int i = 0; i < height - 1; ++i)
			{
				//assign for each cell unique set if it hasn't
				for (int j = 0; j < width; ++j)
					if (currRow[j] == 0)
						for (ushort k = 1; k <= width; ++k)
							if (!usedSet[k])
							{
								usedSet[k] = true;
								currRow[j] = k;
								break;
							}
				//create right walls
				for (int j = 0; j < width - 1; ++j) 
				{
					if (currRow[j] == currRow[j + 1])
						SetWall(Direction.right, true)
				}
			}
		}
	}
}
