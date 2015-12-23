using System;
using Gtk;

namespace DflatGtk
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			/* TODO: Construct dependency graph here  */
			Application.Init ();
			MainWindow win = new MainWindow ();
			win.Show ();
			Application.Run ();
		}
	}
}
