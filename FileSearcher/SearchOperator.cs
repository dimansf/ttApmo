using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearcher
{
	/// <summary>
	/// Класс реализующий алгоритм поиска файлов по заданным критериям
	/// </summary>
	public class SearchOperator
	{
		/// <summary>
		/// директория с которой идет сканирование
		/// </summary>
		public string BaseDir { get; private set; }
		///// <summary>
		///// Кол-во файлов 
		///// </summary>
		//public int CountFiles { get { return filesPath.Count; } }
		/// <summary>
		/// Токен для отмены долгих операций
		/// </summary>
		public CancellationTokenSource CancelToken { get; private set; }
		/// <summary>
		/// Если что то пойдет не так
		/// </summary>
		private static string logFile = "FileSearcher.log";

		public event EventHandler onCurrentScanFile;
		public event EventHandler onSuccessSearch;
		/// <summary>
		/// Список файлов для сканирования
		/// </summary>
		public Queue<string> filesPath { get; private set; }


		//private List<string> result = new List<string>();
		public List<string> Result { get; private set; }



		/// <summary>
		/// 
		/// </summary>
		/// <param name="logFile"> Статичный и общий для всех обьектов класса</param>
		public SearchOperator(string path)
		{
			Result = new List<string>();
			CancelToken = new CancellationTokenSource();
			filesPath = new Queue<string>();
			BaseDir = path;

		}

		/// <summary>
		///  Функция поиска файлов по заданным критериям
		/// </summary>
		/// <param name="fnPattern">filename pattern</param>
		/// <param name="dataPattern"></param>

		/// <returns></returns>
		public bool searchFiles(string fnPattren, string cPattern, CancellationToken? ct = null)
		{

			var fileNamePattern = new Regex(fnPattren);
			var dataContentPattern = new Regex(cPattern);

			var oldName = "";
			while (filesPath.Count != 0) {
				ct?.ThrowIfCancellationRequested();
				
				var name = filesPath.Dequeue();


				var ca = new CustomArgs();
				ca.fileName = name;
				ca.oldName = oldName;
				// файл поступил в обработку
				CurrentScanFile(ca);

				if (File.Exists(name) && fileNamePattern.IsMatch(name))
				{
					try
					{
						using (StreamReader sr = new StreamReader(name))
						{
							string line;

							while ((line = sr.ReadLine()) != null)
							{
								if (dataContentPattern.IsMatch(line))
								{

									SuccessSearch(ca);
									Result.Add(name);
									break;
								}
							}
						}
					}
					catch (Exception e)
					{
						File.AppendAllText(logFile, e.Message);
					}
				}
				oldName = name;
			}

			return true;
		}

		private void CurrentScanFile(EventArgs e)
		{
			EventHandler handler = onCurrentScanFile;
			handler?.Invoke(this, e);
		}
		private void SuccessSearch(EventArgs e)
		{
			EventHandler handler = onSuccessSearch;
			handler?.Invoke(this, e);
		}

		public bool scanDir(string dir = null, CancellationToken? ct = null) {
			var d = dir == null ? BaseDir : dir;
			return _scanDir(d, filesPath, ct);
		}

		public bool _scanDir(string dir, Queue<string> filesList, CancellationToken? ct = null)
		{
			ct?.ThrowIfCancellationRequested();
			try
			{

				var dirs = Directory.GetDirectories(dir);
				var files = Directory.GetFiles(dir);
				files.Select(f => { filesList.Enqueue(f); return 0; }).ToArray();
				var res = dirs.Select(dd => _scanDir(dd, filesList,ct)).ToArray();
			}
			catch (Exception ex)
			{
				File.AppendAllText(logFile, ex.Message);
				return false;
			}
			return true;
		}

	}
	public class CustomArgs : EventArgs
	{
		public string fileName { get; set; }
		public string oldName { get; set; }

	}
}
