using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using System.Windows.Media;
using MazeGenerator.Types;
using System.Threading;

namespace MazeGenerator.Searchers
{
	public class ModifiedDFS : Searcher
	{
		private bool[,] visited;
		public ModifiedDFS(Generator generator) : base(generator) { }
		public override void Search(ref bool? canDoNextStep)
		{
			paths.Clear();
			visited = new bool[generator.height, generator.width];
			SetDeadBlocks();
			Path startPath = new Path();
			DFS(ref canDoNextStep, generator.start, startPath);
		}

		protected override void SearchAsync(IProgress<string> progress, ManualResetEvent signal)
		{
			paths.Clear();
			visited = new bool[generator.height, generator.width];
			SetDeadBlocks();
			Path startPath = new Path();
			//Progress
			progress?.Report($"Started search");
			signal?.Reset();
			signal?.WaitOne();
			DFS(progress, signal, generator.start, startPath);
			//Progress
			progress?.Report($"Search has ended");
			signal?.Dispose();
		}
		private void DFS(IProgress<string> progress, ManualResetEvent signal, Point pnt, Path path)
		{
			visited[pnt.y, pnt.x] = true;
			path.AddPoint(pnt);
			//Progress
			List<Path> tmpPaths = paths;
			paths = new List<Path>();
			paths.Add(path);
			progress?.Report($"Searching...");
			signal?.Reset();
			signal?.WaitOne();
			paths = tmpPaths;
			if (pnt.x == generator.finish.x && pnt.y == generator.finish.y)
				paths.Add(new Path(path));
			else
			{
				if (!generator.mapMatrix[pnt.y, pnt.x].left && pnt.x > 0 && !visited[pnt.y, pnt.x - 1])
				{
					Point nextPoint = pnt;
					--nextPoint.x;
					DFS(progress, signal, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].up && pnt.y > 0 && !visited[pnt.y - 1, pnt.x])
				{
					Point nextPoint = pnt;
					--nextPoint.y;
					DFS(progress, signal, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].right && pnt.x < generator.width - 1 && !visited[pnt.y, pnt.x + 1])
				{
					Point nextPoint = pnt;
					++nextPoint.x;
					DFS(progress, signal, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].down && pnt.y < generator.height - 1 && !visited[pnt.y + 1, pnt.x])
				{
					Point nextPoint = pnt;
					++nextPoint.y;
					DFS(progress, signal, nextPoint, path);
				}
			}
			visited[pnt.y, pnt.x] = false;
			path.RemovePoint(pnt);
			//Progress
			tmpPaths = paths;
			paths = new List<Path>();
			paths.Add(path);
			progress?.Report($"Searching...");
			signal?.Reset();
			signal?.WaitOne();
			paths = tmpPaths;
		}
		private void DFS(ref bool? canDoNextStep, Point pnt, Path path)
		{
			visited[pnt.y, pnt.x] = true;
			path.AddPoint(pnt);
			if (pnt.x == generator.finish.x && pnt.y == generator.finish.y)
				paths.Add(new Path(path));
			else
			{
				if (!generator.mapMatrix[pnt.y, pnt.x].left	 && pnt.x > 0 && !visited[pnt.y, pnt.x - 1])
				{
					Point nextPoint = pnt;
					--nextPoint.x;
					DFS(ref canDoNextStep, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].up	 && pnt.y > 0 && !visited[pnt.y - 1, pnt.x])
				{
					Point nextPoint = pnt;
					--nextPoint.y;
					DFS(ref canDoNextStep, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].right && pnt.x < generator.width - 1 && !visited[pnt.y, pnt.x + 1])
				{
					Point nextPoint = pnt;
					++nextPoint.x;
					DFS(ref canDoNextStep, nextPoint, path);
				}
				if (!generator.mapMatrix[pnt.y, pnt.x].down  && pnt.y < generator.height - 1 && !visited[pnt.y + 1, pnt.x])
				{
					Point nextPoint = pnt;
					++nextPoint.y;
					DFS(ref canDoNextStep, nextPoint, path);
				}
			}
			visited[pnt.y, pnt.x] = false;
			path.RemovePoint(pnt);
		}
		private void SetBlankAsDeadBlock(int y, int x)
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
				if (!generator.mapMatrix[y, x].left)	SetBlankAsDeadBlock(y, (int)(x - 1));
				if (!generator.mapMatrix[y, x].up)		SetBlankAsDeadBlock((int)(y - 1), x);
				if (!generator.mapMatrix[y, x].right)	SetBlankAsDeadBlock(y, (int)(x + 1));
				if (!generator.mapMatrix[y, x].down)	SetBlankAsDeadBlock((int)(y + 1), x);
			}
		}
		private void SetDeadBlocks()
		{
			for (int i = 0; i < visited.GetLength(0); ++i)
				for (int j = 0; j < visited.GetLength(1); ++j)
					SetBlankAsDeadBlock((int)i, (int)j);
		}
	}
}
