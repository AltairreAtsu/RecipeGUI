using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.File
{
	class SaveAsFileOption : IControlMenuOption
	{
		private string name = "Save As";
		public void Execute()
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Recipe File|*.recipe";
			saveFileDialog.Title = "Save a Recipe File";
			saveFileDialog.ShowDialog();

			if(saveFileDialog.FileName != "")
			{
				RecipeEditorWindow.recipeWindow.SaveRecipe(saveFileDialog.FileName);
			}
		}

		public string GetName()
		{
			return name;
		}
	}
}
