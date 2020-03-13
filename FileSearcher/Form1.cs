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

		private FileSearcher fs = null;
		private List<Thread> threads = null;
		private string[] button2Text = new string[] { "Запуск", "Продолжить", "Приостановить" };
		private string dir = "";
		private List<string> selectionList = new List<string>();
		private DateTime runDate = DateTime.Now;
		private bool isRunning = false;
		private static System.Timers.Timer aTimer = new System.Timers.Timer(1000);
		private TreeViewAdder tw = null;

		public Form1()
		{
			InitializeComponent();
			threads = new List<Thread>();
			tw = new TreeViewAdder(treeView1);
			//fs = new FileSearcher(null, treeView1, progressBar1, listBox1);


		}

		private void radioButton1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void label2_Click(object sender, EventArgs e)
		{

		}

		private  void button1_Click(object sender, EventArgs e)
		{
			using (var fbd = new FolderBrowserDialog())
			{
				DialogResult result = fbd.ShowDialog();

				if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
				{
					dir = fbd.SelectedPath;
					
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
			threads.ForEach(th => th.Abort());
			button2.Text = button2Text[0];
			button3.Visible = false;
		}

		private async void watch(bool toStop = false) {

			// Create a timer with a two second interval.
			try {

				if (toStop)
				{
					aTimer.Stop();
				}
			} catch 
			{
				return;
			}

			// Hook up the Elapsed event for the timer. 
			aTimer.Elapsed += updateChecker;
			aTimer.AutoReset = true;
			aTimer.Enabled = true;

			

			
		}
		private async void updateChecker(Object source, ElapsedEventArgs e)
		{

			if (threads.All(th => th.ThreadState == ThreadState.Stopped)) {
				aTimer.Stop();
				return;
			};
			var diff = DateTime.Now - runDate;

			label6.Text = $"{diff.Seconds} секунд" +
				((diff.Minutes != 0) ? $" {diff.Minutes} минут" : "") +
				((diff.Hours != 0) ? $" {diff.Hours} часов" : "");

			tw.Add(selectionList);
			

		}
		private void button2_Click(object sender, EventArgs e)
		{
			
			// лучше бы через enum сделать
			if (button2.Text == /*Запуск*/button2Text[0])
			{
				runDate = DateTime.Now;
				
				watch();
				


				if ((textBox1.Text.Count() & textBox2.Text.Count()) == 0) {
				
					var confF = new ConflictForm();
					ShowDialog(confF);
				}

				var files = FileSearcher.scanDir(dir);

				selectionList = new List<string>();

				// наплодим тредов по фану попробуем распаралелить
				var threadsNum = Environment.ProcessorCount;
				var interval = files.Count / threadsNum;

				for (var i = 0; i < threadsNum; i++)
				{
					var rangeList = threadsNum - i != 1 ? 
						files.GetRange(i * interval, interval) : 
						files.GetRange(i * interval, files.Count - i * interval);
					var fs = new FileSearcher(rangeList, treeView1, progressBar1, listBox1);
					
					
					threads.Add(new Thread(() => 
						fs.searchFiles(ref selectionList, textBox1.Text, textBox2.Text)));
					

				}
				

				button3.Visible = true;
			}
			else if (button2.Text == /*Приостановить*/button2Text[2]) {
				// херовая идея но пока так 
				threads.ForEach(th => th.Suspend());
				watch(true);
			}
			// Продолжить
			else
			{
				//  задепрекейчены 
				threads.ForEach(th => th.Resume());
				watch();
				

			}
		}
	}
}
