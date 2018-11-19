using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MazeGenerator.Types;

namespace MazeGenerator.Generate
{
    public class EllerAlgorithm : Generator
    {
		public EllerAlgorithm(int height, int width) : base(height, width) { }
		protected override void GenerateAsync(IProgress<string> progress, ManualResetEvent signal)
		{
			int[] currRow = new int[width];
			bool[] usedSet = new bool[width + 1];
			Random rand = new Random();
			//external walls
			//left and right
			for (int i = 0; i < height; ++i)
			{
				SetWall(Direction.left, true, i, 0);
				SetWall(Direction.right, true, i, width - 1);
			}
			//up and down
			for (int j = 0; j < width; ++j)
			{
				SetWall(Direction.up, true, 0, j);
				SetWall(Direction.down, true, height - 1, j);
			}
			//Progress
			progress?.Report("Generating...");
			signal?.Reset();
			signal?.WaitOne();
			//Internal walls
			for (int i = 0; i < height; ++i)
			{
				//assign for each cell unique set if it hasn't
				for (int j = 0; j < width; ++j)
					if (currRow[j] == 0)
						for (int k = 1; k <= width; ++k)
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
						SetWall(Direction.right, true, i, j);
					else if (rand.Next(2) == 1 && i != height - 1)
						SetWall(Direction.right, true, i, j);
					else
					{
						int setValue = currRow[j + 1];
						usedSet[setValue] = false;
						for (int k = 0; k < width; ++k)
							if (currRow[k] == setValue)
								currRow[k] = currRow[j];
					}
					//Progress
					if (signal != null)
					{
						progress?.Report("Generating...");
						signal.Reset();
						signal.WaitOne();
					}
				}
				//create down walls
				for (int l = 1; l <= width; ++l)
				{
					if (!usedSet[l])
						continue;
					int cntCellsInSetWithoutWalls = 0;
					for (int j = 0; j < width; ++j)
						if (currRow[j] == l)
							++cntCellsInSetWithoutWalls;
					for (int k = 0; k < width && cntCellsInSetWithoutWalls > 1; ++k)
						if (currRow[k] == l)
							if (rand.Next(2) == 1)
							{
								SetWall(Direction.down, true, (ushort)i, (ushort)k);
								--cntCellsInSetWithoutWalls;
							}
					//Progress
					if (signal != null)
					{
						progress?.Report("Generating...");
						signal.Reset();
						signal.WaitOne();
					}
				}
				//update used set
				for (int j = 1; j <= width; ++j)
					usedSet[j] = false;
				//remove cells with down walls from set
				for (int j = 0; j < width; ++j)
					if (mapMatrix[i, j].down)
						currRow[j] = 0;
					else
						usedSet[currRow[j]] = true;
			}
			//Random walls removing
			if (height > 2 && width > 2)
			{
				int cntWalls = height * width <= 10000 ? 1 : (int)Math.Round(height * width * 0.0001);
				Point p;
				for (int i = 0; i < cntWalls; ++i)
				{
					p.x = rand.Next(1, width - 1);
					p.y = rand.Next(1, height - 1);
					Direction direction = (Direction)rand.Next(4);
					SetWall(direction, false, p.y, p.x);
					//Progress
					if (signal != null)
					{
						progress?.Report("Generating...");
						signal.Reset();
						signal.WaitOne();
					}
				}
			}
			//random start finish
			Direction dir = (Direction)rand.Next(4);
			Point tmp;
			switch (dir)
			{
				case Direction.left:
					tmp.x = 0;
					tmp.y = rand.Next(height);
					start = tmp;
					SetWall(Direction.left, false, start.y, start.x);
					tmp.x = width - 1;
					tmp.y = rand.Next(height);
					finish = tmp;
					SetWall(Direction.right, false, finish.y, finish.x);
					break;
				case Direction.up:
					tmp.x = rand.Next(width);
					tmp.y = 0;
					start = tmp;
					SetWall(Direction.up, false, start.y, start.x);
					tmp.x = rand.Next(width);
					tmp.y = height - 1;
					finish = tmp;
					SetWall(Direction.down, false, finish.y, finish.x);
					break;
				case Direction.right:
					tmp.x = width - 1;
					tmp.y = rand.Next(height);
					start = tmp;
					SetWall(Direction.right, false, start.y, start.x);
					tmp.x = 0;
					tmp.y = rand.Next(height);
					finish = tmp;
					SetWall(Direction.left, false, finish.y, finish.x);
					break;
				case Direction.down:
					tmp.x = rand.Next(width);
					tmp.y = height - 1;
					start = tmp;
					SetWall(Direction.down, false, start.y, start.x);
					tmp.x = rand.Next(width);
					tmp.y = 0;
					finish = tmp;
					SetWall(Direction.up, false, finish.y, finish.x);
					break;
			}
			progress?.Report("Generated");
			signal?.Dispose();
		}
	}
}
