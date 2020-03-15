using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FileSearcher;
using System.Collections.Generic;

namespace ApmoTests
{
	[TestClass]
	public class TreeViewAdderTest
	{
		[TestMethod]
		public void TestMethod1()
		{
		}

		[TestMethod]
		public void AddTest()
		{
			var tree = new TreeView();
			tree.Nodes.Add("a");
			tree.Nodes.Add("b");
			tree.Nodes[0].Nodes.Add("c");
			tree.Nodes[1].Nodes.Add("d");
			tree.Nodes[1].Nodes[0].Nodes.Add("f");

			Assert.AreEqual(tree.Nodes[0].Text, "a");
			Assert.AreEqual(tree.Nodes[1].Text, "b");
			Assert.AreEqual(tree.Nodes[1].Nodes[0].Text, "d");
			
			tree.Nodes.Clear();
		}
		[TestMethod]
		public void RecursiveAddTest() {

			var tree = new TreeView();
			var adder = new TreeViewAdder(tree);

			var item1 = @"C:\Program Files\Java\jdk-13.0.1\release";
			var item2 = @"C:\Windows";
			var l = new List<string>() { item1, item2 };
			adder.Add(l);

			Assert.AreEqual(adder.indexOfText(tree.Nodes, "C:"), 0);
			Assert.AreEqual(adder.indexOfText(tree.Nodes[0].Nodes, "Windows"), 1);

		}

	}
}
