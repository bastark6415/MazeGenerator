using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Generate;

namespace MazeGenerator.Searcher
{
	public class ConcreteSearcher : Searcher
	{
		public ConcreteSearcher(Generator generator) : base(generator) { }
		public override void Search(bool showSteps, ref bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
		private void SetBlankAsDeadBlock(ref bool[,] deadblocksMap, ushort x, ushort y)
		{
			if (y >= deadblocksMap.GetLength(0) && x >= deadblocksMap.GetLength(1))
				throw new ArgumentOutOfRangeException("x or y", "coordinate must be less than dimension");
			if (deadblocksMap[y, x])
				return;
			int k = 0;
			if (mapMatrix[y, x].left) ++k;
			if (mapMatrix[y, x].up) ++k;
			if (mapMatrix[y, x].right) ++k;
			if (mapMatrix[y, x].down) ++k;
			if (k >= 3)
			{
				deadblocksMap[y, x] = true;
				if (!mapMatrix[y, x].left) SetBlankAsDeadBlock(ref deadblocksMap, (ushort)(x - 1), y);
				if (!mapMatrix[y, x].up) SetBlankAsDeadBlock(ref deadblocksMap, x, (ushort)(y - 1));
				if (!mapMatrix[y, x].right) SetBlankAsDeadBlock(ref deadblocksMap, (ushort)(x + 1), y);
				if (!mapMatrix[y, x].down) SetBlankAsDeadBlock(ref deadblocksMap, x, (ushort)(y + 1));
			}
		}
	}
}
