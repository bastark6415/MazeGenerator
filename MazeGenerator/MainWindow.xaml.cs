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
		private const int wallPx = 1;
		private const int cellPx = 4;
		private Generator generator;
		private CancellationTokenSource cancellationToken = new CancellationTokenSource();
		private ManualResetEvent signal;
		private BitmapSource bitmap;
		public MainWindow()
		{
			InitializeComponent();
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
		private void OnPreGenerating()
		{
			if (CheckBoxSteps.IsChecked ?? false)
				SetStyle(ButtonGenerate, Resources["ButtonNextStepStyle"] as Style);
			else
				SetIsEnabled(ButtonGenerate, false);
			SetIsEnabled(CheckBoxSteps, false);
			SetIsEnabled(ButtonSearch, false);
			SetIsEnabled(UpDownHeight, false);
			SetIsEnabled(UpDownWidth, false);
			SetIsEnabled(MenuItemExportTo, false);
			SetIsEnabled(ButtonCancel, true);
		}
		private void OnGenerated()
		{
			SetIsEnabled(ButtonCancel, false);
			SetStyle(ButtonGenerate, Resources["ButtonGenerateStyle"] as Style);
			SetIsEnabled(ButtonGenerate, true);
			SetIsEnabled(ButtonSearch, true);
			SetIsEnabled(MenuItemExportTo, true);
			SetIsEnabled(CheckBoxSteps, true);
			SetIsEnabled(UpDownWidth, true);
			SetIsEnabled(UpDownHeight, true);
		}
		private void OnCancel()
		{
			signal?.Set();
			cancellationToken = null;
			SetIsEnabled(ButtonCancel, false);
			SetStyle(ButtonGenerate, Resources["ButtonGenerateStyle"] as Style);
			if (generator is Searcher)
			{
				SetStyle(ButtonSearch, Resources["ButtonSearchStyle"] as Style);
				SetIsEnabled(ButtonSearch, true);
			}
			SetIsEnabled(ButtonGenerate, true);
			SetIsEnabled(MenuItemExportTo, true);
			SetIsEnabled(CheckBoxSteps, true);
			SetIsEnabled(UpDownWidth, true);
			SetIsEnabled(UpDownHeight, true);
			ImageMaze.Source = generator.ToBitmap(wallPx, cellPx);
		}
		private void ButtonSearch_Click(object sender, RoutedEventArgs e)
		{

		}
		private void OnNextStep(string msg)
		{
			TextBlockStatus.Text = msg;
			if (msg == "Generated")
				OnGenerated();
			if (signal != null || msg == "Search has ended" || msg == "Generated")
				//bitmap = generator.ToBitmap(wallPx, cellPx);
				ImageMaze.Source = generator.ToBitmap(wallPx, cellPx);
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

		}

		private void MenuItemSettings_Click(object sender, RoutedEventArgs e)
		{

		}

		private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
		{

		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			cancellationToken.Cancel(true);
			OnCancel();
		}
	}
}
