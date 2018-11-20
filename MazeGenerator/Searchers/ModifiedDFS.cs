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
		/// <summary>
		///		SearchAsync. This function is execute in separate thread.
		/// </summary>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Protress for showing step by step</param>
		/// <param name="signal">Variable for synchronization</param>
		/// <exception cref="System.ObjectDisposedException">Throws when token disposed</exception>
		protected override void SearchAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal)
		{
			paths.Clear();
			//Progress
			progress?.Report("Пошук...");
			signal?.Reset();
			signal?.WaitOne();
			try
			{
				DFS(token, progress, signal, generator.start, new Path());
			}
			catch (OperationCanceledException)
			{
				return;
			}
			catch (ObjectDisposedException ex)
			{
				progress?.Report("Помилка");
				signal?.Dispose();
				throw ex;
			}
			//Progress
			progress?.Report("Пошук закінчився");
			signal?.Dispose();
		}
		private void DFS(CancellationToken token, IProgress<string> progress, ManualResetEvent signal, Point pnt, Path path)
		{
			//Cancell
			try
			{
				token.ThrowIfCancellationRequested();
			}
			catch (OperationCanceledException ex)
			{
				throw ex;
			}
			catch (ObjectDisposedException ex)
			{
				throw ex;
			}
			//Marked that we visited this point and add it to path
			visited[pnt.y, pnt.x] = true;
			path.AddLast(pnt);
			//Progress
			if (signal != null)
			{
				LinkedList<Path> tmpPaths = paths;
				paths = new LinkedList<Path>();
				paths.AddLast(path);
				progress?.Report("Пошук...");
				signal.Reset();
				signal.WaitOne();
				paths = tmpPaths;
			}
			if (pnt == finish)
				paths.AddLast(new Path(path)); //Found path, add to list of paths and continue search;
			else
			{
				//Checking adjacent cells, if it is blenk go there
				if (!mapMatrix[pnt.y, pnt.x].left && pnt.x > 0 && !visited[pnt.y, pnt.x - 1])
					DFS(token, progress, signal, new Point(pnt.x - 1, pnt.y), path);
				if (!mapMatrix[pnt.y, pnt.x].up && pnt.y > 0 && !visited[pnt.y - 1, pnt.x])
					DFS(token, progress, signal, new Point(pnt.x, pnt.y - 1), path);
				if (!mapMatrix[pnt.y, pnt.x].right && pnt.x < width - 1 && !visited[pnt.y, pnt.x + 1])
					DFS(token, progress, signal, new Point(pnt.x + 1, pnt.y), path);
				if (!mapMatrix[pnt.y, pnt.x].down && pnt.y < height - 1 && !visited[pnt.y + 1, pnt.x])
					DFS(token, progress, signal, new Point(pnt.x, pnt.y + 1), path);
			}
			//Go back
			visited[pnt.y, pnt.x] = false;
			path.RemoveLast();
			//Progress
			if (signal != null)
			{
				LinkedList<Path> tmpPaths = paths;
				paths = new LinkedList<Path>();
				paths.AddLast(path);
				progress?.Report("Пошук...");
				signal.Reset();
				signal.WaitOne();
				paths = tmpPaths;
			}
		}
	}
}
