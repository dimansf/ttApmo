using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;



namespace FileSearcher
{
	public partial class Form1 : Form
	{


		private string[] button2Text = new string[] { "Запуск",  "Приостановить", "Продолжить" };
		private enum button2Pos {RUN = 0, SUSPEND  =1, RESUME = 2};
		private string selectedDir = "";
		private SearchOperator fileSearcher;
		private bool newRun = true;
		private Task task;

		private int chapter = 1;
		private string currentFile = "";

		private TimeSpan runDate = new TimeSpan(0,0,0);


		System.Windows.Forms.Timer timer;

		private CancellationTokenSource cancelToken;
		private TreeViewAdder treeAdder;

		public Form1()
		{
			InitializeComponent();

			
			treeAdder = new TreeViewAdder(treeView1);

			timer = new System.Windows.Forms.Timer();
			timer.Interval = 1000;
			timer.Tick += updateChecker;
			

			cancelToken = new CancellationTokenSource();
			

		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private void button1_Click(object sender, EventArgs e)
		{


			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();
				var path = fbd.SelectedPath;
				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(path))
				{
					button3_Click(null, null);
					fileSearcher = new SearchOperator(path);
					fileSearcher.onCurrentScanFile += (a, args) =>
					{
						var ar = args as CustomArgs;
						currentFile = ar.fileName;
					};
					label1.Text = path;

				}
			}
		}

		private void label1_Click(object sender, EventArgs e)
		{

		}

		private void label2_Click_1(object sender, EventArgs e)
		{

		}

		private void groupBox1_Enter(object sender, EventArgs e)
		{

		}

		private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// Полный сброс
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, EventArgs e)
		{
			try
			{
				cancelToken.Cancel();
				cancelToken.Dispose();
			}
			catch { 
			}
			fileSearcher = null;
			cancelToken = new CancellationTokenSource();
			newRun = true;
			timer.Stop();
			button1.Enabled = true;
			treeView1.Nodes.Clear();
			treeView1.Update();
			progressBar1.Value = 0;
			progressBar1.Visible = true;
			label1.Text = "";
			label6.Text = "";
			label3.Text = "";
			label4.Text = "";
			button2.Text = button2Text[Convert.ToInt32(button2Pos.RUN)];
			button3.Visible = false;
			chapter = 1;

		}


		/// <summary>
		/// ОБновление данных в ui потоке
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void updateChecker(object sender, EventArgs e)
		{
			runDate = runDate.Add(new TimeSpan(0, 0, 1));
			var diff = runDate;

			label6.Text = ((diff.Hours != 0) ? $" {diff.Hours} часов " : "") +
				((diff.Minutes != 0) ? $" {diff.Minutes} минут " : "") +
				$"{diff.Seconds} секунд ";

			label3.Text = currentFile;
			var fls = Convert.ToString(fileSearcher?.Result.Count);
			label4.Text = fls == "0" ? "" : fls;
			treeAdder.Add(fileSearcher?.Result);
			treeView1.Update();
			if(chapter == 2) 
				progressBar1.Value = 
					progressBar1.Maximum - fileSearcher.filesPath.Count;
		}

		/// <summary>
		/// Запуск приостановка возобновление
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void button2_Click(object sender, EventArgs e)
		{

			// лучше бы через enum сделать
			if (button2.Text == button2Text[Convert.ToInt32(button2Pos.RUN)])
			{
				await  run();
				
			}
			else if (button2.Text ==button2Text[Convert.ToInt32(button2Pos.SUSPEND)])
			{
				suspendStation();
			}
			// Продолжить
			else
			{
				await resumeStation();
				
			}
		}
		private async Task run()
		{
			
			if (textBox1.Text.Count() == 0 ||
					textBox2.Text.Count() == 0 ||
					fileSearcher == null ||
					fileSearcher?.BaseDir.Length == 0)
			{
				var confF = new ConflictForm();
				confF.ShowDialog(this);
				return;
			}

			button1.Enabled = false;
			
			timer.Start();
			
			button2.Text =/*Приостановить*/ button2Text[Convert.ToInt32(button2Pos.SUSPEND)];
			button3.Visible = true;
			if (newRun) {
				newRun = !newRun;
				runDate = new TimeSpan(0, 0, 0);
			}

			if (chapter == 1)
			{

				Task tt;
				
				label2.Text = "Идет сканирование директории";

				try
				{
					tt = Task.Run(() => fileSearcher.scanDir(fileSearcher.BaseDir, cancelToken.Token), cancelToken.Token);
					await tt;


				}
				catch
				{
					Console.WriteLine("");
				}
				finally {
					if (!cancelToken.Token.IsCancellationRequested)
					{
						progressBar1.Maximum = fileSearcher.filesPath.Count;
						chapter = 2;
					}
				}
			}

			if (chapter == 2)
			{
				label2.Text = "Идет поиск";
				try
				{
					var tt = Task.Run(() => fileSearcher.searchFiles(textBox1.Text, textBox2.Text, cancelToken.Token),
						cancelToken.Token);
					await tt;
					
				}
				catch
				{
					Console.WriteLine("");
				}
				finally {
					if (!cancelToken.Token.IsCancellationRequested)
					{
						endStation();
						chapter = 1;
					}
				}

			}
			
			
		}
		
		private void endStation() {
			timer.Stop();
			newRun = true;
			button1.Enabled = true;
			label2.Text = "Готово";
			button2.Text = button2Text[Convert.ToInt32(button2Pos.RUN)];
			button3.Visible = false;
			label3.Text = "";
			progressBar1.Visible = false;
		}
		
		private void suspendStation()
		{
			timer.Stop();
			label2.Text = "";
			cancelToken.Cancel();
			
			button2.Text = button2Text[Convert.ToInt32(button2Pos.RESUME)];
		}
		
		private async Task resumeStation()
		{
			cancelToken = new CancellationTokenSource();
			await run();

		}
	}
}
