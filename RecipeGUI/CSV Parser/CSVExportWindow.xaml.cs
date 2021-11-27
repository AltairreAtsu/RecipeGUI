using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WinForms = System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace RecipeGUI.CSV_Parser
{
	/// <summary>
	/// Interaction logic for CSVExportWindow.xaml
	/// </summary>
	public partial class CSVExportWindow : Window
	{
		private CSVParser parser;
		public RecipeEditorWindow recipeEditorWindow;

		public CSVExportWindow()
		{
			InitializeComponent();
			parser = new CSVParser();
		}

		private void BeginExport_Click(object sender, RoutedEventArgs e)
		{
			if (!File.Exists(CSVFilePath_Field.Text))
			{
				MessageBox.Show("Target CSV could not be located! Please ensure a valid path is provided.");
				return;
			}
			try
			{
				ExportStartButton.IsEnabled = false;
				parser.Run(CSVFilePath_Field.Text, OutputFolder_Field.Text, true);
				ExportStartButton.IsEnabled = true;
				MessageBox.Show("Export Complete!");
			}
			catch(Exception E)
			{
				MessageBox.Show(E.Message);
			}
			
		}

		private void LocateFile_Click(object sender, RoutedEventArgs e)
		{
			WinForms.OpenFileDialog fileDialog = new WinForms.OpenFileDialog();
			fileDialog.Filter = "CSV File (*.csv)|*.csv";
			fileDialog.Title = "Select a CSV File to open";

			if (fileDialog.ShowDialog() == WinForms.DialogResult.OK)
			{
				CSVFilePath_Field.Text = fileDialog.FileName;
			}
		}

		private void LocateOutput_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowser = new WinForms.FolderBrowserDialog();
			var result = folderBrowser.ShowDialog();
			if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
			{
				OutputFolder_Field.Text = folderBrowser.SelectedPath;
			}
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			recipeEditorWindow.OnCSVExportWindowClose();
		}
	}
}
