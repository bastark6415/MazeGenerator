using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using System.Windows.Media;
using MazeGenerator.Types;

namespace MazeGenerator.Searchers
{
	public class ModifiedDFS : Searcher
	{
		private bool[,] visited;
		public ModifiedDFS(Generator generator) : base(generator) { }
		public override void Search(bool showSteps, ref bool canDoNextStep)
		{
			visited = new bool[generator.height, generator.width];
			SetDeadBlocks();
			Path startPath = new Path();
			DFS(showSteps, ref canDoNextStep, generator.start, startPath);
		}
		private void DFS(bool showSteps, ref bool canDoNextStep, Point pnt, Path path)
		{
			visited[pnt.y, pnt.x] = true;
			path.AddPoint(pnt);
			if (pnt.x == generator.finish.x && pnt.y == generator.finish.y)
				paths.Add(new Path(path));
			else
			{
				Point nextPoint = pnt;
				if (!generator.mapMatrix[pnt.y, pnt.x].left	 && pnt.x > 0 && !visited[pnt.y, pnt.x - 1])
				{
					--nextPoint.x;
					DFS(showSteps, ref canDoNextStep, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].up	 && pnt.y > 0 && !visited[pnt.y - 1, pnt.x])
				{
					--nextPoint.y;
					DFS(showSteps, ref canDoNextStep, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].right && pnt.x < generator.width - 1 && !visited[pnt.y, pnt.x + 1])
				{
					++nextPoint.x;
					DFS(showSteps, ref canDoNextStep, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].down  && pnt.y < generator.height - 1 && !visited[pnt.y + 1, pnt.x])
				{
					++nextPoint.y;
					DFS(showSteps, ref canDoNextStep, nextPoint, path);
				}
			}
			visited[pnt.y, pnt.x] = false;
			path.RemovePoint(pnt);
		}
		private void SetBlankAsDeadBlock(ushort y, ushort x)
		{
			if (y >= visited.GetLength(0) || x >= visited.GetLength(1))
				return;
			if (visited[y, x])
				return;
			int k = 0;
			if (generator.mapMatrix[y, x].left	|| (x > 0					 && visited[y, x - 1])) ++k;
			if (generator.mapMatrix[y, x].up	|| (y > 0					 && visited[y - 1, x])) ++k;
			if (generator.mapMatrix[y, x].right || (x < generator.width - 1	 && visited[y, x + 1])) ++k;
			if (generator.mapMatrix[y, x].down	|| (y < generator.height - 1 && visited[y + 1, x])) ++k;
			if (k >= 3)
			{
				visited[y, x] = true;
				if (!generator.mapMatrix[y, x].left)	SetBlankAsDeadBlock(y, (ushort)(x - 1));
				if (!generator.mapMatrix[y, x].up)		SetBlankAsDeadBlock((ushort)(y - 1), x);
				if (!generator.mapMatrix[y, x].right)	SetBlankAsDeadBlock(y, (ushort)(x + 1));
				if (!generator.mapMatrix[y, x].down)	SetBlankAsDeadBlock((ushort)(y + 1), x);
			}
		}
		private void SetDeadBlocks()
		{
			for (int i = 0; i < visited.GetLength(0); ++i)
				for (int j = 0; j < visited.GetLength(1); ++j)
					SetBlankAsDeadBlock((ushort)i, (ushort)j);
		}
	}
}
