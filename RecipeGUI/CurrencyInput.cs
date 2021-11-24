using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI
{
	public class CurrencyInput
	{
		public string name;
		public int? amount;

		public CurrencyInput(string name, int? amount)
		{
			this.name = name;
			this.amount = amount;
		}
	}
}
