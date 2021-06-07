using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI
{
	public class Recipe
	{
		public Dictionary<String, int> currencyInputs { get; set; }
		public RecipeItem[] input { get; set; }
		public RecipeItem output { get; set; }
		public string[] groups { get; set; }
	}
	public class RecipeItem
	{
		public string item { get; set; }
		public int count { get; set; }

		public RecipeItem(string item, int count)
		{
			this.item = item;
			this.count = count;
		}
	}
}
