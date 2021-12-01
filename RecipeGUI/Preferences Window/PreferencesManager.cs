using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RecipeGUI.Preferences_Window
{
	public class PreferencesManager
	{
		public bool doAutocomplete = true;
		public bool doCreatePatch = false;
		public bool doOverridePatchFile = false;

		private string configPath;

		public PreferencesManager()
		{
			configPath = Environment.CurrentDirectory + "\\Config.config";
		}

		public PreferencesManager(bool doAutocomplete, bool doCreatePatch, bool doOverridePatchFile)
		{
			configPath = Environment.CurrentDirectory + "\\Config.config";
			this.doAutocomplete = doAutocomplete;
			this.doCreatePatch = doCreatePatch;
			this.doOverridePatchFile = doOverridePatchFile;
		}

		public void WriteConfig()
		{
			string jsonString = JsonConvert.SerializeObject(this, Formatting.Indented);
			File.WriteAllText(configPath, jsonString);
		}

		public void ReadConfig()
		{
			string jsonString = File.ReadAllText(configPath);
			var prefs = JsonConvert.DeserializeObject<PreferencesManager>(jsonString);
			doAutocomplete = prefs.doAutocomplete;
			doCreatePatch = prefs.doCreatePatch;
			doOverridePatchFile = prefs.doOverridePatchFile;
		}
	}
}
