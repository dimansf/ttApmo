using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSearcher;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace ApmoTests
{
	[TestClass]
	public class SearchOperatorTest
	{
		string dir = "../../testdir";
		string dir2 = @"C:\Program Files";
		//string dir = @"C:\Users\dimansf\Downloads\aaaa";
		//private ProgressBar pr;
		[TestMethod]
		public void shallowSearchFilesTest()
		{
				var fs3 = new SearchOperator(dir);
				fs3.scanDir();
				Assert.AreEqual(fs3.filesPath.Count, 7);

				fs3.searchFiles("file12", "12$");
				Assert.AreEqual(fs3.Result.Count, 1);
				Assert.AreEqual(0, fs3.filesPath.Count);

		}
		[TestMethod]
		public void interruptionScanDirTest() {
			try
			{
				var fs3 = new SearchOperator(dir2);
				try
				{
					var ts = new CancellationTokenSource();
					var tsk = Task.Run(() => fs3.scanDir(null, ts.Token), ts.Token);
					Thread.Sleep(1000);
					fs3.CancelToken.Cancel();
				}
				catch(Exception e) {
					Console.WriteLine(e.Message);
					Assert.Fail();
				}
				var count = fs3.filesPath.Count;
				var res2 =  Task.Run(() => fs3.scanDir());
				res2.Wait();
				
				Assert.IsTrue(count < fs3.filesPath.Count);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex.Message);
				Assert.Fail();

			}
		}

		[TestMethod]
		public void interruptionSearchFileTest()
		{
			var fs3 = new SearchOperator(@"C:\Program Files\Java");
			fs3.scanDir();
			////chapter 1
			var count = fs3.filesPath.Count;
			var t = new CancellationTokenSource();
			var ts = Task.Run(() => fs3.searchFiles(".", ".", t.Token), t.Token);
			Thread.Sleep(400);
			t.Cancel();
			//ts.Wait();
			var c1 = count - fs3.filesPath.Count;

			////chapter 2
			t = new CancellationTokenSource();
			ts = Task.Run(() => fs3.searchFiles(".", ".", t.Token), t.Token);
			Thread.Sleep(400);
			t.Cancel();
			//ts.Wait();
			//t.Dispose();
			var c2 = count - fs3.filesPath.Count;

			//chapter3
			ts = Task.Run(() => fs3.searchFiles(".", "."));
			ts.Wait();
			var c3 = count - fs3.filesPath.Count - c1 - c2;
			Assert.AreEqual(count, c1 + c2 + c3);
			//Assert.AreEqual(fs3.filesPath.Count, 0);
		}

		
		public void searchFilesTest() {
			var fs3 = new SearchOperator(dir2);
			fs3.scanDir();
			var cc = fs3.filesPath.Count;
			fs3.searchFiles(".", ".");
			Assert.AreEqual( 0, fs3.filesPath.Count);
			Assert.AreEqual(cc, fs3.Result.Count);

		}

		[TestMethod]
		public void regexTest()
		{
			var file = dir +  @"\aaaa\dir1\file12.txt";

			var rx = new Regex(@"12$");

			using (StreamReader sr = new StreamReader(file))
			{
				string line;

				while ((line = sr.ReadLine()) != null)
				{
					if (rx.IsMatch(line))
					{

						Assert.IsTrue(true);
					}

				}
			}
			
		}
		//[TestMethod]
		//public void scanDirTest() {

			

			//Assert.AreEqual(6, FileSearch.scanDir(dir).Count);

			//}
	}
}
