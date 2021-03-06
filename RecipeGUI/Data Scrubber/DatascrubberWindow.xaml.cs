using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class DatascrubberWindow : Window
	{
		private Scrubber scrubber;
		public RecipeEditorWindow recipeEditorWindow;

		public DatascrubberWindow()
		{
			InitializeComponent();
			scrubber = new Scrubber();
		}

		private void LocateModClick(object sender, RoutedEventArgs e)
		{
			var folderBrowser = new WinForms.FolderBrowserDialog();
			var result = folderBrowser.ShowDialog();
			if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
			{
				ModDirectoryField.Text = folderBrowser.SelectedPath;
			}
		}

		private void LocateOutputClick(object sender, RoutedEventArgs e)
		{
			var folderBrowser = new WinForms.FolderBrowserDialog();
			var result = folderBrowser.ShowDialog();
			if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
			{
				OutputFolderField.Text = folderBrowser.SelectedPath;
			}
		}

		private void BeginScrubbingClick(object sender, RoutedEventArgs e)
		{
			ScrubStartButton.IsEnabled = false;

			string rootPath = ModDirectoryField.Text;
			string outputPath = OutputFolderField.Text;

			if (!Directory.Exists(rootPath))
			{
				MessageBox.Show(Lang.modDoesNotExist);
				return;
			}
			if (!Directory.Exists(outputPath))
			{
				MessageBox.Show(Lang.outputDoesNotExist);
				return;
			}
			// Warn the user if the metadata file is not found
			if (!DirectoryContainsMetadataFile(rootPath))
			{
				WinForms.DialogResult dialogResult = WinForms.MessageBox.Show(Lang.metadataFileNotFoundBody, Lang.metadataFileNotFoundTitle, WinForms.MessageBoxButtons.YesNo);
				if(dialogResult == WinForms.DialogResult.No)
				{
					ScrubStartButton.IsEnabled = true;
					return;
				}
			}

			scrubber.statusUpdateEvent += ScrubStatusEvent;
			scrubber.fileQuantityWarningEvent += FileQuantityWarning;
			scrubber.RunAndLoad(rootPath, outputPath, recipeEditorWindow.GetListLoader());
			ScrubStartButton.IsEnabled = true;
		}

		public bool DirectoryContainsMetadataFile(string path)
		{
			if (Directory.Exists(path))
			{
				string[] files = Directory.GetFiles(path);
				foreach (string file in files)
				{
					string fileName = System.IO.Path.GetFileName(file);
					bool fileNameContainsMeta = fileName.IndexOf("metadata", StringComparison.OrdinalIgnoreCase) >= 0;
					if (fileNameContainsMeta) return true;

				}
			}
			return false;
		}

		private bool FileQuantityWarning()
		{
			var result = MessageBox.Show(Lang.fileWarningContent, Lang.fileWarningTitle, MessageBoxButton.YesNo);
			return result == MessageBoxResult.No;
		}

		private void ScrubStatusEvent(string eventMessage)
		{
			ScrubbingStatusTextblock.Text = eventMessage;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			recipeEditorWindow.OnDatascrubberWindowClose();
		}
	}
}
