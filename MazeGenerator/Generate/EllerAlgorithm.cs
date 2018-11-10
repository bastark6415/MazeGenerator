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
			//throw new NotImplementedException();
			Random random = new Random();
			for (int i = 0; i < height; ++i)
			{
				for (int j = 0; j < width; ++j)
				{
					mapMatrix[i, j] = random.Next(-1, 1);
				}
			}
		}
	}
}
