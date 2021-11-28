using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI.ControlMenuOptions.Other
{
	class DataScrubberWindowOption : IControlMenuOption
	{
		private string name = "Data Scrubber Tool";
		public void Execute()
		{
			RecipeEditorWindow.recipeWindow.OpenDatascrubberWindow();
		}

		public string GetName()
		{
			return name;
		}
	}
}
