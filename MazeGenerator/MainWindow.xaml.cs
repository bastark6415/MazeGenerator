using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
using MazeGenerator.Generate;
using MazeGenerator.Searchers;

namespace MazeGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Generator generator;
		private bool canDoNextStep = true;
		private const int wallPx = 1;
		private const int cellPx = 8;
		public MainWindow()
		{
			InitializeComponent();
		}
		private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(UpDownHeight.Text) || string.IsNullOrEmpty(UpDownWidth.Text))
				return;
			generator = new EllerAlgorithm((ushort)UpDownHeight.Value, (ushort)UpDownWidth.Value);
			generator.Generate(CheckBoxSteps.IsChecked ?? false, ref canDoNextStep);
			PrintMaze(sender, e);
		}
		private void ButtonSearch_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(UpDownHeight.Text) || string.IsNullOrEmpty(UpDownWidth.Text))
				return;
			if (generator == null)
				ButtonGenerate_Click(sender, e);
			if (!(generator is Searcher))
				generator = new ModifiedDFS(generator);
			Searcher searcher = generator as Searcher;
			searcher.Search(CheckBoxSteps.IsChecked ?? false, ref canDoNextStep);
			PrintMaze(sender, e);
		}	
		private void PrintMaze(object sender, RoutedEventArgs e)
		{
			if (generator == null)
				return;
			ImageMaze.Source = generator.ToBitmap(wallPx, cellPx);
		}
		private void PrintMaze(object sender, RoutedEventArgs e, bool[] paths)
		{
			if (generator == null)
				return;
			 Searcher searcher = generator as Searcher;
			if (searcher == null)
				return;
			ImageMaze.Source = searcher.ToBitmap(wallPx, cellPx);
		}
	}
}
