using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.File
{
	class SaveFileOption : IControlMenuOption
	{
		private string name = "Save";
		public void Execute()
		{
			bool success = RecipeEditorWindow.recipeWindow.SaveRecipe();
			if (!success)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.Filter = "Recipe File|*.recipe";
				saveFileDialog.Title = "Save a Recipe File";
				saveFileDialog.ShowDialog();

				if (saveFileDialog.FileName != "")
				{
					RecipeEditorWindow.recipeWindow.SaveRecipe(saveFileDialog.FileName);
				}
			}
		}

		public string GetName()
		{
			return name;
		}
	}
}
