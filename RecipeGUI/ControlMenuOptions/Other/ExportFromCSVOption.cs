using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.Other
{
	class ExportFromCSVOption : IControlMenuOption
	{
		private string name = "Export recipes from CSV";
		
		public void Execute()
		{
			RecipeEditorWindow.recipeWindow.OpenCSVExporter();
		}

		public string GetName()
		{
			return name;
		}
	}
}
