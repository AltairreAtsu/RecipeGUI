using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace RecipeGUI
{
	class RecipeJsonWriter
	{
		public static bool WriteJson(string path, Recipe recipe, bool doPatch)
		{
			string completePath = path + "\\" + recipe.output.item + ".recipe";

			JsonSerializerSettings settings = new JsonSerializerSettings();
			settings.Formatting = Formatting.Indented;
			settings.NullValueHandling = NullValueHandling.Ignore;
			string jsonString = JsonConvert.SerializeObject(recipe, settings);
			try
			{
				File.WriteAllText(completePath, jsonString);
				if (doPatch)
				{
					string patchPath = path + "\\player.config.patch";
					if (File.Exists(patchPath))
					{
						List<string> contents = File.ReadAllLines(patchPath).ToList();
						contents[contents.Count - 2] = contents[contents.Count - 2] + ",";
						contents.Insert(contents.Count - 1, "{\"op\":\"add\", \"path\": \"/defaultBlueprints/tier1/-\", \"value\": {\"item\": \"" + recipe.output.item + "\"}}");
						File.WriteAllLines(patchPath, contents);
						return true;
					}
					string patchContent = "[\n{\"op\":\"add\", \"path\": \"/defaultBlueprints/tier1/-\", \"value\": {\"item\": \"" + recipe.output.item + "\"}}\n]";
					File.WriteAllText(patchPath, patchContent);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
