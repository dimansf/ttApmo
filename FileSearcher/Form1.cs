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

		private int chapter = 1;
		private string currentFile = "";

		private DateTime runDate = DateTime.Now;


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

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					fileSearcher = new SearchOperator(fbd.SelectedPath);
					fileSearcher.onCurrentScanFile += (a, args) =>
					{
						var ar = args as CustomArgs;
						currentFile = ar.fileName;

					};
					label1.Text = selectedDir;

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

		private void button3_Click(object sender, EventArgs e)
		{
			cancelToken.Cancel();
			cancelToken = new CancellationTokenSource();

			treeView1.Nodes.Clear();
			progressBar1.Value = 0;
			label1.Text = "";
			label6.Text = "";
			label3.Text = "";
			button2.Text = button2Text[Convert.ToInt32(button2Pos.RUN)];
			button3.Visible = false;
			chapter = 1;

		}



		private void updateChecker(object sender, EventArgs e)
		{
			var diff = DateTime.Now - runDate;

			label6.Text = ((diff.Hours != 0) ? $" {diff.Hours} часов " : "") +
				((diff.Minutes != 0) ? $" {diff.Minutes} минут " : "") +
				$"{diff.Seconds} секунд ";

			label3.Text = currentFile;
			treeAdder.Add(fileSearcher.Result);
		}

		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void button2_Click(object sender, EventArgs e)
		{

			// лучше бы через enum сделать
			if (button2.Text == /*Запуск*/button2Text[Convert.ToInt32(button2Pos.RUN)])
			{
				await run();

			}
			else if (button2.Text == /*Приостановить*/button2Text[Convert.ToInt32(button2Pos.SUSPEND)])
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
			timer.Start();
			if (textBox1.Text.Count() == 0 ||
					textBox2.Text.Count() == 0 ||
					fileSearcher.BaseDir.Length == 0)
			{
				var confF = new ConflictForm();
				confF.ShowDialog(this);
				return;
			}
			button2.Text =/*Приостановить*/ button2Text[Convert.ToInt32(button2Pos.SUSPEND)];
			button3.Visible = true;

			if (chapter == 1)
			{
				

				runDate = DateTime.Now;
				label2.Text = "Идет сканирование директории";

				try
				{
					await Task.Run(() => fileSearcher.scanDir(fileSearcher.BaseDir, cancelToken.Token), cancelToken.Token);
					progressBar1.Maximum = fileSearcher.filesPath.Count;
				}
				catch (OperationCanceledException)
				{
					return;
				}
				
				chapter = 2;
			}
			if (chapter == 2)
			{
				label2.Text = "Идет поиск";

				try
				{
					await Task.Run(() => fileSearcher.searchFiles(textBox1.Text, textBox2.Text, cancelToken.Token),
						cancelToken.Token);
				}
				catch (OperationCanceledException)
				{
					return;
				}

				endStation();
				chapter = 1;

			}
		}
		
		private void endStation() {
			timer.Stop();
			label2.Text = "Готово";
			button2.Text = button2Text[Convert.ToInt32(button2Pos.RUN)];
			button3.Visible = false;
			progressBar1.Value = 0;
			textBox1.Text = "";
			textBox2.Text = "";
			label3.Text = "";
		}
		
		private void suspendStation()
		{
			timer.Stop();
			cancelToken.Cancel();
			cancelToken = new CancellationTokenSource();
			button2.Text = button2Text[Convert.ToInt32(button2Pos.RESUME)];
		}
		
		private async Task resumeStation()
		{
			await run();

		}
	}
}
