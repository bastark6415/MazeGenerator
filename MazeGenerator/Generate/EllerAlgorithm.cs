using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeGenerator.Generate
{
    public class EllerAlgorithm : Generator
    {
		public EllerAlgorithm(ushort height, ushort width) : base(height, width) { }
		public override void Generate(bool showSteps, bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
	}
}
