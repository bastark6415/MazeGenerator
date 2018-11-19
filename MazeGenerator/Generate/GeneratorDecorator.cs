using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MazeGenerator.Types;

namespace MazeGenerator.Generate
{
    public abstract class GeneratorDecorator : Generator
    {
		public Generator generator { get; private set; }
		public new Cell[,] mapMatrix { get => generator.mapMatrix; private set { } }
		public new Point start { get => generator.start; private set { } }
		public new Point finish { get => generator.finish; private set { } }
		public new int height { get => generator.height; private set { } }
		public new int width { get => generator.width; private set { } }
		public GeneratorDecorator(Generator generator) : base(0, 0)
		{
			this.generator = generator;
		}
		sealed protected override void GenerateAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal)
		{
			throw new NotImplementedException();
		}
		sealed public override Task Generate(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			generator.Generate(token, progress, signal);
	}
}
