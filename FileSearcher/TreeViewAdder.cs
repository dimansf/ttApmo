using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearcher
{
	
	class TreeViewAdder
	{

		TreeView tw = null;
		public TreeViewAdder(TreeView tw) {
			this.tw = tw;
		}
		
		
		public async void Add( List<string> files) {
			
			tw.BeginUpdate();
			files.Select(name => treeViewAdder(name));
			tw.EndUpdate();
			
		}
		private int treeViewAdder(string file)
		{
			var shards = new Queue<string>(file.Split('\\'));
			_treeViewAdder(tw.Nodes, shards);

			return 0;

		}
		/// <summary>
		/// Для рекурсивного добавления в дерево
		/// не нашел готового добавления по TreeNode.Fullpath
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="shards"></param>
		private void _treeViewAdder(TreeNodeCollection tr, Queue<string> shards)
		{
			string ss;
			if (shards.Count == 0) return;

			if (tr.IndexOfKey(ss = shards.Dequeue()) == -1)
			{
				tr.Add(ss);
				_treeViewAdder(tr[ss].Nodes, shards);
			}
			else
			{
				_treeViewAdder(tr[ss].Nodes, shards);
			}
		}
	}
}
