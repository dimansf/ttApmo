using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearcher
{
	class FileSearcher
	{
		private string path = "";
		private string logFile = "";
		private ListBox statusBox = null;


		private List<string> filesList = new List<string>();
		public FileSearcher(string logFile, string path, ListBox statusBox=null)
		{
			this.statusBox = statusBox;
			this.logFile = logFile;
			this.path = path;
		}
		
		public string[] searchFiles(string pattern)
		{
			fillFilesList(path);

			return null;

		}

		public int fillFilesList(string dir)
		{
			try
			{

				var dirs = Directory.GetDirectories(dir);
				var files = Directory.GetFiles(dir);
				filesList.AddRange(files);
				dirs.Select(dd => fillFilesList(dd));
				

			} catch (Exception ex) {
				File.AppendAllText(logFile, ex.Message);
				return 0;
			}
			return 1;
		}

	}
}
