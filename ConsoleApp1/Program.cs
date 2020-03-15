using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{

			Console.WriteLine(1&1&8);
			Console.WriteLine(0&4);
			Console.WriteLine(0&0);

			var x =  Asy();
			
			

			Console.WriteLine(x.Result);
			Console.WriteLine("main method");

			Console.ReadLine();
		}
		static async Task<int> Asy() {
			Console.WriteLine(" Some asyncs");
			var t = await Task.Run(async () => {
				await Task.Delay(2000);
				await Task.Run(() => Console.WriteLine("some sync work"));
				return 5;
			});
			return t;
			
		}
		static void hah(object sender, EventArgs e)
		{
			Console.WriteLine("is evtn");
		}
	}


	class Tss
	{
		public void callEvent() {
			myEvent(EventArgs.Empty);
		}
		public event EventHandler ev;
		public void myEvent(EventArgs e)
		{
			EventHandler handler = ev;
			handler?.Invoke(this, e);
		}
		

	}
}
