using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Diagnostics;

namespace TabAlt
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private static int _processId = Process.GetCurrentProcess().Id;
		public static int ProcessId
		{
			get
			{
				return _processId;
			}
		}
		protected override void OnStartup(StartupEventArgs e)
		{
			RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
		}
	}
}
