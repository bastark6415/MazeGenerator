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

namespace MazeGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
		{
			MazeGenerator.Generate.Generator generator = new MazeGenerator.Generate.EllerAlgorithm(
				(ushort)UpDownHeight.Value, (ushort)UpDownWidth.Value);
			bool tmp = true;
			generator.Generate(false, ref tmp);
			ImageMaze.Source = generator.ToBitmap();
			using (var fs = new System.IO.FileStream(@"C:\Users\bastark\Downloads\1.bmp", System.IO.FileMode.Create))
			{
				var encoder = new BmpBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(generator.ToBitmap()));
				encoder.Save(fs);
			}
		}
	}
}
