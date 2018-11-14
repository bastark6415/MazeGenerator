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
using System.IO;
using Microsoft.Win32;
using System.Runtime.Serialization.Formatters.Binary;

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
			ListBoxPaths.ItemsSource = null;
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
			ListBoxItem[] items = new ListBoxItem[searcher.paths.Count];
			for (int i = 0; i < items.Length; ++i)
			{
				ListBoxItem tmp = new ListBoxItem();
				CheckBox check = new CheckBox();
				tmp.Margin = new Thickness(2, 2, 2, 0);
				check.Content = $"Path {i + 1}";
				check.IsChecked = true;
				check.Click += ListBoxPathsChange;
				tmp.Content = check;
				items[i] = tmp;
			}
			ListBoxPaths.ItemsSource = items;
		}	
		private void PrintMaze(object sender, RoutedEventArgs e)
		{
			if (generator == null)
				return;
			ImageMaze.Source = generator.ToBitmap(wallPx, cellPx);
		}
		private void ListBoxPathsChange(object sender, RoutedEventArgs e)
		{
			ItemCollection items = ListBoxPaths.Items;
			bool[] paths = new bool[items.Count];
			for (int i = 0; i < paths.Length; ++i)
				paths[i] = (bool)((items[i] as ListBoxItem).Content as CheckBox).IsChecked;
			ImageMaze.Source = (generator as Searcher).ToBitmap(wallPx, cellPx, paths);
		}
		private void MenuItemBitmap_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.AddExtension = true;
			dialog.DefaultExt = ".bmp";
			dialog.Filter = "Bitmap image (*.bmp)|*.bmp";
			dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (dialog.ShowDialog() == true)
			{
				BmpBitmapEncoder encoder = new BmpBitmapEncoder();
				encoder.Frames.Add(BitmapFrame.Create(ImageMaze.Source as BitmapSource));
				using (FileStream stream = new FileStream(dialog.FileName, FileMode.Create))
					encoder.Save(stream);
			}
		}
	}
}
