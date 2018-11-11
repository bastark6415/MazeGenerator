using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Types;

namespace MazeGenerator.Generate
{
    public class EllerAlgorithm : Generator
    {
		public EllerAlgorithm(ushort height, ushort width) : base(height, width) { }
		public override void Generate(bool showSteps, ref bool canDoNextStep)
		{
			ushort[] currRow = new ushort[width];
			bool[] usedSet = new bool[width + 1];
			Random rand = new Random();
			for (int i = 0; i < height; ++i)
			{
				//assign for each cell unique set if it hasn't
				for (int j = 0; j < width; ++j)
					if (currRow[j] == 0)
						for (int k = 1; k <= width; ++k)
							if (!usedSet[k])
							{
								usedSet[k] = true;
								currRow[j] = (ushort)k;
								break;
							}
				//create right walls
				for (int j = 0; j < width - 1; ++j)
				{
					if (currRow[j] == currRow[j + 1])
						SetWall(Direction.right, true, (ushort)i, (ushort)j);
					else if (rand.Next(2) == 1 && i != height - 1)
						SetWall(Direction.right, true, (ushort)i, (ushort)j);
					else
					{
						int setValue = currRow[j + 1];
						usedSet[setValue] = false;
						for (int k = 0; k < width; ++k)
							if (currRow[k] == setValue)
								currRow[k] = currRow[j];
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
			//external walls
			//left right
			for (int i = 0; i < height; ++i)
			{
				SetWall(Direction.left, true, (ushort)i, 0);
				SetWall(Direction.right, true, (ushort)i, (ushort)(width - 1));
			}
			//up down
			for (int j = 0; j < width; ++j)
			{
				SetWall(Direction.up, true, 0, (ushort)j);
				SetWall(Direction.down, true, (ushort)(height - 1), (ushort)j);
			}
			//random start finish
			int dir = rand.Next(4);
			Point tmp;
			switch (dir)
			{
				//left
				case 0:
					tmp.x = 0;
					tmp.y = (ushort)rand.Next(height);
					start = tmp;
					SetWall(Direction.left, false, start.y, start.x);
					tmp.x = (ushort)(width - 1);
					tmp.y = (ushort)rand.Next(height);
					finish = tmp;
					SetWall(Direction.right, false, finish.y, finish.x);
					break;
				//up
				case 1:
					tmp.x = (ushort)rand.Next(width);
					tmp.y = 0;
					start = tmp;
					SetWall(Direction.up, false, start.y, start.x);
					tmp.x = (ushort)rand.Next(width);
					tmp.y = (ushort)(height - 1);
					finish = tmp;
					SetWall(Direction.down, false, finish.y, finish.x);
					break;
				//right
				case 2:
					tmp.x = (ushort)(width - 1);
					tmp.y = (ushort)rand.Next(height);
					start = tmp;
					SetWall(Direction.right, false, start.y, start.x);
					tmp.x = 0;
					tmp.y = (ushort)rand.Next(height);
					finish = tmp;
					SetWall(Direction.left, false, finish.y, finish.x);
					break;
				//down
				case 3:
					tmp.x = (ushort)rand.Next(width);
					tmp.y = (ushort)(height - 1);
					start = tmp;
					SetWall(Direction.down, false, start.y, start.x);
					tmp.x = (ushort)rand.Next(width);
					tmp.y = 0;
					finish = tmp;
					SetWall(Direction.up, false, finish.y, finish.x);
					break;
			}
			//Removing walls
			int WallsForRemove = rand.Next(Math.Max(width, height) / 2);
			for (int i = 0; i < WallsForRemove; ++i)
			{
				int x = rand.Next(1, width - 1);
				int y = rand.Next(1, height - 1);
				Direction direction = Direction.left;
				switch(rand.Next(4))
				{
					case 0:
						direction = Direction.left;
						break;
					case 1:
						direction = Direction.up;
						break;
					case 2:
						direction = Direction.right;
						break;
					case 3:
						direction = Direction.down;
						break;
				}
				SetWall(direction, false, (ushort)y, (ushort)x);
			}
		}
	}
}
