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
using System.Threading;

namespace MazeGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Generator generator;
		private bool? canDoNextStep = true;
		private const int wallPx = 1;
		private const int cellPx = 4;
		CancellationTokenSource cancelSource = new CancellationTokenSource();
		public MainWindow()
		{
			InitializeComponent();
		}
		ManualResetEvent signal;
		private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(UpDownHeight.Text) || string.IsNullOrEmpty(UpDownWidth.Text))
				return;
			generator = new EllerAlgorithm((int)UpDownHeight.Value, (int)UpDownWidth.Value);
			Progress<string> progress = new Progress<string>(s => OnNextStep(s));
			ListBoxPaths.ItemsSource = null;
			signal = (bool)CheckBoxSteps.IsChecked ? new ManualResetEvent(false) : null;
			Task task = generator.Generate(cancelSource.Token, progress, signal);
		}
		private void OnNextStep(string msg)
		{
			//System.Windows.MessageBox.Show(msg);
			//To Status Bar
			if ((bool)CheckBoxSteps.IsChecked || msg == "Search has ended" || msg == "Generated")
				ImageMaze.Source = generator.ToBitmap(wallPx, cellPx);
		}
		private void ButtonSearch_Click(object sender, RoutedEventArgs e)
		{
			if (generator == null)
				return;
			if (!(generator is Searcher))
				generator = new ModifiedDFS(generator);
			Searcher searcher = generator as Searcher;
			Progress<string> progress = new Progress<string>(s => { OnNextStep(s); UpdatePathsList(s); });
			signal = (bool)CheckBoxSteps.IsChecked ? new ManualResetEvent(false) : null;
			searcher.Search(cancelSource.Token, progress, signal);
			UpdatePathsList("Search has ended");
		}
		private void UpdatePathsList(string s)
		{
			if (!(bool)CheckBoxSteps.IsChecked && s != "Search has ended")
				return;
			Searcher searcher = generator as Searcher;
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

		private void MenuItem_Click(object sender, RoutedEventArgs e)
		{
			if (signal != null && !signal.SafeWaitHandle.IsClosed) signal.Set();
		}

		private void ButtCancell_Click(object sender, RoutedEventArgs e)
		{
			cancelSource.Cancel();
		}
	}
}
