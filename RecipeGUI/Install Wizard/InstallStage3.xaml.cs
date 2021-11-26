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
	public partial class InstallStage3 : UserControl
	{
		public InstallWindow installWindow;

		public InstallStage3()
		{
			InitializeComponent();
		}
		
		private void FinishInstall_Click(Object sender, RoutedEventArgs e)
		{
			installWindow.OpenMainApp();
		}

	}
}
