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
			for (int i = 1; i < height - 1; ++i)
				for (int j = 1; j < width - 1; ++j)
				{
					bool addwall = random.Next(2) == 1;
					int dir = random.Next(4);
					switch (dir)
					{
						case 0:
							mapMatrix[i, j].left = addwall;
							mapMatrix[i, j - 1].right = addwall;
							break;
						case 1:
							mapMatrix[i, j].up = addwall;
							mapMatrix[i - 1, j].down = addwall;
							break;
						case 2:
							mapMatrix[i, j].right = addwall;
							mapMatrix[i, j + 1].left = addwall;
							break;
						case 3:
							mapMatrix[i, j].down = addwall;
							mapMatrix[i + 1, j].up = addwall;
							break;
					}
				}
		}
	}
}
