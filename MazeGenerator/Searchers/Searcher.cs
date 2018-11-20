using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using MazeGenerator.Types;
using System.Windows.Media;
using System.Threading;

namespace MazeGenerator.Searchers
{
	/// <summary>
	/// <c>Searcher</c> is a base abstract class that decorate "Search" for <c>Generator</c>
	/// </summary>
	/// <remarks>
	///	<c>Searcher</c> has a LinkedList of <c>Path</c>.
	///	<c>Searcher</c> doesn't implement search.
	/// </remarks>
	public abstract class Searcher : GeneratorDecorator
    {
		/// <value>
		/// Get LinkedList of paths
		/// </value>
		public LinkedList<Path> paths { get; protected set; }
		protected bool[,] visited;
		/// <summary>
		/// Constructor of <c>Searcher</c>
		/// </summary>
		/// <remarks>
		/// Precalculate ways for search
		/// </remarks>
		/// <param name="generator"></param>
		public Searcher(Generator generator) : base(generator)
		{
			paths = new LinkedList<Path>();
			visited = new bool[height, width];
			SetDeadBlocks();
		}
		/// <summary>
		/// Start Search Async
		/// </summary>
		/// <remarks>
		///		This function creates a new task with async thread. If signal is null steps will skip
		///	</remarks>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Progress for showing step by step execution</param>
		/// <param name="signal">Variable for syncronization</param>
		/// <returns>Returns new Task</returns>
		public virtual Task Search(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			Task.Run(() => SearchAsync(token,progress, signal));
		/// <summary>
		///		SearchAsync. This function is execute in separate thread.
		/// </summary>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Protress for showing step by step></param>
		/// <param name="signal">Variable for synchronization</param>
		/// <exception cref="System.ObjectDisposedException">Throws when token disposed</exception>
		protected abstract void SearchAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal);
		/// <summary>
		///  Function sets blank as dead bock and repeat recursivelly.
		/// </summary>
		/// <remarks>
		/// Counting walls and already dead blocks.
		/// If summ of these number more than 3, mark this block as dead
		/// and repead recurcivelly for adjacent(if possible to go there) blocks.
		/// If current block already marked as dead, return recursion.
		/// </remarks>
		/// <param name="y">An Int</param>
		/// <param name="x">An Int</param>
		protected void SetBlankAsDeadBlock(int y, int x)
		{
			if (visited[y, x])
				return;
			int k = 0;
			if (mapMatrix[y, x].left  || (x > 0 && visited[y, x - 1])) ++k;
			if (mapMatrix[y, x].up	  || (y > 0 && visited[y - 1, x])) ++k;
			if (mapMatrix[y, x].right || (x < width - 1 && visited[y, x + 1])) ++k;
			if (mapMatrix[y, x].down  || (y < height - 1 && visited[y + 1, x])) ++k;
			if (k >= 3)
			{
				visited[y, x] = true;
				if (!mapMatrix[y, x].left	&& x > 0)			SetBlankAsDeadBlock(y, x - 1);
				if (!mapMatrix[y, x].up		&& y > 0)			SetBlankAsDeadBlock(y - 1, x);
				if (!mapMatrix[y, x].right	&& x < width - 1)	SetBlankAsDeadBlock(y, x + 1);
				if (!mapMatrix[y, x].down	&& y < height - 1)	SetBlankAsDeadBlock(y + 1, x);
			}
		}
		/// <summary>
		/// Find dead block as more, as possible
		/// </summary>
		/// <remarks>
		///	For each block start recursion.
		/// </remarks>
		protected void SetDeadBlocks()
		{
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					SetBlankAsDeadBlock(i, j);
		}
	}
}
