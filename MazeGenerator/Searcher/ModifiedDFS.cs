using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using System.Windows.Media;

namespace MazeGenerator.Searcher
{
	public class ModifiedDFS : Searcher
	{
		public ModifiedDFS(Generator generator) : base(generator) { }
		public override void Search(bool showSteps, ref bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
		private void SetBlankAsDeadBlock(ref bool[,] deadblocksMap, ushort y, ushort x)
		{
			if (y >= deadblocksMap.GetLength(0) || x >= deadblocksMap.GetLength(1))
				return;
			if (deadblocksMap[y, x])
				return;
			int k = 0;
			if (mapMatrix[y, x].left || (x > 0 && deadblocksMap[y, x - 1])) ++k;
			if (mapMatrix[y, x].up || (y > 0 && deadblocksMap[y - 1, x])) ++k;
			if (mapMatrix[y, x].right || (x < width - 1 && deadblocksMap[y, x + 1])) ++k;
			if (mapMatrix[y, x].down || (y < height - 1 && deadblocksMap[y + 1, x])) ++k;
			if (k >= 3)
			{
				deadblocksMap[y, x] = true;
				if (!mapMatrix[y, x].left) SetBlankAsDeadBlock(ref deadblocksMap, y, (ushort)(x - 1));
				if (!mapMatrix[y, x].up) SetBlankAsDeadBlock(ref deadblocksMap, (ushort)(y - 1), x);
				if (!mapMatrix[y, x].right) SetBlankAsDeadBlock(ref deadblocksMap, y, (ushort)(x + 1));
				if (!mapMatrix[y, x].down) SetBlankAsDeadBlock(ref deadblocksMap, (ushort)(y + 1), x);
			}
		}
		private void SetDeadBlocks(ref bool[,] deadblocksMap)
		{
			for (int i = 0; i < deadblocksMap.GetLength(0); ++i)
				for (int j = 0; j < deadblocksMap.GetLength(1); ++j)
					SetBlankAsDeadBlock(ref deadblocksMap, (ushort)i, (ushort)j);
		}
	}
}
