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
		/// <value>
		///	Get generator
		/// </value>
		public Generator generator { get; private set; }
		/// <value>
		///	Hide basic variable. Get access to mapMatrix by reference on <c>Generator</c>
		/// </value>
		public new Cell[,] mapMatrix { get => generator.mapMatrix; private set { } }
		/// <value>
		///	Hide basic variable. Get access to start by reference on <c>Generator</c>
		/// </value>
		public new Point start { get => generator.start; private set { } }
		/// <value>
		///	Hide basic variable. Get access to finish by reference on <c>Generator</c>
		/// </value>
		public new Point finish { get => generator.finish; private set { } }
		/// <value>
		///	Hide basic variable. Get access to height by reference on <c>Generator</c>
		/// </value>
		public new int height { get => generator.height; private set { } }
		/// <value>
		///	Hide basic variable. Get access to width by reference on <c>Generator</c>
		/// </value>
		public new int width { get => generator.width; private set { } }
		/// <summary>
		/// Constructor of <c>GeneratorDecorator</c>.
		/// </summary>
		/// <param name="generator"><c>Generator</c> to decorate.</param>
		public GeneratorDecorator(Generator generator) : base(0, 0)
		{
			this.generator = generator ?? throw new ArgumentNullException();
		}
		/// <summary>
		///		GenerateAsync. This function is execute in separate thread.
		/// </summary>
		/// <remarks>
		/// <c>GeneratorDecorator</c> hides functionality of parent <c>Generator</c>
		/// </remarks>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Protress for showing step by step</param>
		/// <param name="signal">Variable for syncronization</param>
		/// <exception cref="System.NotImplementedException">Throws always when called GenerateAsync</exception>
		sealed protected override void GenerateAsync(CancellationToken token, IProgress<string> progress, ManualResetEvent signal)
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Start Generate Async
		/// </summary>
		/// <remarks>
		///		This function creates a new task with async thread
		///	</remarks>
		/// <param name="token">Cancellation Token for cancelling thread</param>
		/// <param name="progress">Progress for showing step by step execution</param>
		/// <param name="signal">Variable for syncronization</param>
		/// <returns>Returns new Task</returns>
		sealed public override Task Generate(CancellationToken token, IProgress<string> progress, ManualResetEvent signal) =>
			generator.Generate(token, progress, signal);
	}
}
