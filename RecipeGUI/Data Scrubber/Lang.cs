using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeGUI
{
	class Lang
	{
		public static string currentlyScrubbing = "Currently scrubbing provided mod directory.";
		public static string exportingFiles = "Scrubbing complete. Now exporting files to lists.";
		public static string exportComplete = "Export complete.";

		public static string modDoesNotExist = "Error: Could no locate mod directory!";
		public static string outputDoesNotExist = "Error: Could no locate output directory!";

		public static string metadataFileNotFoundTitle = "Error: Metadata file missing";
		public static string metadataFileNotFoundBody = "The selected target directory does not contain a metadata file. Do you want to proceed?";
	}
}
