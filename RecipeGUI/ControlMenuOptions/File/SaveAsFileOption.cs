using System;
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
			throw new NotImplementedException();
		}

		public string GetName()
		{
			return name;
		}
	}
}
