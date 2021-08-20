using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace DataSINC
{
	public static class Debug
	{
		public static bool Active = true;
		private static bool isfirst = true;
		private static readonly string path = Path.Combine(Application.StartupPath, "log.txt");

		public enum LogType
		{
			None,
			Info,
			Warning,
			Exception,
			Error
		}

		public static void SetExceptionLogger()
		{
			AppDomain.CurrentDomain.FirstChanceException += FirstChanceExceptionLogger;
			AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionLogger;
		}

		private static void UnhandledExceptionLogger(object sender, UnhandledExceptionEventArgs e)
		{
			Exception(e.ExceptionObject as Exception, false);
		}

		private static void FirstChanceExceptionLogger(object sender, FirstChanceExceptionEventArgs e)
		{
			Exception(e.Exception, false);
		}

		private static void Log(string msg, LogType type = LogType.None)
		{
			if(isfirst)
			{
				isfirst = false;
				FirstStart();
			}

			Directory.CreateDirectory(new FileInfo(path).DirectoryName);
			using (StreamWriter sw = new StreamWriter(path, true))
			{
				if(type == LogType.None)
				{
					sw.WriteLine(msg);
				}
				else
				{
					if(!Active && type != LogType.Exception)
					{
						return;
					}
					sw.WriteLine(DateTime.Now + ": [" + type.ToString() + "] " + msg);
					System.Diagnostics.Debug.WriteLine(msg, "Debug (" + type.ToString() + ")");
				}
			}
		}

		private static void FirstStart()
		{
			if (File.Exists(path)) { File.Delete(path); }

			//GET OS INFORMATIONS
			Log("System Informations");

			ManagementObjectSearcher mos = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
			foreach (ManagementObject managementObject in mos.Get())
			{
				if (managementObject["Caption"] != null)
				{
					Log("OS Name: " + managementObject["Caption"].ToString());   //Display operating system caption
				}
				if (managementObject["OSArchitecture"] != null)
				{
					Log("OS Architecture: " + managementObject["OSArchitecture"].ToString());   //Display operating system architecture.
				}
				if (managementObject["CSDVersion"] != null)
				{
					Log("OS Service Pack: " + managementObject["CSDVersion"].ToString());     //Display operating system version.
				}
			}
			Microsoft.VisualBasic.Devices.ComputerInfo ci = new Microsoft.VisualBasic.Devices.ComputerInfo();
			Log("OS Version: " + ci.OSVersion);
			Log("Physical Memory (Free): " + ci.AvailablePhysicalMemory + "b => " + (((ci.AvailablePhysicalMemory / 1000) / 1000) / 1000) + "gb");
			Log("Physical Memory (Total): " + ci.TotalPhysicalMemory + "b => " + (((ci.TotalPhysicalMemory / 1000) / 1000) / 1000) + "gb");
			RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);   //This registry entry contains entry for processor info.

			if (processor_name != null)
			{
				if (processor_name.GetValue("ProcessorNameString") != null)
				{
					Log("Processor: " + (string)processor_name.GetValue("ProcessorNameString"));   //Display processor ingo.
				}
			}

			Log("");
			Info("Starting application...");
		}

		public static void Info(string msg)
		{
			Log(msg, LogType.Info);
		}

		public static void Warn(string msg)
		{
			Log(msg, LogType.Warning);
		}

		public static void Error(string msg) 
		{
			Log(msg, LogType.Error);
		}

		public static void Exception(Exception ex, bool throwexeption = true)
		{
			string msg = "Message: " + ex.Message + "\n";

			if(ex.Data != null)
			{
				foreach (var data in ex.Data)
				{
					msg += data.ToString() + "\n";
				}
			}
			Log(msg, LogType.Exception);

			if(throwexeption)
			{
				throw ex;
			}
		}
	}
}
