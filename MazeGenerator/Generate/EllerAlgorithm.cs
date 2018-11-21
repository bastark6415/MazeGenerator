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
		/// <summary>
		/// Constructor of <c>EllerAlgorithm</c>
		/// </summary>
		/// <remarks>
		/// Fills fields of <c>EllerAlgorithm</c>
		///	</remarks>
		/// <param name="height">An integer value</param>
		/// <param name="width"> An integer value></param>
		/// <exception cref="System.ArgumentNullException">Throws if sizes are not suitable</exception>
		public EllerAlgorithm(int height, int width) : base(height, width) { }
		/// <summary>
		///		GenerateAsync. This function is execute in separate thread.
		/// </summary>
		/// <remarks>
		///		This method realized Eller's algorithm for generating.
		///		Possible message to progress: Generating..., Generated, Crashed.
		/// </remarks>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Protress for showing step by step</param>
		/// <param name="signal">Variable for syncronization</param>
		/// <exception cref="System.ObjectDisposedException">Throws when token disposed</exception>
		protected override void GenerateAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal)
		{
			//Current row with numbers of sets
			int[] currRow = new int[width];
			//Set
			bool[] usedSet = new bool[width + 1];
			Random rand = new Random();
			//External walls
			//Left and Right
			for (int i = 0; i < height; ++i)
			{
				SetWall(Direction.left, true, i, 0);
				SetWall(Direction.right, true, i, width - 1);
			}
			//Up and Down
			for (int j = 0; j < width; ++j)
			{
				SetWall(Direction.up, true, 0, j);
				SetWall(Direction.down, true, height - 1, j);
			}
			//Progress
			progress?.Report("Генерація...");
			signal?.Reset();
			signal?.WaitOne();
			//Internal walls
			for (int i = 0; i < height; ++i)
			{
				//Cancell
				try
				{
					token.ThrowIfCancellationRequested();
				}
				catch (OperationCanceledException)
				{
					return;
				}
				catch (ObjectDisposedException)
				{
					progress?.Report("Помилка");
					signal?.Dispose();
					throw;
				}
				//Assign for each cell unique set if it hasn't
				for (int j = 0; j < width; ++j)
					if (currRow[j] == 0)
						for (int k = 1; k <= width; ++k)
							if (!usedSet[k])
							{
								usedSet[k] = true;
								currRow[j] = k;
								break;
							}
				//Create right walls
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
						progress?.Report("Генерація...");
						signal.Reset();
						signal.WaitOne();
					}
				}
				//Create down walls
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
						progress?.Report("Генерація...");
						signal.Reset();
						signal.WaitOne();
					}
				}
				//Update used set
				for (int j = 1; j <= width; ++j)
					usedSet[j] = false;
				//Remove cells with down walls from set
				for (int j = 0; j < width; ++j)
					if (mapMatrix[i, j].down)
						currRow[j] = 0;
					else
						usedSet[currRow[j]] = true;
			}
			//Random walls removing
			if (height > 2 && width > 2)
			{
				int cntWalls = 1;
				if (height * width > 25000)
					cntWalls = 125;
				else if (height * width > 200)
					cntWalls = (int)Math.Round(height * width * 0.005);
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
						progress?.Report("Генерація...");
						signal.Reset();
						signal.WaitOne();
					}
				}
			}
			//Random Start & Finish
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
			progress?.Report("Згенеровано");
			signal?.Dispose();
		}
	}
}
