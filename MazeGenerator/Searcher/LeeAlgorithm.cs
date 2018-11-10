using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MazeGenerator.Types;
using MazeGenerator.Generate;

namespace MazeGenerator.Searcher
{
    public class LeeAlgorithm : Searcher
    {
		public LeeAlgorithm(Generator generator) : base(generator) { }
		public override void Search(bool showSteps, ref bool canDoNextStep)
		{
			throw new NotImplementedException();
		}
	}
}
