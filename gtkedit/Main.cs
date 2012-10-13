using System;
using Gtk;

namespace gtkedit
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			
			string filearg = "";
			
			if (args.Length == 1)
				filearg = args[0];

			MainWindow win = new MainWindow (filearg);
			win.Show ();
			Application.Run ();
		}
	}
}
