using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace RecipeGUI
{
	static class NumberboxValidator
	{
		private static readonly Regex _regex = new Regex("[^0-9]"); //regex that matches disallowed text
		private static bool IsTextAllowed(string text)
		{
			return !_regex.IsMatch(text);
		}

		public static void PreviewTextInput(TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(e.Text);
		}
		public static void Pasting_Event(DataObjectPastingEventArgs e)
		{
			if (e.DataObject.GetDataPresent(typeof(String)))
			{
				String text = (String)e.DataObject.GetData(typeof(String));
				if (!IsTextAllowed(text))
				{
					e.CancelCommand();
				}
			}
			else
			{
				e.CancelCommand();
			}
		}
	}
}
