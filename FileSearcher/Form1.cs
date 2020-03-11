using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearcher
{
	public partial class Form1 : Form
{

		private FileSearcher fs = null;
		
		public Form1()
		{
			InitializeComponent();
			

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
					fs = new FileSearcher("error.log", fbd.SelectedPath);
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
	}
}
