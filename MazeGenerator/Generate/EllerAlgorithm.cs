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
				for (int j = 1; j < currGeneration.Length; ++j)
				{
					if (currGeneration[j] == currGeneration[j - 1])
						continue;
					addWall = random.Next(2) == 1;
					mapMatrix[i, j].left = addWall;
					if (!addWall)
					{
						usedSet[currGeneration[j] - 1] = false;
						ushort setItem = currGeneration[j];
						int k = j;
						while (currGeneration[k] == setItem)
						{
							currGeneration[k] = currGeneration[j - 1];
							++k;
						}
					}
				}
				for (int j = 0; j < currGeneration.Length; ++j)
				{
					int k = j + 1;
					while (k < currGeneration.Length && currGeneration[k] == currGeneration[k - 1])
						++k;
					int walls = random.Next(1, k - j + 1);
					for (int l = j; l < k; ++l)
						mapMatrix[i + 1, l].up = true;
					for (int l = 0; l < walls; ++l)
					{
						int pos = random.Next(j, k);
						mapMatrix[i + 1, pos].up = false;
					}
					for (int l = j; l < k; ++l)
						if (mapMatrix[i + 1, l].up == true)
						{
							usedSet[currGeneration[l] - 1] = false;
							currGeneration[l] = 0;
						}
				}

			}
			for (int j = 0; j < currGeneration.Length; ++j)
				if (currGeneration[j] == 0)
					for (int k = 0; k < usedSet.Length; ++k)
						if (!usedSet[k])
						{
							usedSet[k] = true;
							currGeneration[j] = (ushort)(k + 1);
						}
			for (int j = 1; j < currGeneration.Length; ++j)
			{
				if (currGeneration[j] != currGeneration[j - 1])
				{
					ushort setItem = currGeneration[j];
					int k = j;
					mapMatrix[height - 1, j].left = false;
					while (currGeneration[k] == setItem)
					{
						currGeneration[k] = currGeneration[j - 1];
						++k;
					}
				}
			}
		}
	}
}
