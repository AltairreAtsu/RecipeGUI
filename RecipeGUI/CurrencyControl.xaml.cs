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
	/// Interaction logic for CurrencyControl.xaml
	/// </summary>
	public partial class CurrencyControl : UserControl
	{
		public RecipeEditorWindow recipeWindow;
		public CurrencyControl()
		{
			InitializeComponent();
		}

		public void SetCurrencyWindow(RecipeEditorWindow recipeWindow)
		{
			this.recipeWindow = recipeWindow;
			recipeWindow.LocationChanged += CurrenyNameSuggestion.OnWindowMove;
		}

		public void ClosePopup()
		{
			CurrenyNameSuggestion.CloseSuggestionBox();
		}

		private void RemoveControlButton_Click(object sender, RoutedEventArgs e)
		{
			recipeWindow.RemoveCurrencyControl(this);
		}

		public CurrencyInput GetCurrencyInput()
		{
			string name = CurrenyNameSuggestion.SuggestionTextField.Text;
			int? amount = null;
			try
			{
				amount = int.Parse(AmountTextField.Text);
			}
			catch
			{
				amount = null;
			}
			return new CurrencyInput(name, amount);
		}
	}
}
