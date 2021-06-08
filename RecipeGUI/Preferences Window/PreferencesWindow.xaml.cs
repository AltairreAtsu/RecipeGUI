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

namespace RecipeGUI.Preferences_Window
{
	/// <summary>
	/// Interaction logic for Prefrences.xaml
	/// </summary>
	public partial class PreferencesWindow : Window
	{
		public RecipeEditorWindow recipeEditor;
		public PreferencesManager preferencesManager;

		private PreferencesManager originalPrefs;

		private bool savingAndExiting = false;

		public PreferencesWindow()
		{
			InitializeComponent();
		}

		public void Initalize()
		{
			AutoGeneratePatch_Checkbox.IsChecked = preferencesManager.doCreatePatch;
			OverridePatchFile_Checkbox.IsChecked = preferencesManager.doOverridePatchFile;
			UseAutoComplete_Checkbox.IsChecked = preferencesManager.doAutocomplete;

			originalPrefs = new PreferencesManager(preferencesManager.doAutocomplete, preferencesManager.doCreatePatch, preferencesManager.doOverridePatchFile);
		}

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (savingAndExiting)
			{
				recipeEditor.OnPrefrencesWindowClose();
				return;
			}
			bool dataHasChanged =	(originalPrefs.doAutocomplete != (bool)UseAutoComplete_Checkbox.IsChecked)
									|| (originalPrefs.doCreatePatch != (bool)AutoGeneratePatch_Checkbox.IsChecked)
									|| (originalPrefs.doOverridePatchFile != (bool)OverridePatchFile_Checkbox.IsChecked);
			if (dataHasChanged)
			{
				var result = MessageBox.Show("You haven't saved your changes, exit anyway?", "Warning!", MessageBoxButton.YesNo);
				if(result != MessageBoxResult.Yes)
				{
					e.Cancel = true;
				}
			}
			
			recipeEditor.OnPrefrencesWindowClose();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			preferencesManager.WriteConfig();
			savingAndExiting = true;
			Close();
		}

		private void AutoGeneratePatch_Checkbox_Changed(object sender, RoutedEventArgs e)
		{
			preferencesManager.doCreatePatch = (bool)AutoGeneratePatch_Checkbox.IsChecked;
		}

		private void OverridePatchFile_Checkbox_Changed(object sender, RoutedEventArgs e)
		{
			preferencesManager.doOverridePatchFile = (bool)OverridePatchFile_Checkbox.IsChecked;
		}

		private void UseAutoComplete_Checkbox_Changed(object sender, RoutedEventArgs e)
		{
			preferencesManager.doAutocomplete = (bool)UseAutoComplete_Checkbox.IsChecked;
		}
	}
}
