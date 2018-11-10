using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Generate;
using MazeGenerator.Types;

namespace MazeGenerator.Searcher
{
    public abstract class Searcher : GeneratorDecorator
    {
		public List<Path> paths { get; protected set; }
		public Searcher(Generator generator) : base(generator)
		{
			paths = new List<Path>();
		}
		public override void Action(bool showSteps, ref bool canDoNextStep)
		{
			base.Action(showSteps, ref canDoNextStep);
		}
		public abstract void Search(bool showSteps, ref bool canDoNextStep);
		public override BitmapSource ToBitmap()
		{
			return base.ToBitmap();
		}
		public virtual BitmapSource ToBitmap(int[] paths)
		{
			throw new NotImplementedException();
		}
	}
}
