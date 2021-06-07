using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.Other
{
	class CurrencyManagerOption : IControlMenuOption
	{
		private string name = "Currency Manager";

		public void Execute()
		{
			RecipeEditorWindow.recipeWindow.OpenCurrencyWindow();
		}

		public string GetName()
		{
			return name;
		}
	}
}
