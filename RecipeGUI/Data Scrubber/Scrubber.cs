using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RecipeGUI
{
	class Scrubber
	{
		private string[] searchTargetExtensions = new string[] { ".object", ".activeitem", ".item", ".liquid", ".material", ".recipe" };
		private List<string> objectNames;
		private List<string> itemNames;
		private List<string> materialNames;
		private List<string> liquidNames;
		private List<string> groupNames;
		private List<string> currencies;

		private List<string> vanillaCurrencies;
		private List<string> vanillaGroupNames;
		private bool vanillaListsExist = false;
		private double totalFilesInSystem = 0;
		private double scrubbedFiles = 0;
		private bool hasWarned = false;
		private bool exitScrubbing = false;
		private const int WARNING_THRESHOLD = 5000;

		// TODO: Remove this delegate and it calls
		public delegate void StatusEvent(string eventText);
		public StatusEvent statusUpdateEvent;

		public delegate void ProcessCompleteEvent();
		public ProcessCompleteEvent processCompleteEvent;

		public delegate bool FileQuantityWarningEvent();
		public FileQuantityWarningEvent fileQuantityWarningEvent;

		public delegate void FileScrubEvent(string fileName, double percentage);
		public FileScrubEvent fileScrubEvent;

		public void Run(string searchRoot, string outputPath)
		{
			objectNames = new List<string>();
			itemNames = new List<string>();
			materialNames = new List<string>();
			liquidNames = new List<string>();
			groupNames = new List<string>();
			currencies = new List<string>();

			hasWarned = false;
			exitScrubbing = false;
			scrubbedFiles = 0;
			totalFilesInSystem = 0;

			vanillaListsExist = ReadVanillaLists();
			totalFilesInSystem = GetNumberOfFilesInSystem(searchRoot);
			if (exitScrubbing)
			{
				if (statusUpdateEvent != null) statusUpdateEvent(Lang.exportCanceld);
				return;
			}

			if (statusUpdateEvent != null) statusUpdateEvent(Lang.currentlyScrubbing);
			ScrubDirectories(searchRoot);
			if (statusUpdateEvent != null) statusUpdateEvent(Lang.exportingFiles);
			WriteLists(outputPath);
			if (statusUpdateEvent != null) statusUpdateEvent(Lang.exportComplete);


			if (processCompleteEvent != null) processCompleteEvent();
		}

		public void RunAndLoad(string searchRoot, string outputPath, ListLoader listLoader)
		{
			Run(searchRoot, outputPath);
			listLoader.LoadFilesFromDirectory(outputPath);
		}

		public bool ReadVanillaLists()
		{
			string path = Environment.CurrentDirectory + "\\Lists\\Vanilla Lists";

			vanillaCurrencies = new List<string>();
			vanillaGroupNames = new List<string>();

			if (!Directory.Exists(path)) return false;
			ReadList(path + "\\Currencies.list", vanillaCurrencies);
			ReadList(path + "\\Crafting Groups.list", vanillaGroupNames);
			return true;
		}
		public bool ReadList(string path, List<string> list)
		{
			try
			{
				string[] lines = File.ReadAllLines(path);
				foreach (string line in lines)
				{
					list.Add(line);
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void WriteLists(string outputPath)
		{
			if (!Directory.Exists(outputPath)) Directory.CreateDirectory(outputPath);

			if(objectNames.Count > 0)
			WriteList(objectNames, outputPath + "/Object Names.list");
			
			if(liquidNames.Count > 0)
			WriteList(liquidNames, outputPath + "/Liquid Names.list");
			
			if(itemNames.Count > 0)
			WriteList(itemNames, outputPath + "/Item Names.list");
			
			if(materialNames.Count > 0)
			WriteList(materialNames, outputPath + "/Material Names.list");
			
			if(groupNames.Count > 0)
			WriteList(groupNames, outputPath + "/Crafting Groups.List");
			
			if(currencies.Count > 0)
			WriteList(currencies, outputPath + "/Currencies.list");
		}

		public void WriteList(List<string> strings, string path)
		{
			FileStream fileStream = new FileStream(path, FileMode.Create);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			foreach (string item in strings)
			{
				streamWriter.WriteLine(item);
			}
			streamWriter.Close();
		}

		public void ScrubDirectories(string root)
		{
			Stack<string> dirs = new Stack<string>();
			dirs.Push(root);

			while (dirs.Count > 0)
			{
				string dir = dirs.Pop();

				ScrubFilesInDirectory(dir);

				foreach (var item in Directory.EnumerateDirectories(dir))
				{
					dirs.Push(item);
				}
			}
		}

		public void ScrubFilesInDirectory(string path)
		{
			string[] files = Directory.GetFiles(path);
			if (ArrayIsEmpty(files)) return;

			foreach (string file in files)
			{
				if (IsFileSearchTarget(file))
				{
					ScrubFile(file);
				}
				scrubbedFiles++;
				double progress = (scrubbedFiles / totalFilesInSystem) * 100;
				if (fileScrubEvent != null) fileScrubEvent(Path.GetFileName(path), progress);
			}
		}

		public int GetNumberOfFilesInSystem(string root)
		{
			int targetFiles = 0;
			Stack<string> dirs = new Stack<string>();
			dirs.Push(root);

			while (dirs.Count > 0)
			{
				if(targetFiles >= WARNING_THRESHOLD && !hasWarned)
				{
					hasWarned = true;
					if (fileQuantityWarningEvent != null)
						if (fileQuantityWarningEvent())
						{
							exitScrubbing = true;
							return 0;
						}
					
				}
				string dir = dirs.Pop();

				targetFiles += Directory.GetFiles(dir).Length;

				foreach (var item in Directory.EnumerateDirectories(dir))
				{
					dirs.Push(item);
				}
			}
			return targetFiles;
		}

		public void ScrubFile(string path)
		{
			string jsonString = File.ReadAllText(path);
			JObject fileSearch = JObject.Parse(jsonString);

			string extension = Path.GetExtension(path);
			if (extension.Equals(".object"))
			{
				JToken token = fileSearch.SelectToken("objectName");
				objectNames.Add(token.ToString());
			}
			else if (extension.Equals(".item") || extension.Equals(".activeitem"))
			{
				JToken token = fileSearch.SelectToken("itemName");
				itemNames.Add(token.ToString());
			}
			else if (extension.Equals(".material"))
			{
				JToken token = fileSearch.SelectToken("materialName");
				materialNames.Add(token.ToString());
			}
			else if (extension.Equals(".liquid"))
			{
				JToken token = fileSearch.SelectToken("name");
				liquidNames.Add(token.ToString());
			}
			else if (extension.Equals(".recipe"))
			{
				ScrubRecipeFile(path);
			}
		}

		public void ScrubRecipeFile(string path)
		{
			string jsonString = File.ReadAllText(path);
			JObject fileSearch = JObject.Parse(jsonString);

			JToken groupsToken = fileSearch.SelectToken("groups");
			string[] groups = groupsToken.ToObject<string[]>();
			foreach (string group in groups)
			{
				if (groupNames.Contains(group)) return;
				if (vanillaListsExist && vanillaGroupNames.Contains(group)) return;
				groupNames.Add(group);
			}

			JToken currencyInputsToken = fileSearch.SelectToken("currencyInputs");
			if (currencyInputsToken == null) return;
			Dictionary<string, int> currencyInputs = currencyInputsToken.ToObject<Dictionary<string, int>>();
			foreach (string key in currencyInputs.Keys)
			{
				if (currencies.Contains(key)) return;
				if (vanillaListsExist && vanillaCurrencies.Contains(key)) return;
				currencies.Add(key);
			}
		}

		public bool IsFileSearchTarget(string path)
		{
			string extension = Path.GetExtension(path);
			foreach (string searchExtension in searchTargetExtensions)
			{
				if (extension.Equals(searchExtension)) return true;
			}
			return false;
		}

		public static bool ArrayIsEmpty(string[] array)
		{
			return (array.Length == 0) ? true : false;
		}
	}
}
