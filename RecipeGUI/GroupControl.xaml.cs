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

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for UserControl2.xaml
	/// </summary>
	public partial class GroupControl : UserControl
	{
		private RecipeEditorWindow window;
		public GroupControl()
		{
			InitializeComponent();
		}

		public void SetRecipeEditorWindow(RecipeEditorWindow window)
		{
			this.window = window;
			window.LocationChanged += SuggesrionField.OnWindowMove;
		}

		public void ClosePopup()
		{
			SuggesrionField.CloseSuggestionBox();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			window.RemoveGroupElement(this);
		}
	}
}
