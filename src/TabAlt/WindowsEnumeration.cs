using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace TabAlt
{
	public static class WindowsEnumeration
	{
		/// <summary>

		/// Win32 API Imports

		/// </summary>
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);
		[DllImport("user32.dll")]
		private static extern int GetWindowText(int hWnd, StringBuilder title, int size);
		[DllImport("user32.dll")]
		private static extern int GetWindowModuleFileName(int hWnd, StringBuilder title, int size);
		[DllImport("user32.dll")]
		private static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(int hWnd);

		//delegate used for EnumWindows() callback function

		public delegate bool EnumWindowsProc(int hWnd, int lParam);

		// necessary for IEnumerable

		//implement IEnumerable
		public static IEnumerable<ShowableWindow> GetShowableWindows()
		{
			return WindowsEnumeration.GetShowableWindows(false, false);
		}

		public static IEnumerable<ShowableWindow> GetShowableWindows(bool showInvisible, bool showNotitle)
		{
			var windows = new List<ShowableWindow>();
			var processes = Process.GetProcesses(".").ToList();
			EnumWindowsProc ewp = new EnumWindowsProc((hWnd, lParam)=>
			{
				if (showInvisible == false && !IsWindowVisible(hWnd))
					return (true);

				StringBuilder title = new StringBuilder(256);
				StringBuilder module = new StringBuilder(256);

				GetWindowModuleFileName(hWnd, module, 256);
				GetWindowText(hWnd, title, 256);

				if (showNotitle == false && title.Length == 0)
					return (true);

				int processId = 0;
				var x = GetWindowThreadProcessId(new HandleRef(new object{}, new IntPtr(hWnd)), out processId);
				Process process = null;
				try
				{
					process = Process.GetProcessById(processId);
				}
				catch { }
				var window = new ShowableWindow(title.ToString(), (IntPtr)hWnd, process);

				windows.Add(window);
				return true;
			});
			//Enumerate all Windows
			EnumWindows(ewp, 0);
			return windows;


		}

	}
}

