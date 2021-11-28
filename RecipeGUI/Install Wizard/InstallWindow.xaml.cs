using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;

namespace RecipeGUI.Install_Wizard
{
	/// <summary>
	/// Interaction logic for InstallWindow.xaml
	/// </summary>
	public partial class InstallWindow : Window
	{
		private InstallStage1 installStage1;
		private InstallStage2 installStage2;
		private InstallStage3 installStage3;

		public InstallWindow()
		{
			InitializeComponent();

			installStage1 = new InstallStage1();
			installStage1.installWindow = this;
			installStage2 = new InstallStage2();
			installStage2.installWindow = this;
			installStage3 = new InstallStage3();
			installStage3.installWindow = this;

			Content = installStage1;
		}

		public void BeginStage2Async(string searchDirectory)
		{
			if (!Directory.Exists(searchDirectory))
			{
				MessageBox.Show("Warning! The application could not locate the provided directory, please navigate to your unpacked assets folder.");
				return;
			}

			if (!IsPackedAssetsDirectory(searchDirectory))
			{
				MessageBox.Show("Warning! The selected file does not contain Starbound's _metadata file. Please navigate to your unpacked assets folder.");
				return;
			}

			SwitchDisplay(installStage2);
			installStage2.PrepareScrub(searchDirectory);
		}

		public void BeginStage3()
		{
			SwitchDisplay(installStage3);
		}

		public void OpenMainApp()
		{
			RecipeEditorWindow editorWindow = new RecipeEditorWindow();
			editorWindow.Show();
			Close();
		}

		private void SwitchDisplay(UIElement element)
		{
			Content = element;
		}

		private bool IsPackedAssetsDirectory(string directory)
		{
			string filePath = directory + "\\_metadata";
			if (!File.Exists(filePath)) return false;

			JObject fileJson = JObject.Parse(File.ReadAllText(filePath));
			string author = (string)fileJson["author"];
			string name = (string)fileJson["name"];

			if (author.Equals("Chucklefish") && name.Equals("base")) return true;

			return false;
		}
	}
}
