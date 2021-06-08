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
using System.Windows.Shapes;

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for Prefrences.xaml
	/// </summary>
	public partial class Prefrences : Window
	{
		public RecipeEditorWindow recipeEditor;
		
		public Prefrences()
		{
			InitializeComponent();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			recipeEditor.OnPrefrencesWindowClose();
		}
	}
}
