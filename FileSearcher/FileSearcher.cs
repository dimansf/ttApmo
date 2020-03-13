using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearcher
{
	/// <summary>
	/// Класс реализующий алгоритм поиска файлов по заданным критериям
	/// </summary>
	class FileSearcher
	{

		private string path = "";
		/// <summary>
		/// Если что то пойдет не так
		/// </summary>
		private static string logFile = "FileSearcher.log";
		

		/// <summary>
		/// Список файлов для сканирования
		/// </summary>
		private List<string> filesList = new List<string>();
		private TreeViewAdder twa;
		private ProgressBar prBar = null;
		private TreeView treeV = null;
		/// <summary>
		/// Для отслеживания сканируемых файлов
		/// </summary>
		private ListBox statusBox = null;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="logFile"> Статичный и общий для всех обьектов класса</param>
		public FileSearcher(List<string> files=null, TreeView tw = null, ProgressBar pb=null, ListBox lb=null)
		{
			filesList = files;
			statusBox = lb;
			//treeV = tw;
			prBar = pb;
			twa = new TreeViewAdder(tw);
		}
		public void setFilelist(List<string> files) {
			filesList = files;
		}
		/// <summary>
		///  Функция поиска файлов по заданным критериям
		/// </summary>
		/// <param name="fnPattern">filename pattern</param>
		/// <param name="dataPattern"></param>
		/// <param name="regex"></param>
		/// <param name="tw"></param>
		/// <returns></returns>
		public bool searchFiles(ref List<string>resultList, string fnPattern, string dataPattern, bool regex=false)
		{
			

			var rx = new Regex(fnPattern);
			var dataRx = new Regex(dataPattern);


			string oldname = "";
			filesList?.ForEach(name =>
			{
				if (rx.IsMatch(name)) {
					
					if (File.Exists(name)) {
						try
						{
							// статус файла в обработке
							setStatus(oldname, name);
							
							using (StreamReader sr = new StreamReader(name))
							{
								string line;
								
								while ((line = sr.ReadLine()) != null)
								{
									if (dataRx.IsMatch(line))
									{
										twa.Add(name);
										resultList.Add(name);
									}

								}
							}
							// файл обработан
							prBar?.PerformStep();
						}
						catch (Exception e)
						{
							// Let the user know what went wrong.
							Console.WriteLine("The file could not be read:");
							File.AppendAllText(logFile, e.Message);
							
						}

					}
				}
			});

			return false;

		}

		public void setStatus(string oldName, string name ) {
			var items = statusBox?.Items;
			items?.Remove(name);
			items?.Add(oldName);
		}

		public static List<string> scanDir(string dir)
		{
			var filesList = new List<string>();
			try
			{

				var dirs = Directory.GetDirectories(dir);
				var files = Directory.GetFiles(dir);
				filesList.AddRange(files);
				dirs.Select(dd => scanDir(dd));
				

			} catch (Exception ex) {
				File.AppendAllText(logFile, ex.Message);
				return filesList;
			}
			return filesList;
		}

	}
}
