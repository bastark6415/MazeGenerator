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
using System.Security;

namespace MazeGenerator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly int wallPx = 1;
		private readonly int cellPx = 3;
		private Generator generator;
		//Object for cancell operation
		private CancellationTokenSource cancellationToken;
		//Object for synchronization threads
		private ManualResetEvent signal; 
		//Converter from Maze to BitmapSource
		private ConverterToBitmap converter { get; set; }
		/// <summary>
		/// Constructor that initialize <c>Main Window</c> and <c>Converter</c>
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			try
			{
				converter = new ConverterToBitmap(wallPx, cellPx);
			}
			catch (ArgumentOutOfRangeException)
			{
				System.Windows.MessageBox.Show("Неможливо створити об'єкт перетворення лабіринту в зображення." +
	"Вказано недопустимі значення кількості пікселів. Мають бути додатні.",
						"Об'єкт перетворення лабіринту", MessageBoxButton.OK, MessageBoxImage.Error);
				Close();
			}
			ImageMaze.DataContext = converter;
		}
		/// <summary>
		/// UI Interaction before generating
		/// </summary>
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
		/// <summary>
		/// UI interaction after generating
		/// </summary>
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
			ConvertToBitmapAndCatchException();
			UpdateListOfPathes();
		}
		/// <summary>
		/// UI Interaction before search
		/// </summary>
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
		/// <summary>
		/// UI Interaction after search
		/// </summary>
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
			if (((Searcher)generator).paths.Count == 0)
				System.Windows.MessageBox.Show("Шляхів не існує");
			TextBlockPaths.Text = $"Шляхів: {((Searcher)generator).paths.Count}";
			ConvertToBitmapAndCatchException();
			UpdateListOfPathes();
		}
		/// <summary>
		/// UI Interaction after cancel
		/// </summary>
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
			TextBlockStatus.Text = "Скасовано";
			TextBlockPaths.Text = "";
			TextBlockSizes.Text = "";
			ConvertToBitmapAndCatchException();
			UpdateListOfPathes();
		}
		/// <summary>
		/// UI Interacton after catching exception from generateAsync or searchAsync
		/// </summary>
		private void OnCrashed()
		{
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
			TextBlockSizes.Text = $"{generator.height} x {generator.width}";
			TextBlockPaths.Text = "";
			ConvertToBitmapAndCatchException();
		}
		/// <summary>
		/// Progress changed
		/// </summary>
		/// <param name="msg">String to out in status bar</param>
		private void OnNextStep(string msg)
		{
			TextBlockStatus.Text = msg;
			if (msg == "Помилка")
				OnCrashed();
			else if (msg == "Згенеровано")
				OnGenerated();
			else if (msg == "Пошук закінчився")
				OnEndSearch();
			else if (signal != null)
				ConvertToBitmapAndCatchException();
		}
		/// <summary>
		/// Update List of paths on form
		/// </summary>
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
				check.Content = $"Шлях {i + 1}";
				check.IsChecked = true;
				check.Click += ChangeVisiblePaths;
				lbi.Content = check;
				items[i] = lbi;
			}
			ListBoxPaths.ItemsSource = items;
		}
		/// <summary>
		/// Sent a signal to thread
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonNextStep_Click(object sender, RoutedEventArgs e)
		{
			signal.Set();
		}
		/// <summary>
		/// Start generating
		/// </summary>
		/// <remarks>
		/// Setting cancel and synchronize parameters.
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonGenerate_Click(object sender, RoutedEventArgs e)
		{
			OnPreGenerating();
			generator = new EllerAlgorithm((int)UpDownHeight.Value, (int)UpDownWidth.Value);
			Progress<string> progress = new Progress<string>((msg) => OnNextStep(msg));
			cancellationToken = new CancellationTokenSource();
			signal = (bool)CheckBoxSteps.IsChecked ? new ManualResetEvent(false) : null;
			Task.Run(() => ExceptionCatcherAsync(generator.Generate(cancellationToken.Token, progress, signal)));
		}
		/// <summary>
		/// Start searching
		/// </summary>
		/// <remarks>
		/// Decorating generator by search
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonSearch_Click(object sender, RoutedEventArgs e)
		{
			OnPreSearch();
			try
			{
				if (!(generator is Searcher))
					generator = new ModifiedDFS(generator);
			}
			catch (ArgumentNullException)
			{
				System.Windows.MessageBox.Show("Об'єкт генератор не існує або не створений",
				"Нульові аргументи", MessageBoxButton.OK, MessageBoxImage.Warning);
				TextBlockStatus.Text = "Помилка";
				OnCrashed();
				return;
			}
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
			Task.Run(() => ExceptionCatcherAsync((generator as Searcher).Search(cancellationToken.Token, progress, signal)));
		}
		/// <summary>
		/// Repaint bitmap
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ChangeVisiblePaths(object sender, RoutedEventArgs e)
		{
			ItemCollection items = ListBoxPaths.Items;
			bool[] paths = new bool[items.Count];
			for (int i = 0; i < paths.Length; ++i)
				paths[i] = (bool)((items[i] as ListBoxItem).Content as CheckBox).IsChecked;
			ConvertToBitmapAndCatchException(paths);
		}
		/// <summary>
		/// Say cancellation token that can exit. Calling UI interaction after that
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			cancellationToken.Cancel();
			OnCancel();
		}
		/// <summary>
		/// Active or not generate button. Depends on values of UpDowns
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void UpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e) =>
			SetIsEnabled(ButtonGenerate, UpDownHeight?.Value != null && UpDownWidth?.Value != null);
		/// <summary>
		/// To change property IsEnabled of control element
		/// </summary>
		/// <param name="control"></param>
		/// <param name="IsEnabled"></param>
		private void SetIsEnabled(Control control, bool IsEnabled)
		{
			if (control != null)
				control.IsEnabled = IsEnabled;
		}
		/// <summary>
		/// To change style of control element
		/// </summary>
		/// <param name="control"></param>
		/// <param name="style"></param>
		private void SetStyle(Control control, Style style)
		{
			if (control != null)
				control.Style = style;
		}
		/// <summary>
		/// Saving maze to extern memory
		/// </summary>
		/// <remarks>
		/// Catching all exceptions from filestream
		/// </remarks>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemBitmap_Click(object sender, RoutedEventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
			dialog.AddExtension = true;
			dialog.DefaultExt = ".bmp";
			dialog.Filter = "Зображення bitmap(*.bmp)|*.bmp";
			dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			if (dialog.ShowDialog() == true)
			{
				try
				{
					converter.SaveToFile(dialog.FileName);
				}
				catch (ArgumentNullException)
				{
					System.Windows.MessageBox.Show("Шлях до файлу не може бути нулем. Спробуйте знову.",
						"Нульовий шлях", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				catch (ArgumentOutOfRangeException)
				{
					System.Windows.MessageBox.Show("Шлях до файлу містить недопустимі символи. Спробуйте інший шлях.",
						"Вихід за межі допустимих символів", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				catch (ArgumentException)
				{
					System.Windows.MessageBox.Show("Неправильний шлях до файлу. Спробуйте знову.",
						"Неправильний шлях", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				catch(NotSupportedException)
				{
					System.Windows.MessageBox.Show("Ця функція не підтримується. Вибачте за незручності.",
						"Непідтримувана функція", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (SecurityException)
				{
					System.Windows.MessageBox.Show("Не дозволено записати цей файл. Виберіть, будь ласка, інший",
						"Помилка безпеки", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (FileNotFoundException)
				{
					System.Windows.MessageBox.Show("Файл за таким шляхом не існує. Спробуйте знову.",
						"Файл не існує", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				catch (DirectoryNotFoundException)
				{
					System.Windows.MessageBox.Show("Тека за цим шляхом не знайдена. Спробуйте знову",
						"Тека не знайдена", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				catch (PathTooLongException)
				{
					System.Windows.MessageBox.Show("Шлях до файлу занадто довгий. Спробуйте зберегти файл в іншому місці",
						"Шлях до файлу", MessageBoxButton.OK, MessageBoxImage.Warning);
				}
				catch (IOException)
				{
					System.Windows.MessageBox.Show("Помилка вводу/виводу. Спробуйте знову",
						"Ввід/вивід", MessageBoxButton.OK, MessageBoxImage.Error);
				}
				catch (InvalidOperationException)
				{
					System.Windows.MessageBox.Show("Використаний невластивий метод для  цього об'єкту.",
						"Невластивий метод", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}
		/// <summary>
		/// Show help to program
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemHelp_Click(object sender, RoutedEventArgs e)
		{
			if (File.Exists(Environment.CurrentDirectory + "\\Help.html"))
				System.Diagnostics.Process.Start("Help.html");
			else
				System.Windows.MessageBox.Show($"Файл за шляхом:\n {Environment.CurrentDirectory}\\Help.html" +
	$" не існує.\nПомістіть файл допомоги в одну директорію з виконавчим файлом.",
							"Допомога не знайдена", MessageBoxButton.OK, MessageBoxImage.Warning);
		}
		/// <summary>
		/// Exit.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MenuItemExit_Click(object sender, RoutedEventArgs e)
		{
			cancellationToken?.Cancel();
			Close();
		}
		/// <summary>
		/// Conver to bitmap
		/// </summary>
		/// <param name="paths"></param>
		private void ConvertToBitmapAndCatchException(bool[] paths = null)
		{
			try
			{
				if (paths == null)
					converter.Convert((dynamic)generator);
				else
					converter.Convert((Searcher)generator, paths);
			}
			catch (ArgumentNullException)
			{
				System.Windows.MessageBox.Show("Об'єкт генератор або пошук не існує або ще не створений",
			"Нульові аргументи", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		/// <summary>
		/// Catching exceptions from generating or search
		/// </summary>
		/// <param name="t">Task where operation executing</param>
		private void ExceptionCatcherAsync(Task t)
		{
			try
			{
				t.Wait();
			}
			catch (AggregateException ae)
			{
				switch (ae.InnerException)
				{
					case ObjectDisposedException ex:
						System.Windows.MessageBox.Show("Неправильно використаний об'єкт скасування процесу." +
						"Спробуйте перезавантажити програму та скасувати знову.",
						"Об'єкт скасування", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
					case NotImplementedException ex:
						System.Windows.MessageBox.Show("Неправильно використовується функція об'єкта",
						"Нереалізована функція", MessageBoxButton.OK, MessageBoxImage.Warning);
						break;
					default:
						System.Windows.MessageBox.Show("Сталась непередбачена ситуація. Будь ласка, перезавантажте програму",
						"Невідома помилка", MessageBoxButton.OK, MessageBoxImage.Error);
						break;
				}
			}
			catch
			{
				System.Windows.MessageBox.Show("Сталась непередбачена ситуація. Будь ласка, перезавантажте програму",
						"Невідома помилка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
