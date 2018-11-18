using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MazeGenerator.Generate;
using MazeGenerator.Types;

namespace MazeGenerator.Searchers
{
	class ModifiedBFS : Searcher
	{
		public ModifiedBFS(Generator generator) : base(generator) { }
		protected override void SearchAsync(IProgress<string> progress, ManualResetEvent signal)
		{
			//Progress
			progress?.Report("Started search");
			signal?.Reset();
			signal?.WaitOne();
			//Search
			paths.Clear();
			BFS(progress, signal);
			//Progress
			progress?.Report("Search has ended");
			signal?.Dispose();
		}
		private void BFS(IProgress<string> progress, ManualResetEvent signal)
		{
			//Initialize path
			LinkedList<Path> allPossiblePaths = new LinkedList<Path>();
			Path tmp = new Path();
			tmp.AddLast(start);
			allPossiblePaths.AddLast(tmp);
			//Search
			while(allPossiblePaths.Count != 0)
			{
				tmp = allPossiblePaths.First.Value;
				allPossiblePaths.RemoveFirst();
				Point lastPoint = tmp.path.Last.Value;
				if (lastPoint.x == generator.finish.x && lastPoint.y == generator.finish.y)
					paths.AddLast(tmp);
				else
				{
					//left
					if (!mapMatrix[lastPoint.y, lastPoint.x].left && lastPoint.x > 0 &&
						IsNotVisited(tmp, new Point (lastPoint.x - 1, lastPoint.y )))
					{
						Path p = new Path(tmp);
						p.AddLast(new Point (lastPoint.x - 1, lastPoint.y ));
						allPossiblePaths.AddLast(p);
					}
					//up
					if (!mapMatrix[lastPoint.y, lastPoint.x].up && lastPoint.y > 0 &&
						IsNotVisited(tmp, new Point(lastPoint.x, lastPoint.y - 1)))
					{
						Path p = new Path(tmp);
						p.AddLast(new Point (lastPoint.x, lastPoint.y - 1));
						allPossiblePaths.AddLast(p);
					}
					//right
					if (!mapMatrix[lastPoint.y, lastPoint.x].right && lastPoint.x < width - 1 &&
						IsNotVisited(tmp, new Point (lastPoint.x + 1,lastPoint.y )))
					{
						Path p = new Path(tmp);
						p.AddLast(new Point (lastPoint.x + 1, lastPoint.y));
						allPossiblePaths.AddLast(p);
					}
					//down
					if (!mapMatrix[lastPoint.y, lastPoint.x].down && lastPoint.y < height - 1 &&
						IsNotVisited(tmp, new Point (lastPoint.x, lastPoint.y + 1)))
					{
						Path p = new Path(tmp);
						p.AddLast(new Point (lastPoint.x, lastPoint.y + 1));
						allPossiblePaths.AddLast(p);
					}
					//Progress
					if (signal != null)
					{
						LinkedList<Path> tmpPaths = paths;
						paths = allPossiblePaths;
						progress?.Report("Searching...");
						signal.Reset();
						signal.WaitOne();
						paths = tmpPaths;
					}
				}
			}
		}
		private bool IsNotVisited(Path p, Point point) => !p.ContainsPoint(point) && !visited[point.y, point.x];
	}
}
