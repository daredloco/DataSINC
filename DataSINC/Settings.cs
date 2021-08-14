using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSINC
{
	public static class Settings
	{
		public static string latestmod;

		private static readonly string location = System.IO.Path.Combine(Application.StartupPath, "app.cfg");

		public static void Load()
		{
			if(!System.IO.File.Exists(location))
			{
				latestmod = Application.StartupPath;
				return;
			}
			string[] flines = System.IO.File.ReadAllLines(location);
			latestmod = flines[0];
		}

		public static void Save()
		{
			string[] flines = new string[] {latestmod };
			if (System.IO.File.Exists(location))
			{
				System.IO.File.Delete(location);
			}

			System.IO.File.WriteAllLines(location,flines);
		}
	}
}
