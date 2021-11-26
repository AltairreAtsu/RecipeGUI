using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WinForms = System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace RecipeGUI.Install_Wizard
{
	/// <summary>
	/// Interaction logic for InstallStage1.xaml
	/// </summary>
	public partial class InstallStage2 : UserControl
	{
		public InstallWindow installWindow;
		
		private delegate void UpdateDisplayEvent(string filename, double percentDone);
		private delegate void InstallCompleteEvent();
		
		private string VanillaListsPath = Environment.CurrentDirectory + "\\Lists\\Vanilla Lists";
		private Scrubber scrubber;

		public InstallStage2()
		{
			InitializeComponent();
			scrubber = new Scrubber();
			DataContext = this;
		}

		public void PrepareScrub(string searchDirectory)
		{	
			Thread t = new Thread(() => 
			{
				Thread.CurrentThread.IsBackground = true;
				BeginScrub(searchDirectory);
			}
			);
			t.Start();
		}

		private void BeginScrub(string searchDirectory)
		{
			scrubber.fileScrubEvent += FileStatusEvent;
			scrubber.processCompleteEvent += ScrubbingComplete;
			scrubber.Run(searchDirectory, VanillaListsPath);
		}

		private void UpdateInstallDisplay(string fileName, double percentDone)
		{
			InstallProgressBar.Value = percentDone;
			FileNameDisplay.Text = fileName;
		}

		private void InstallComplete()
		{
			ContinueInstall_Button.IsEnabled = true;
			FileNameDisplay.Text = "Export Complete!";
		}

		#region Thread Event Routing
		private void ScrubbingComplete()
		{
			Dispatcher.BeginInvoke(new InstallCompleteEvent(InstallComplete), new object[] { });
		}
		private void FileStatusEvent(string fileName, double percentDone)
		{
			Dispatcher.BeginInvoke(new UpdateDisplayEvent(UpdateInstallDisplay), new object[] { fileName, percentDone });
		}
		#endregion

		private void ContinueInstall_Button_Click(object sender, RoutedEventArgs e)
		{
			installWindow.BeginStage3();
		}
	}
}
