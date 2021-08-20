using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DataSINC
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
	{
		protected override void OnStartup(StartupEventArgs e)
		{
#if DEBUG
			Debug.Active = true;
#else
			if(e.Args.Contains("-debug"))
			{
				Debug.Active = true;
			}
			else
			{
				Debug.Active = false;
			}
#endif

			CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
			Debug.SetExceptionLogger();
			base.OnStartup(e);
		}
	}
}
