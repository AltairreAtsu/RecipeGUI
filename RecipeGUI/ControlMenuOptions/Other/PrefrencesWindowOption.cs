using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.Other
{
	class PrefrencesWindowOption : IControlMenuOption
	{
		private string name = "Prefrences";

		public void Execute()
		{
			RecipeEditorWindow.recipeWindow.OpenPrefrencesWindow();
		}

		public string GetName()
		{
			return name;
		}
	}
}
