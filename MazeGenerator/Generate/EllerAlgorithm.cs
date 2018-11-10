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
			ushort[] currGeneration = new ushort[width];
			bool[] usedSet = new bool[width];
			Random random = new Random();
			bool addWall;
			for (int i = 0; i < height - 1; ++i)
			{
				for (int j = 0; j < currGeneration.Length; ++j)
					if (currGeneration[j] == 0)
						for (int k = 0; k < usedSet.Length; ++k)
							if (!usedSet[k])
							{
								usedSet[k] = true;
								currGeneration[j] = (ushort)(k + 1);
							}
				for (int j = 0; j < currGeneration.Length - 1; ++j)
				{
					addWall = random.Next(2) == 1;
					mapMatrix[i, j].right = mapMatrix[i, j + 1].left
				}
			}
		}
	}
}
