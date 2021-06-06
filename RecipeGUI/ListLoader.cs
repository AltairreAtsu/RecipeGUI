using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI
{
	class ListLoader
	{
		private string listDirectoryPath = Environment.CurrentDirectory + "\\Lists";

		public List<string> nameStrings;
		public List<string> categoryStrings;
		public List<string> currencyStrings;

		public void LoadNamesIntoLists()
		{
			nameStrings = new List<string>();
			categoryStrings = new List<string>();
			currencyStrings = new List<string>();

			if (!Directory.Exists(listDirectoryPath))
			{
				Directory.CreateDirectory(listDirectoryPath);
				return;
			}
			string[] subDirectories = Directory.GetDirectories(listDirectoryPath);
			foreach (string directory in subDirectories)
			{
				LoadFilesFromDirectory(directory);
			}
		}

		private void LoadFilesFromDirectory(string subDirectory)
		{
			ReadFileToList(subDirectory + "\\Item Names.list", nameStrings);
			ReadFileToList(subDirectory + "\\Material Names.list", nameStrings);
			ReadFileToList(subDirectory + "\\Object Names.list", nameStrings);
			ReadFileToList(subDirectory + "\\Liquid Names.list", nameStrings);
			ReadFileToList(subDirectory + "\\Crafting Groups.list", categoryStrings);
			ReadFileToList(subDirectory + "\\Currencies.list", currencyStrings);
		}

		private void ReadFileToList(string path, List<string> list)
		{
			if (!File.Exists(path)) return;

			FileStream fileStream = new FileStream(path, FileMode.Open);
			StreamReader streamReader = new StreamReader(fileStream);

			string nextLine = streamReader.ReadLine();
			while (nextLine != null)
			{
				list.Add(nextLine);
				nextLine = streamReader.ReadLine();
			}

			streamReader.Close();
		}
	}
}
