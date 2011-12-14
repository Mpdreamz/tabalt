using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tabalt.Win32;
using System.Runtime.InteropServices;
using Tabalt.Win32.Structs;
using System.IO;
using Tabalt.Extensions;

namespace Tabalt.Domain
{
	//GENERAL NOTE: looping through enumerate windows is generally faster and more reliable then calling Process.GetCurrentProcess().MainModuleWindowHanlde -- go figure

	public static class ApplicationWindows
	{

		public static IEnumerable<ApplicationRecord> ApplicationRecords
		{
			get
			{
				try
				{
					var visibleProcesses =
						from w in ApplicationWindows.AltTabable
						where !string.IsNullOrEmpty(w.Title) && !w.Title.Equals("tabalt-unique-window-name-xblah")
						let img = Shell32.LoadSmallIcon(w.ProcessName)
						let processName = ApplicationWindows.TryGetFileNameFromPath(w.ProcessName)
						where img != null && !string.IsNullOrEmpty(processName) 
						select
							new ApplicationRecord
							{
								//Process = w.Process,
								ImageSource = img,
								ProcessPath = w.ProcessName,
								ProcessName = processName,
								WindowTitle = w.Title,
								CommittedMemory = w.Process.PrivateMemorySize64.FormatBytes(),
								Window = w,

							};
					return visibleProcesses.ToList();
				}
				catch (Exception e)
				{
					return Enumerable.Empty<ApplicationRecord>();
				}
			}
		}

		private static string TryGetFileNameFromPath(string fileFullPath)
		{
			try
			{
				return Path.GetFileName(fileFullPath);
			}
			catch (Exception e)
			{
				return string.Empty;
			}

		}

		public static IEnumerable<ApplicationWindow> Systray
		{
			get
			{
				var notificationAreaHwnd = User32.GetNotificationToolbarWindowHandle();
				if (notificationAreaHwnd == IntPtr.Zero)
					yield break;
				var hash = new HashSet<IntPtr>();
				var list = new List<ApplicationWindow>();

				int count = User32.GetButtonCount(notificationAreaHwnd);

				for (int i = 0; i < count; i++)
				{
					var window = User32.GetNthApplicationWindowOnNotificationArea(notificationAreaHwnd, i);
					if (window == null)
						continue;

					yield return window;
				}
			}
			//          User32.SendMessage( hToolbar, TB.CUSTOMIZE, IntPtr.Zero, IntPtr.Zero );
		}

		public static IEnumerable<ApplicationWindow> AltTabable
		{
			get
			{
				foreach (var w in ApplicationWindows.FindAltTabableWindows())
					yield return w;
			}
		}

		public static ApplicationWindow FindWindowByCaption(string caption)
		{
			ApplicationWindow window = null;
			var ewp = new User32.EnumWindowsProc((hwnd, lparam) =>
			{
				var w = ApplicationWindows.GetApplicationWindow(hwnd, lparam);
				if (w != null && w.Title.Equals(caption, StringComparison.InvariantCulture))
					window = w;

				return (window == null); //continue as long as window == null;

			});
			User32.EnumWindows(ewp, 0);
			return window;
		}

		private static IEnumerable<ApplicationWindow> FindAltTabableWindows()
		{
			List<ApplicationWindow> windows = new List<ApplicationWindow>();
			var ewp = new User32.EnumWindowsProc((hwnd, lparam) =>
			{
				ApplicationWindow window = null;
				var shouldContinue = ApplicationWindows.Filter(hwnd, lparam, out window);
				if (window != null)
					windows.Add(window);
				return shouldContinue;

			});
			User32.EnumWindows(ewp, 0);
			return windows.DistinctBy(w => w.hWnd);
		}

		private static ApplicationWindow GetApplicationWindow(int hwnd, int lparam)
		{
			var iHwnd = new IntPtr(hwnd);
			StringBuilder title = new StringBuilder(256);
			User32.GetWindowText(hwnd, title, 256);
			var t = title.ToString();
			uint processId = 0;
			User32.GetWindowThreadProcessId(iHwnd, out processId);

			var fileName = User32.GetWindowModuleFileNameFromHandle(iHwnd);
			var window = new ApplicationWindow(t, iHwnd, (int)processId, fileName);

			return window;
		}

		private static bool Filter(int hwnd, int lparam, out ApplicationWindow window)
		{
			window = null;
			var iHwnd = new IntPtr(hwnd);


			StringBuilder title = new StringBuilder(256);
			User32.GetWindowText(hwnd, title, 256);
			var t = title.ToString();
			//Skip if the window does not have a caption.
			if (t.Length == 0)
				return true;

			//Skip if parent is not the desktop.
			var parentHwnd = User32.GetParent(iHwnd);
			if (!parentHwnd.Equals(new IntPtr(0)))
				return true;

			if (!User32.KeepWindowHandleInAltTabList(iHwnd))
				return true;

			//Skip if the window is not visible but not minimized and has SW_HIDE (0) style.
			var placement = new WINDOWPLACEMENT();
			User32.GetWindowPlacement(iHwnd, ref placement);
			if (!User32.IsWindowVisible(hwnd) && !User32.IsIconic(iHwnd) && placement.flags == 0)
				return true;

			var info = new WINDOWINFO();
			info.cbSize = (uint)Marshal.SizeOf(info);
			User32.GetWindowInfo(iHwnd, ref info);
			//skip window if it's really small (usually an indicator it's not a alt tab candidate window.
			if (info.rcWindow.Width <= 10 && info.rcWindow.Height <= 10)
				return true;
			
			window = ApplicationWindows.GetApplicationWindow(hwnd, lparam);

			return true;
		}
	}
}
