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
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class InputItemControl : UserControl
	{
		private RecipeEditorWindow window;
	
		public InputItemControl()
		{
			InitializeComponent();
		}

		public void SetRecipeEditorWindow(RecipeEditorWindow window)
		{
			this.window = window;
			window.LocationChanged += InputItemSuggestionField.OnWindowMove;
		}

		public void ClosePopup()
		{
			InputItemSuggestionField.CloseSuggestionBox();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			// Remove this GUI Item
			window.RemoveInputElement(this);
		}

		private void CountField_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			NumberboxValidator.PreviewTextInput(e);
		}

		private void CountField_Pasting(object sender, DataObjectPastingEventArgs e)
		{
			NumberboxValidator.Pasting_Event(e);
		}
	}
}
