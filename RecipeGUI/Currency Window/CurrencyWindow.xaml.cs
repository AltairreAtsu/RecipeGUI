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
using RecipeGUI.Preferences_Window;

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for CurrencyWindow.xaml
	/// </summary>
	/// 
	public partial class CurrencyWindow : Window
	{
		private List<CurrencyControl> currencyControls = new List<CurrencyControl>();
		private List<string> currencySuggestionStrings;
		private Dictionary<string, int> originalOutputs;
		private bool SavingAndClosing = false;
		private PreferencesManager prefs;

		public RecipeEditorWindow mainWindow;

		public CurrencyWindow()
		{
			InitializeComponent();
		}

		public void RecoverState(Dictionary<string, int> currencyInputs)
		{
			originalOutputs = currencyInputs;
			foreach(KeyValuePair<string, int> kvp in currencyInputs)
			{
				CurrencyControl control = AddCurrencyControl();
				control.CurrenyNameSuggestion.SuggestionTextField.Text = kvp.Key;
				control.AmountTextField.Text = kvp.Value.ToString();
			}
		}

		public void SetCurrencyStrings(List<string> currencyStrings)
		{
			currencySuggestionStrings = currencyStrings;
		}
		public void SetPreferences(PreferencesManager prefs)
		{
			this.prefs = prefs;
		}

		public CurrencyControl AddCurrencyControl()
		{
			CurrencyControl currencyControl = new CurrencyControl();
			CurrencyControlsStackPanel.Children.Add(currencyControl);
			currencyControls.Add(currencyControl);
			currencyControl.currencyWindow = this;
			currencyControl.CurrenyNameSuggestion.suggestionStrings = currencySuggestionStrings;
			currencyControl.CurrenyNameSuggestion.prefs = prefs;
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

		private ParsingResult ParseCurrencies()
		{
			var currencyInputs = new Dictionary<string, int>();
			var parsingResult = new ParsingResult();


			if (currencyControls.Count == 0)
			{
				parsingResult.parsingFailed = false;
				return parsingResult;
			}
			foreach (CurrencyControl control in currencyControls)
			{
				var input = control.GetCurrencyInput();
				if (input.amount == null)
				{
					MessageBox.Show("Parsing Error! Amount of " + input.name + "not set to numerical value! Canceling Export.");
					parsingResult.parsingFailed = true;
					return parsingResult;
				}
				if (currencyInputs.ContainsKey(input.name))
				{
					MessageBox.Show("Duplicate Currency name detected. Please ensure only one input exists for key " + input.name + ".");
					parsingResult.parsingFailed = true;
					return parsingResult;
				}
				currencyInputs.Add(input.name, (int)input.amount);
			}
			parsingResult.parsingFailed = false;
			parsingResult.currencyInputs = currencyInputs;
			return parsingResult;
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			mainWindow.OnCurrencyWindowClose();
			if (SavingAndClosing)
			{
				return;
			}
			var parsingResult = ParseCurrencies();

			if (parsingResult.parsingFailed)
			{
				var result = MessageBox.Show("Parsing data failed and has not been saved. Do you want to exit the window?", "Warning!", MessageBoxButton.YesNo);
				if (result == MessageBoxResult.Yes)
				{
					return;
				}
				else
				{
					e.Cancel = true;
				}
			}

			bool dataHasChanged =	(originalOutputs == null && parsingResult.currencyInputs != null) ||
									(originalOutputs != null && parsingResult.currencyInputs == null) ||
									(originalOutputs != null && !DictionariesAreEqual(originalOutputs, parsingResult.currencyInputs));
			if (dataHasChanged)
			{
				var result = MessageBox.Show("You have unsaved changes, are you sure you want to close the window?", "Warning!", MessageBoxButton.YesNo);
				if(result == MessageBoxResult.Yes)
				{
					return;
				}
				else
				{
					e.Cancel = true;
				}
			}
		}

		private void Done_Click(object sender, RoutedEventArgs e)
		{
			var parsingResult = ParseCurrencies();
			if (parsingResult.currencyInputs == null)
			{
				return;
			}
			mainWindow.setCurrencyDictionary(parsingResult.currencyInputs);
			SavingAndClosing = true;
			Close();
		}

		private bool DictionariesAreEqual(Dictionary<string, int> dic1, Dictionary<string, int> dic2)
		{
			return dic1.Count == dic2.Count && !dic1.Except(dic2).Any();
		}
	}

	public class ParsingResult
	{
		public Dictionary<string, int> currencyInputs;
		public bool parsingFailed = false;
	}
}
