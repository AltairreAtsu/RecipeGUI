using System;
using RecipeGUI;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.File
{
	class NewFileOption : IControlMenuOption
	{
		private string name = "New";
		
		public void Execute()
		{
			RecipeEditorWindow.recipeWindow.ResetWindow();
		}

		public string GetName()
		{
			return name;
		}
	}
}
