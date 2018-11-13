using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Types;

namespace MazeGenerator.Generate
{
    public abstract class GeneratorDecorator : Generator
    {
		public Generator generator { get; protected set; }
		public GeneratorDecorator(Generator generator) : base(0, 0)
		{
			this.generator = generator;
		}
		public override void Action(bool showSteps, ref bool canDoNextStep) => generator.Action(showSteps, ref canDoNextStep);
		public override BitmapSource ToBitmap(int wallPx, int cellPx) => generator.ToBitmap(wallPx, cellPx);
		sealed public override void Generate(bool showSteps, ref bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
	}
}
