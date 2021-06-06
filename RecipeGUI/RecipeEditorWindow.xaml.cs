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

namespace RecipeGUI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class RecipeEditorWindow : Window
	{
		private List<InputItemControl> inputItemsControls;
		private List<GroupControl> groupControls;
		private ListLoader listLoader;
		
		private Dictionary<string, int> currencyInputs;
		private CurrencyWindow currencyWindow;

		public RecipeEditorWindow()
		{
			InitializeComponent();
			inputItemsControls = new List<InputItemControl>();
			groupControls = new List<GroupControl>();
			currencyInputs = new Dictionary<string, int>();
			listLoader = new ListLoader();
			listLoader.LoadNamesIntoLists();
			OutputSuggestionField.suggestionStrings = listLoader.nameStrings;
		}

		private void AddInputItem_Click(object sender, RoutedEventArgs e)
		{
			InputItemControl userControl = new InputItemControl();
			userControl.window = this;
			userControl.InputItemSuggestionField.suggestionStrings = listLoader.nameStrings;
			Input_Stack_Panel.Children.Add(userControl);
			Input_Stack_Panel.Height += 50;
			inputItemsControls.Add(userControl);
		}
		private void AddCraftingGroup_Click(object sender, RoutedEventArgs e)
		{
			GroupControl userControl = new GroupControl();
			userControl.window = this;
			userControl.SuggesrionField.suggestionStrings = listLoader.categoryStrings;
			GroupsStackPanel.Children.Add(userControl);
			GroupsStackPanel.Height += 50;
			groupControls.Add(userControl);
		}

		private void LocateFolder_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog browserDialog = new FolderBrowserDialog();
			DialogResult result = browserDialog.ShowDialog();
			if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(browserDialog.SelectedPath)){
				OutputFilePathField.Text = browserDialog.SelectedPath;
			}
			
		}

		private void Generate_Click(object sender, RoutedEventArgs e)
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

			if(currencyInputs.Count != 0)
			{
				recipe.currencyInputs = currencyInputs;
			}

			bool doPatch = (bool)GeneratePatchCheckbox.IsChecked;
			bool sucess = RecipeJsonWriter.WriteJson(OutputFilePathField.Text, recipe, doPatch);
			if (!sucess)
			{
				System.Windows.MessageBox.Show("Error Writing Json file, is the output path correctly formated?");
			}
		}

		private string[] ParseGroups()
		{
			if (groupControls.Count == 0) return null;
			
			List<string> groupStrings = new List<string>();
			foreach(GroupControl control in groupControls)
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
				foreach(InputItemControl control in inputItemsControls)
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


		private void root_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Keyboard.ClearFocus();
		}

		private void OpenCurrencyManager_Click(object sender, RoutedEventArgs e)
		{
			if (currencyWindow != null) return;
			currencyWindow = new CurrencyWindow();
			currencyWindow.mainWindow = this;
			currencyWindow.SetCurrencyStrings(listLoader.currencyStrings);
			if (currencyInputs.Count != 0) currencyWindow.RecoverState(currencyInputs);
			currencyWindow.Show();
		}

		public void setCurrencyDictionary(Dictionary<string, int> currencyInputs)
		{
			this.currencyInputs = currencyInputs;
			currencyWindow = null;
		}
	}
}
