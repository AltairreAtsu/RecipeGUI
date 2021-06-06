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
	/// Interaction logic for CurrencyWindow.xaml
	/// </summary>
	/// 
	public partial class CurrencyWindow : Window
	{
		private List<CurrencyControl> currencyControls = new List<CurrencyControl>();
		private List<string> currencyStrings;
		public RecipeEditorWindow mainWindow;

		public CurrencyWindow()
		{
			InitializeComponent();
		}

		public void RecoverState(Dictionary<string, int> currencyInputs)
		{
			foreach(KeyValuePair<string, int> kvp in currencyInputs)
			{
				CurrencyControl control = AddCurrencyControl();
				control.CurrenyNameSuggestion.SuggestionTextField.Text = kvp.Key;
				control.AmountTextField.Text = kvp.Value.ToString();
			}
		}

		public void SetCurrencyStrings(List<string> currencyStrings)
		{
			this.currencyStrings = currencyStrings;
		}

		public CurrencyControl AddCurrencyControl()
		{
			CurrencyControl currencyControl = new CurrencyControl();
			CurrencyControlsStackPanel.Children.Add(currencyControl);
			currencyControls.Add(currencyControl);
			currencyControl.currencyWindow = this;
			currencyControl.CurrenyNameSuggestion.suggestionStrings = currencyStrings;
			CurrencyControlsStackPanel.Height += currencyControl.Height;
			return currencyControl;
		}


		public void RemoveCurrencyControl(CurrencyControl currencyControl)
		{
			CurrencyControlsStackPanel.Children.Remove(currencyControl);
			currencyControls.Remove(currencyControl);
			CurrencyControlsStackPanel.Height -= currencyControl.Height;
		}

		private void AddCurrencyButton_Click(object sender, RoutedEventArgs e)
		{
			AddCurrencyControl();
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			Dictionary<string, int> currencyInputs = new Dictionary<string, int>();
			if (currencyControls.Count == 0) return;
			foreach(CurrencyControl control in currencyControls)
			{
				var input = control.GetCurrencyInput();
				if(input.amount == null)
				{
					MessageBox.Show("Parsing Error! Amount of " + input.name + "not set to numerical value! Canceling Export.");
					e.Cancel = true;
					return;
				}
				if (currencyInputs.ContainsKey(input.name))
				{
					MessageBox.Show("Duplicate Currency name detected. Please ensure only one input exists for key " + input.name + ".");
					e.Cancel = true;
					return;
				}
				currencyInputs.Add(input.name, (int)input.amount);
			}
			mainWindow.setCurrencyDictionary(currencyInputs);
		}
	}
}
