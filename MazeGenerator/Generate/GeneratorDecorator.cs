using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Generate
{
    public abstract class GeneratorDecorator : Generator
    {
		public Generator generator { get; protected set; }
		public GeneratorDecorator(Generator generator) : base(generator.height, generator.width)
		{
			this.generator = generator;
		}
		public override void Action(bool showSteps, ref bool canDoNextStep)
		{
			generator.Action(showSteps, ref canDoNextStep);
			mapMatrix = generator.mapMatrix;
			start = generator.start;
			finish = generator.finish;
		}
		sealed public override void Generate(bool showSteps, ref bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
	}
}
