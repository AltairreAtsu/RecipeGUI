using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RecipeGUI.CSV_Parser
{
	class CSVParser
	{
		private const string INPUT_KEY = "Input Item";
		private const string OUTPUT_KEY = "Output Item";
		private const string CURRENCY_KEY = "Currency";
		private const string GROUP_KEY = "Group";
		private const string QUANTITY_KEY = "Quantity";
		private const string FILENAME_KEY = "File Name";

		private Dictionary<int, string> topRowValues;
		private List<string> fileNames;

		public void Run(string filePath, string outputPath, bool doPatch)
		{
			try
			{
				fileNames = new List<string>();
				List<NamedRecipe> recipes = BeginParse(filePath);
				WriteRecipes(recipes, outputPath, doPatch);
			}
			catch (Exception E)
			{
				throw E;
			}

		}

		#region ParsingCSV
		private List<NamedRecipe> BeginParse(string filePath)
		{
			try
			{
				List<NamedRecipe> recipes = new List<NamedRecipe>();
				string file = File.ReadAllText(filePath);
				string[] lines = file.Split(new[] { Environment.NewLine },
									 StringSplitOptions.RemoveEmptyEntries);

				ValidateTopRow(SplitLine(lines[0]));

				for (int i = 1; i < lines.Count(); i++)
				{
					recipes.Add(ReadRow(SplitLine(lines[i]), i));
				}

				if (recipes.Count == 0) throw new Exception("No recipes were found on the provided CSV Sheet.");
				return recipes;
			}
			catch (Exception E)
			{
				throw E;
			}
		}

		private NamedRecipe ReadRow(string[] values, int row)
		{
			Recipe r = new Recipe();
			List<string> groups = new List<string>();
			List<RecipeItem> input = new List<RecipeItem>();
			Dictionary<string, int> currencyInputs = new Dictionary<string, int>();
			RecipeItem output = null;
			string name = "";

			for (int i = 0; i < values.Length; i++)
			{
				string value = values[i];
				if (value.Equals(""))
				{
					continue;
				}

				switch (topRowValues[i])
				{
					case FILENAME_KEY:

						if (fileNames.Contains(value)) throw new Exception("Error at cell row: " + row + " column: " + i + ", every recipe must have a unique file name! Please remove or change duplicate occurrences of: " + value);
						name = value;
						fileNames.Add(name);
						break;

					case GROUP_KEY:
						if (groups.Contains(value)) throw new Exception("Error at cell row: " + row + " column: " + i + ", a single recipe cannot contain a duplicate group key.  Please remove all duplicate occurrences of: " + value);
						groups.Add(value);
						break;

					case OUTPUT_KEY:
						try
						{
							int quantity = int.Parse(values[i + 1]);
							output = new RecipeItem(value, quantity);
							i++;
						}
						catch (FormatException)
						{
							throw new Exception("Error at cell row: " + row + " column: " + i + ", failed to parse quantity. Please ensure only numerical values are used.");
						}
						catch (ArgumentNullException)
						{
							throw new Exception("Error at cell row: " + row + " column: " + i + ", quantity cell cannot be empty or null. Please provide a numerical value.");
						}
						catch (Exception E)
						{
							throw E;
						}
						break;

					case INPUT_KEY:
						try
						{
							if (InputContainsKey(input, value)) throw new Exception("Error at cell row: " + row + " column: " + i + ", a single recipe cannot contain a duplicate input item name. Please remove all duplicate names.");
							int quantity = int.Parse(values[i + 1]);
							input.Add(new RecipeItem(value, quantity));
							i++;
						}
						catch(FormatException)
						{
							throw new Exception("Error at cell row: " + row + " column: " + i + ", failed to parse quantity cell. Please ensure only numerical values are used.");
						}
						catch(ArgumentNullException)
						{
							throw new Exception("Error at cell row: " + row + " column: " + i + ", quantity cell cannot be empty or null. Please provide a numerical value.");
						}
						catch (Exception E)
						{
							throw E;
						}
						break;

					case CURRENCY_KEY:
						try
						{
							if (currencyInputs.ContainsKey(value)) throw new Exception("Error at cell row:" + row + " column:" + i + " a single recipe cannot contain a duplicate currency key. Please remove all duplicate keys.");
							int quantity = int.Parse(values[i + 1]);
							currencyInputs.Add(value, quantity);
							i++;
						}
						catch (FormatException)
						{
							throw new Exception("Error at cell row: " + row + " column: " + i + ", failed to parse quantity cell. Please ensure only numerical values are used.");
						}
						catch (ArgumentNullException)
						{
							throw new Exception("Error at cell row: " + row + " column: " + i + ", quantity cell cannot be empty or null. Please provide a numerical value.");
						}
						catch (Exception E)
						{
							throw E;
						}
						break;

					default:
						throw new Exception("Error at cell row: " + row + " column: " + i + ", unrecognized header key. Please ensure the top row uses only protected keys.");
				}
			}

			if (name.Equals("")) throw new Exception("Error at row: " + row + ", a recipe file must contain a file name.");
			if (output == null) throw new Exception("Error at row: " + row + ", a recipe file must contain an output item.");
			if (input.Count == 0) throw new Exception("Error at row :" + row + ", a recipe file must contain at least one input item.");

			r.output = output;
			r.input = input.ToArray();
			if(currencyInputs.Count > 0)
				r.currencyInputs = currencyInputs;
			r.groups = groups.ToArray();

			return new NamedRecipe(r, name);
		}

		private string[] SplitLine(string line)
		{
			return line.Split(new[] { ',' });
		}

		private bool ValidateTopRow(string[] values)
		{
			topRowValues = new Dictionary<int, string>();

			for (int i = 0; i < values.Length; i++)
			{
				string value = values[i];

				if (value.Equals(""))
					throw new Exception("Error at column: " + i + ", header row must not contain any empty cells.");

				if (i > 0 && KeyRequiresQuantity(topRowValues[i - 1]))
					if (!value.Equals(QUANTITY_KEY)) throw new Exception("Error at column: " + i + ", header key: " + topRowValues[i - 1] + " must be followed by a quantity header key.");
				if (i > 0 && value.Equals(QUANTITY_KEY) && !KeyRequiresQuantity(topRowValues[i - 1])) throw new Exception("Error at column: " + i + ", a quantity header key must be preceded by an Input Item, Output Item, or Currency header key.");

				if (value.Equals(OUTPUT_KEY))
				{
					if (topRowValues.ContainsValue(OUTPUT_KEY)) throw new Exception("Top row must only contain one Output Item!");
					topRowValues.Add(i, value);
					continue;
				}

				if (value.Equals(GROUP_KEY))
				{
					topRowValues.Add(i, value);
					continue;
				}

				if (value.Equals(INPUT_KEY))
				{
					topRowValues.Add(i, value);
					continue;
				}

				if (value.Equals(CURRENCY_KEY))
				{
					topRowValues.Add(i, value);
					continue;
				}

				if (value.Equals(QUANTITY_KEY))
				{
					topRowValues.Add(i, value);
					continue;
				}

				if (value.Equals(FILENAME_KEY))
				{
					if (topRowValues.ContainsValue(FILENAME_KEY)) throw new Exception("Header row cannot contain more than one Output Item header key.!");
					topRowValues.Add(i, value);
					continue;
				}

				throw new Exception("Header key: " + value + " at column " + i + " was not recognized! Please ensure only provided header keys are used.");
			}
			return false;
		}

		private bool KeyRequiresQuantity(string key)
		{
			return key.Equals(INPUT_KEY) || key.Equals(OUTPUT_KEY) || key.Equals(CURRENCY_KEY);
		}
		private bool InputContainsKey(List<RecipeItem> input, string key)
		{
			foreach (RecipeItem recipeItem in input)
			{
				if (recipeItem.item.Equals(key)) return true;
			}
			return false;
		}
		#endregion

		private void WriteRecipes(List<NamedRecipe> namedRecipes, string outputFolderPath, bool doPatch)
		{
			if (!Directory.Exists(outputFolderPath)) Directory.CreateDirectory(outputFolderPath);
			try
			{
				foreach (NamedRecipe namedRecipe in namedRecipes)
				{
					string filePath = outputFolderPath + "\\" + namedRecipe.name + ".recipe";
					if (!RecipeJsonHandler.WriteJson(filePath, namedRecipe.recipe, doPatch, overridePatch: false))
						throw new Exception("Failed to write Recipe Json!");
				}
			}
			catch (Exception E)
			{
				throw E;
			}

		}
	}

	public class NamedRecipe
	{
		public Recipe recipe;
		public string name;
		public NamedRecipe(Recipe recipe, string name)
		{
			this.recipe = recipe;
			this.name = name;
		}
	}
}
