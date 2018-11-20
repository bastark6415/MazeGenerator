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
using MazeGenerator.Additional;
using System.ComponentModel;

namespace MazeGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Generator generator;
		private CancellationTokenSource cancellationToken;
		private ManualResetEvent signal;
		private ConverterToBitmap converter { get; set; } = new ConverterToBitmap(1, 4);
		public MainWindow()
		{
			InitializeComponent();
			ImageMaze.DataContext = converter;
		}
		private void OnPreGenerating()
		{
			if (CheckBoxSteps.IsChecked ?? false)
				SetStyle(ButtonGenerate, (Style)Resources["ButtonNextStepStyle"]);
			else
				SetIsEnabled(ButtonGenerate, false);
			SetIsEnabled(CheckBoxSteps, false);
			SetIsEnabled(ButtonSearch, false);
			SetIsEnabled(UpDownHeight, false);
			SetIsEnabled(UpDownWidth, false);
			SetIsEnabled(MenuItemExportTo, false);
			SetIsEnabled(ButtonCancel, true);
			TextBlockPaths.Text = "";
			TextBlockSizes.Text = "";
		}
		private void OnGenerated()
		{
			SetStyle(ButtonGenerate, (Style)Resources["ButtonGenerateStyle"]);
			SetIsEnabled(ButtonGenerate, true);
			SetIsEnabled(ButtonSearch, true);
			SetIsEnabled(CheckBoxSteps, true);
			SetIsEnabled(UpDownHeight, true);
			SetIsEnabled(UpDownWidth, true);
			SetIsEnabled(MenuItemExportTo, true);
			SetIsEnabled(ButtonCancel, false);
			TextBlockSizes.Text = $"{generator.height} x {generator.width}";
			converter.Convert((dynamic)generator);
			UpdateListOfPathes();
		}
		private void OnPreSearch()
		{
			if (CheckBoxSteps.IsChecked ?? false)
				SetStyle(ButtonSearch, (Style)Resources["ButtonNextStepStyle"]);
			else
				SetIsEnabled(ButtonSearch, false);
			SetIsEnabled(ButtonGenerate, false);
			SetIsEnabled(CheckBoxSteps, false);
			SetIsEnabled(UpDownHeight, false);
			SetIsEnabled(UpDownWidth, false);
			SetIsEnabled(MenuItemExportTo, false);
			SetIsEnabled(ButtonCancel, true);
			TextBlockPaths.Text = "";
			UpdateListOfPathes();
		}
		private void OnEndSearch()
		{
			SetStyle(ButtonSearch, (Style)Resources["ButtonSearchStyle"]);
			SetIsEnabled(ButtonGenerate, true);
			SetIsEnabled(ButtonSearch, true);
			SetIsEnabled(MenuItemExportTo, true);
			SetIsEnabled(CheckBoxSteps, true);
			SetIsEnabled(UpDownWidth, true);
			SetIsEnabled(UpDownHeight, true);
			SetIsEnabled(ButtonCancel, false);
			TextBlockPaths.Text = $"Paths: {((Searcher)generator).paths.Count}";
			converter.Convert((dynamic)generator);
			UpdateListOfPathes();
		}
		private void OnCancel()
		{
			signal?.Set();
			cancellationToken = null;
			SetIsEnabled(ButtonCancel, false);
			SetStyle(ButtonGenerate, (Style)Resources["ButtonGenerateStyle"]);
			if (generator is Searcher)
			{
				SetStyle(ButtonSearch, (Style)Resources["ButtonSearchStyle"]);
				SetIsEnabled(ButtonSearch, true);
			}
			SetIsEnabled(ButtonGenerate, true);
			SetIsEnabled(MenuItemExportTo, true);
			SetIsEnabled(CheckBoxSteps, true);
			SetIsEnabled(UpDownWidth, true);
			SetIsEnabled(UpDownHeight, true);
			TextBlockStatus.Text = "Canceled";
			TextBlockPaths.Text = "";
			TextBlockSizes.Text = "";
			converter.Convert((dynamic)generator);
		}
		private void OnNextStep(string msg)
		{
			TextBlockStatus.Text = msg;
			if (msg == "Generated")
				OnGenerated();
			else if (msg == "Search has ended")
				OnEndSearch();
			else if (signal != null)
				converter.Convert((dynamic)generator);
		}
		private void UpdateListOfPathes()
		{
			ListBoxPaths.ItemsSource = null;
			Searcher searcher = generator as Searcher;
			if (searcher == null)
				return;
			ListBoxItem[] items = new ListBoxItem[searcher.paths.Count];
			for (int i = 0; i < items.Length; ++i)
			{
				ListBoxItem lbi = new ListBoxItem();
				CheckBox check = new CheckBox();
				lbi.Margin = new Thickness(2, 2, 2, 0);
				check.Content = $"Path {i + 1}";
				check.IsChecked = true;
				check.Click += ChangeVisiblePaths;
				lbi.Content = check;
				items[i] = lbi;
			}
			ListBoxPaths.ItemsSource = items;
		}
		private void ButtonNextStep_Click(object sender, RoutedEventArgs e)
		{
			signal.Set();
		}
		private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
		{
			OnPreGenerating();
			//Generating
			generator = new EllerAlgorithm((int)UpDownHeight.Value, (int)UpDownWidth.Value);
			Progress<string> progress = new Progress<string>((msg) => OnNextStep(msg));
			cancellationToken = new CancellationTokenSource();
			signal = (bool)CheckBoxSteps.IsChecked ? new ManualResetEvent(false) : null;
			generator.Generate(cancellationToken.Token, progress, signal);
		}
		private void ButtonSearch_Click(object sender, RoutedEventArgs e)
		{
			OnPreSearch();
			//Search
			if (!(generator is Searcher))
				generator = new ModifiedDFS(generator);
			Action<string> action;
			if ((bool)CheckBoxSteps.IsChecked)
			{
				action = msg => { OnNextStep(msg); UpdateListOfPathes(); };
				signal = new ManualResetEvent(false);
			}
			else
			{
				action = msg => OnNextStep(msg);
				signal = null;
			}
			Progress<string> progress = new Progress<string>(action);
			cancellationToken = new CancellationTokenSource();
			(generator as Searcher).Search(cancellationToken.Token, progress, signal);
		}
		private void ChangeVisiblePaths(object sender, RoutedEventArgs e)
		{
			ItemCollection items = ListBoxPaths.Items;
			bool[] paths = new bool[items.Count];
			for (int i = 0; i < paths.Length; ++i)
				paths[i] = (bool)((items[i] as ListBoxItem).Content as CheckBox).IsChecked;
			converter.Convert((dynamic)generator, paths);
		}
		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			cancellationToken.Cancel();
			OnCancel();
		}
		private void UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) =>
			SetIsEnabled(ButtonGenerate, UpDownHeight?.Value != null && UpDownWidth?.Value != null);
		private void SetIsEnabled(Control control, bool IsEnabled)
		{
			if (control != null)
				control.IsEnabled = IsEnabled;
		}
		private void SetStyle(Control control, Style style)
		{
			if (control != null)
				control.Style = style;
		}
		private void MenuItemBitmap_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.AddExtension = true;
			dialog.DefaultExt = ".bmp";
			dialog.Filter = "Bitmap image (*.bmp)|*.bmp";
			dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (dialog.ShowDialog() == true)
				converter.SaveToFile(dialog.FileName);
		}
		private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MenuItemExit_Click(object sender, RoutedEventArgs e)
		{
			cancellationToken?.Cancel();
			Close();
		}
	}
}
