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


		/// <summary>
		/// Список файлов с полным путем 
		/// </summary>
		public List<string> Result { get; private set; }



		
		public SearchOperator(string path)
		{
			Result = new List<string>();
			CancelToken = new CancellationTokenSource();
			filesPath = new Queue<string>();
			BaseDir = path;

		}

		/// <summary>
		/// Поиск файла по условию
		/// </summary>
		/// <param name="fnPattren"></param>
		/// <param name="cPattern"></param>
		/// <param name="ct"></param>
		/// <returns></returns>
		public bool searchFiles(string fnPattren, string cPattern, CancellationToken? ct = null)
		{

			var fileNamePattern = new Regex(fnPattren);
			var dataContentPattern = new Regex(cPattern);

			var oldName = "";
			while (filesPath.Count != 0) {
				ct?.ThrowIfCancellationRequested();
				
				var name = filesPath.Peek();


				var ca = new CustomArgs();
				ca.fileName = name;
				ca.oldName = oldName;
				// файл поступил в обработку
				CurrentScanFile(ca);

				if (File.Exists(name) && fileNamePattern.IsMatch(name))
				{
					try
					{
						// по дефолту utf-8 кодировка 
						using (StreamReader sr = new StreamReader(name))
						{
							string line;

							while ((line = sr.ReadLine()) != null)
							{
								ct?.ThrowIfCancellationRequested();

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
					finally {
						filesPath.Dequeue();
						oldName = name;
					}
				}
				// удаляем файл из очереди после того как проверим
				
			}

			return true;
		}
		/// <summary>
		/// Текущий обрабатываемый файл
		/// </summary>
		/// <param name="e"></param>
		private void CurrentScanFile(EventArgs e)
		{
			EventHandler handler = onCurrentScanFile;
			handler?.Invoke(this, e);
		}
		/// <summary>
		/// Удачное совпадение
		/// </summary>
		/// <param name="e"></param>
		private void SuccessSearch(EventArgs e)
		{
			EventHandler handler = onSuccessSearch;
			handler?.Invoke(this, e);
		}
		/// <summary>
		/// Сканирует директорию и создает список всех файлов по абсолютному пути 
		/// </summary>
		/// <param name="e"></param>
		public bool scanDir(string dir = null, CancellationToken? ct = null) {
			ct?.ThrowIfCancellationRequested();
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
