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
    public abstract class Searcher : GeneratorDecorator
    {
		public LinkedList<Path> paths { get; protected set; }
		protected bool[,] visited;
		public Searcher(Generator generator) : base(generator)
		{
			paths = new LinkedList<Path>();
			visited = new bool[height, width];
			SetDeadBlocks();
		}
		public virtual Task Search(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			Task.Run(() => SearchAsync(token,progress, signal));
		protected abstract void SearchAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal);
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
		protected void SetDeadBlocks()
		{
			for (int i = 0; i < height; ++i)
				for (int j = 0; j < width; ++j)
					SetBlankAsDeadBlock(i, j);
		}
	}
}
