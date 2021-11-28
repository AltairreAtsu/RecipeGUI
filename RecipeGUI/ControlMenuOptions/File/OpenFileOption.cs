using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.File
{
	class OpenFileOption : IControlMenuOption
	{
		private string name = "Open";

		public void Execute()
		{
			OpenFileDialog fileDialog = new OpenFileDialog();
			fileDialog.Filter = "Recipe File (*.recipe)|*.recipe";
			fileDialog.Title = "Select a Recipe File to open";
			
			if(fileDialog.ShowDialog() == DialogResult.OK)
			{
				string filePath = fileDialog.FileName;
				Recipe recipe = RecipeJsonHandler.ReadJson(filePath);
				if(recipe == null)
				{
					MessageBox.Show("Error Loading the specified  file, please try again.");
					return;
				}
				RecipeEditorWindow.recipeWindow.LoadRecipe(recipe, filePath);
			}
		}

		public string GetName()
		{
			return name;
		}
	}
}
