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
using WinForms = System.Windows.Forms;

namespace RecipeGUI.Install_Wizard
{
	/// <summary>
	/// Interaction logic for InstallStage1.xaml
	/// </summary>
	public partial class InstallStage1 : UserControl
	{
		public InstallWindow installWindow;

		public InstallStage1()
		{
			InitializeComponent();
		}

		private void OpenDirectorySearch_Button_Click(object sender, RoutedEventArgs e)
		{
			var folderBrowser = new WinForms.FolderBrowserDialog();
			var result = folderBrowser.ShowDialog();
			if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowser.SelectedPath))
			{
				UnpackedDirectory_Field.Text = folderBrowser.SelectedPath;
			}
		}

		private void ContinueInstall_Button_Click(object sender, RoutedEventArgs e)
		{
			installWindow.BeginStage2Async(UnpackedDirectory_Field.Text);
		}
	}
}
