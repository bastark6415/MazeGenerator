using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using System.Windows.Media;
using MazeGenerator.Types;

namespace MazeGenerator.Searcher
{
	public class ModifiedDFS : Searcher
	{
		private bool[,] deadblocksMap;
		private bool[,] visited;
		public ModifiedDFS(Generator generator) : base(generator) { }
		public override void Search(bool showSteps, ref bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
		private void SetBlankAsDeadBlock(ushort y, ushort x)
		{
			if (y >= deadblocksMap.GetLength(0) || x >= deadblocksMap.GetLength(1))
				return;
			if (deadblocksMap[y, x])
				return;
			int k = 0;
			if (generator.mapMatrix[y, x].left	|| (x > 0					 && deadblocksMap[y, x - 1])) ++k;
			if (generator.mapMatrix[y, x].up	|| (y > 0					 && deadblocksMap[y - 1, x])) ++k;
			if (generator.mapMatrix[y, x].right || (x < generator.width - 1	 && deadblocksMap[y, x + 1])) ++k;
			if (generator.mapMatrix[y, x].down	|| (y < generator.height - 1 && deadblocksMap[y + 1, x])) ++k;
			if (k >= 3)
			{
				deadblocksMap[y, x] = true;
				if (!generator.mapMatrix[y, x].left)	SetBlankAsDeadBlock(y, (ushort)(x - 1));
				if (!generator.mapMatrix[y, x].up)		SetBlankAsDeadBlock((ushort)(y - 1), x);
				if (!generator.mapMatrix[y, x].right)	SetBlankAsDeadBlock(y, (ushort)(x + 1));
				if (!generator.mapMatrix[y, x].down)	SetBlankAsDeadBlock((ushort)(y + 1), x);
			}
		}
		private void SetDeadBlocks()
		{
			for (int i = 0; i < deadblocksMap.GetLength(0); ++i)
				for (int j = 0; j < deadblocksMap.GetLength(1); ++j)
					SetBlankAsDeadBlock((ushort)i, (ushort)j);
		}
	}
}
