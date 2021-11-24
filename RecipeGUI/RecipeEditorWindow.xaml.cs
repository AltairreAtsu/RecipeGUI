using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RecipeGUI.ControlMenuOptions.File;
using RecipeGUI.ControlMenuOptions.Other;
using RecipeGUI.Preferences_Window;

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class RecipeEditorWindow : Window, IRecipeWindow
	{
		public static IRecipeWindow recipeWindow;
		
		private List<InputItemControl> inputItemsControls;
		private List<GroupControl> groupControls;
		private List<CurrencyControl> currencyControls;
		private ListLoader listLoader;
		
		private Dictionary<string, int> currencyInputs;
		
		private PreferencesWindow preferencesWindow;
		private PreferencesManager preferencesManager;

		private DatascrubberWindow datascrubberWindow;

		private string loadedFilePath;

		public RecipeEditorWindow()
		{
			InitializeComponent();
			recipeWindow = this;

			inputItemsControls = new List<InputItemControl>();
			groupControls = new List<GroupControl>();
			currencyControls = new List<CurrencyControl>();

			currencyInputs = new Dictionary<string, int>();
			listLoader = new ListLoader();
			listLoader.LoadNamesIntoLists();

			SetupPrefrences();
			SetupFileMenu();
			SetupOtherMenu();

			OutputSuggestionField.suggestionStrings = listLoader.nameStrings;
			OutputSuggestionField.prefs = preferencesManager;
		}

		private void SetupPrefrences()
		{
			preferencesManager = new PreferencesManager();
			try
			{
				preferencesManager.ReadConfig();
			}
			catch
			{
				try
				{
					preferencesManager.WriteConfig();
				}
				catch
				{
					System.Windows.MessageBox.Show("Error! Could not load or create config!");
				}
			}
		}

		private void SetupFileMenu()
		{
			var list = new List<IControlMenuOption>();
			list.Add(new NewFileOption());
			list.Add(new OpenFileOption());
			list.Add(new SaveFileOption());
			list.Add(new SaveAsFileOption());
			File_MenuItem.InitializeOptions(list);
		}
		private void SetupOtherMenu()
		{
			var list = new List<IControlMenuOption>();
			list.Add(new DataScrubberWindowOption());
			list.Add(new PreferencesWindowOption());
			Other_MenuItem.InitializeOptions(list);
		}
	

		#region Input and Group Controls
		private void AddInputItem_Click(object sender, RoutedEventArgs e)
		{
			AddInputItem("", 0);
		}

		private void AddCraftingGroup_Click(object sender, RoutedEventArgs e)
		{
			AddCraftingGroup("");
		}

		public void AddInputItem(string name, int count)
		{
			InputItemControl userControl = new InputItemControl();
			userControl.SetRecipeEditorWindow(this);
			userControl.InputItemSuggestionField.suggestionStrings = listLoader.nameStrings;
			userControl.InputItemSuggestionField.prefs = preferencesManager;
			userControl.InputItemSuggestionField.SuggestionTextField.Text = name;
			userControl.CountField.Text = count.ToString();

			Input_Stack_Panel.Children.Add(userControl);
			Input_Stack_Panel.Height += 50;
			inputItemsControls.Add(userControl);
		}

		public void AddCraftingGroup(string name)
		{
			GroupControl userControl = new GroupControl();
			userControl.SetRecipeEditorWindow(this);
			userControl.SuggesrionField.suggestionStrings = listLoader.categoryStrings;
			userControl.SuggesrionField.prefs = preferencesManager;
			userControl.SuggesrionField.SuggestionTextField.Text = name;

			GroupsStackPanel.Children.Add(userControl);
			GroupsStackPanel.Height += 50;
			groupControls.Add(userControl);
		}

		public void RemoveInputElement(UIElement element)
		{
			Input_Stack_Panel.Children.Remove(element);
			Input_Stack_Panel.Height -= 50;
			inputItemsControls.Remove((InputItemControl)element);
		}

		public void RemoveGroupElement(UIElement element)
		{
			GroupsStackPanel.Children.Remove(element);
			GroupsStackPanel.Height -= 50;
			groupControls.Remove((GroupControl)element);
		}

		private void RemoveAllInputElements()
		{
			inputItemsControls = new List<InputItemControl>();
			Input_Stack_Panel.Children.Clear();
		}

		private void RemoveAllGroupElements()
		{
			groupControls = new List<GroupControl>();
			GroupsStackPanel.Children.Clear();
		}

		public void GroupScrollChanged(object sender, ScrollChangedEventArgs args)
		{
			if(ReferenceEquals(args.Source, GroupScroll)
				&& args.VerticalChange != 0)
			{
				foreach (GroupControl control in groupControls)
				{
					control.ClosePopup();
				}
			}
		}

		public void InputScrollChanged(object sender, ScrollChangedEventArgs args)
		{
			if (ReferenceEquals(args.Source, InputScroll)
				&& args.VerticalChange != 0)
			{
				foreach (InputItemControl control in inputItemsControls)
				{
					control.ClosePopup();
				}
			}
		}
		#endregion

		#region Currency Manager
		private void AddCurrencyButton_Click(object sender, RoutedEventArgs e)
		{
			AddCurrencyControl();
		}

		private void CurrencyScrollChanged(object sender, ScrollChangedEventArgs e)
		{
			if (ReferenceEquals(e.Source, CurrencyScroll)
				&& e.VerticalChange != 0)
			{
				foreach (CurrencyControl control in currencyControls)
				{
					control.ClosePopup();
				}
			}

		}

		private void Expander_Expanded(object sender, RoutedEventArgs e)
		{
			CurrencyExpander.Height = 160;
			Height += (CurrencyExpander.Height - 40);
			// Expand the window
		}

		private void Expander_Collapsed(object sender, RoutedEventArgs e)
		{
			// Collapse the window
			Height -= (CurrencyExpander.Height - 40);
			CurrencyExpander.Height = 40;
		}

		private void RecoverState(Dictionary<string, int> currencyInputs)
		{
			foreach (KeyValuePair<string, int> kvp in currencyInputs)
			{
				CurrencyControl control = AddCurrencyControl();
				control.CurrenyNameSuggestion.SuggestionTextField.Text = kvp.Key;
				control.AmountTextField.Text = kvp.Value.ToString();
			}
		}

		private CurrencyControl AddCurrencyControl()
		{
			CurrencyControl currencyControl = new CurrencyControl();
			CurrencyControlsStackPanel.Children.Add(currencyControl);
			currencyControls.Add(currencyControl);
			currencyControl.SetCurrencyWindow(this);
			currencyControl.CurrenyNameSuggestion.suggestionStrings = listLoader.currencyStrings;
			currencyControl.CurrenyNameSuggestion.prefs = preferencesManager;
			currencyControl.Width = CurrencyControlsStackPanel.Width;
			CurrencyControlsStackPanel.Height += currencyControl.Height;
			return currencyControl;
		}
		public void RemoveCurrencyControl(CurrencyControl currencyControl)
		{
			CurrencyControlsStackPanel.Children.Remove(currencyControl);
			currencyControls.Remove(currencyControl);
			CurrencyControlsStackPanel.Height -= currencyControl.Height;
		}
		private void RemoveAllCurrencyElements()
		{
			CurrencyControlsStackPanel.Children.Clear();
			currencyControls = new List<CurrencyControl>();
		}

		#endregion

		#region Parsing Values
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
					System.Windows.MessageBox.Show("Parsing Error! Amount of " + input.name + "not set to numerical value! Canceling Export.");
					parsingResult.parsingFailed = true;
					return parsingResult;
				}
				if (currencyInputs.ContainsKey(input.name))
				{
					System.Windows.MessageBox.Show("Duplicate Currency name detected. Please ensure only one input exists for key " + input.name + ".");
					parsingResult.parsingFailed = true;
					return parsingResult;
				}
				currencyInputs.Add(input.name, (int)input.amount);
			}
			parsingResult.parsingFailed = false;
			parsingResult.currencyInputs = currencyInputs;
			return parsingResult;
		}

		private string[] ParseGroups()
		{
			if (groupControls.Count == 0) return null;

			List<string> groupStrings = new List<string>();
			foreach (GroupControl control in groupControls)
			{
				groupStrings.Add(control.SuggesrionField.SuggestionTextField.Text);

			}
			return groupStrings.ToArray();
		}

		private RecipeItem[] TryParseInputItems()
		{
			if (inputItemsControls.Count == 0) return null;

			try
			{
				List<RecipeItem> items = new List<RecipeItem>();
				foreach (InputItemControl control in inputItemsControls)
				{
					items.Add(new RecipeItem(
						control.InputItemSuggestionField.SuggestionTextField.Text,
						int.Parse(control.CountField.Text)));
				}
				return items.ToArray();
			}
			catch
			{
				System.Windows.MessageBox.Show("Error Parsing Input Items. Please ensure only numeric values are used in count fields.");
				return null;
			}
		}

		private RecipeItem TryParseOutput()
		{
			try
			{
				string name = OutputSuggestionField.SuggestionTextField.Text;
				int count = int.Parse(OutputItemCountField.Text);
				RecipeItem output = new RecipeItem(name, count);
				return output;
			}
			catch
			{
				System.Windows.MessageBox.Show("Error Parsing Output Item! Please ensure only numeric values are used in the count field.");
				return null;
			}
		}
		#endregion

		// Remove Focus to close suggestion boxes
		private void root_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
		}

		public void OnPrefrencesWindowClose()
		{
			preferencesWindow = null;
		}

		public void OnDatascrubberWindowClose()
		{
			datascrubberWindow = null;
		}

		public void setCurrencyDictionary(Dictionary<string, int> currencyInputs)
		{
			this.currencyInputs = currencyInputs;
		}

		#region Interface Methods
		public void ResetWindow()
		{
			RemoveAllInputElements();
			RemoveAllGroupElements();
			RemoveAllCurrencyElements();
			OutputSuggestionField.SuggestionTextField.Text = "";
			OutputItemCountField.Text = "";

			currencyInputs = new Dictionary<string, int>();

			loadedFilePath = null;
		}

		public void OpenPrefrencesWindow()
		{
			if (preferencesWindow != null) return;

			preferencesWindow = new PreferencesWindow();
			preferencesWindow.preferencesManager = preferencesManager;
			preferencesWindow.recipeEditor = this;
			preferencesWindow.Initalize();
			preferencesWindow.Show();
		}

		public void OpenDatascrubberWindow()
		{
			if (datascrubberWindow != null) return;

			datascrubberWindow = new DatascrubberWindow();
			datascrubberWindow.recipeEditorWindow = this;
			datascrubberWindow.Show();
		}

		public bool SaveRecipe()
		{
			if(loadedFilePath != null)
			{
				SaveRecipe(loadedFilePath);
				return true;
			}
			return false;
		}

		public void SaveRecipe(string path)
		{
			Recipe recipe = new Recipe();

			RecipeItem output = TryParseOutput();
			if (output == null) return;
			recipe.output = output;

			RecipeItem[] input = TryParseInputItems();
			if (output == null) return;
			recipe.input = input;

			string[] groups = ParseGroups();
			if (groups == null) return;
			recipe.groups = groups;

			var parsingResult = ParseCurrencies();
			if (parsingResult.parsingFailed)
			{
				return;
			}
			else
			{
				recipe.currencyInputs = parsingResult.currencyInputs;
			}


			bool sucess = RecipeJsonHandler.WriteJson(path, recipe, preferencesManager.doCreatePatch, preferencesManager.doOverridePatchFile);
			if (!sucess)
			{
				System.Windows.MessageBox.Show("Error Writing Json file, is the output path correctly formated?");
			}
		}

		public void LoadRecipe(Recipe recipe, string path)
		{
			RemoveAllInputElements();
			RemoveAllGroupElements();
			RemoveAllCurrencyElements();

			loadedFilePath = path;

			foreach(RecipeItem item in recipe.input)
			{
				AddInputItem(item.item, item.count);
			}
			foreach(string group in recipe.groups)
			{
				AddCraftingGroup(group);
			}

			OutputSuggestionField.SuggestionTextField.Text = recipe.output.item;
			OutputItemCountField.Text = recipe.output.count.ToString();

			currencyInputs = recipe.currencyInputs;
			if (currencyInputs == null) currencyInputs = new Dictionary<string, int>();
			RecoverState(currencyInputs);
		}
		#endregion
		public ListLoader GetListLoader()
		{
			return listLoader;
		}
	}
}

public class ParsingResult
{
	public Dictionary<string, int> currencyInputs;
	public bool parsingFailed = false;
}