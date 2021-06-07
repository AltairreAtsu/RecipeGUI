using System;
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
			throw new NotImplementedException();
		}

		public string GetName()
		{
			return name;
		}
	}
}
