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
		public ModifiedDFS(Generator generator) : base(generator) { }

		protected override void SearchAsync(IProgress<string> progress, ManualResetEvent signal)
		{
			paths.Clear();
			//Progress
			progress?.Report("Started search");
			signal?.Reset();
			signal?.WaitOne();
			DFS(progress, signal, generator.start, new Path());
			//Progress
			progress?.Report("Search has ended");
			signal?.Dispose();
		}
		private void DFS(IProgress<string> progress, ManualResetEvent signal, Point pnt, Path path)
		{
			visited[pnt.y, pnt.x] = true;
			path.AddLast(pnt);
			//Progress
			if (signal != null)
			{
				LinkedList<Path> tmpPaths = paths;
				paths = new LinkedList<Path>();
				paths.AddLast(path);
				progress?.Report("Searching...");
				signal.Reset();
				signal.WaitOne();
				paths = tmpPaths;
			}
			if (pnt == finish)
				paths.AddLast(new Path(path));
			else
			{
				if (!mapMatrix[pnt.y, pnt.x].left && pnt.x > 0 && !visited[pnt.y, pnt.x - 1])
					DFS(progress, signal, new Point(pnt.x - 1, pnt.y), path);
				if (!mapMatrix[pnt.y, pnt.x].up && pnt.y > 0 && !visited[pnt.y - 1, pnt.x])
					DFS(progress, signal, new Point(pnt.x, pnt.y - 1), path);
				if (!mapMatrix[pnt.y, pnt.x].right && pnt.x < width - 1 && !visited[pnt.y, pnt.x + 1])
					DFS(progress, signal, new Point(pnt.x + 1, pnt.y), path);
				if (!mapMatrix[pnt.y, pnt.x].down && pnt.y < height - 1 && !visited[pnt.y + 1, pnt.x])
					DFS(progress, signal, new Point(pnt.x, pnt.y + 1), path);
			}
			visited[pnt.y, pnt.x] = false;
			path.RemoveLast();
			//Progress
			if (signal != null)
			{
				LinkedList<Path> tmpPaths = paths;
				paths = new LinkedList<Path>();
				paths.AddLast(path);
				progress?.Report("Searching...");
				signal.Reset();
				signal.WaitOne();
				paths = tmpPaths;
			}
		}
	}
}
