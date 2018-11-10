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
		sealed public override void Generate(bool showSteps, bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
	}
}
