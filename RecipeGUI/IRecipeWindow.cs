using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI
{
	public interface IRecipeWindow
	{
		bool SaveRecipe();
		void SaveRecipe(string path);
		void ResetWindow();
		void LoadRecipe(Recipe recipe, string path);
		void OpenCurrencyWindow();
	}
}
