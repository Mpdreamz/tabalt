using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TabAlt.Domain;
using System.Diagnostics;

namespace TabAlt.Win32
{
	public static class User32
	{

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(int hWnd, StringBuilder title, int size);
		
		[DllImport("user32.dll")]
		public static extern int GetWindowModuleFileName(int hWnd, StringBuilder title, int size);
		
		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
		public delegate bool EnumWindowsProc(int hWnd, int lParam);

		[DllImport("user32.dll")]
		public static extern bool IsWindowVisible(int hWnd);
		
		[DllImport("user32.dll")]
		public static extern IntPtr GetForegroundWindow();

		[DllImport("user32.dll")]
		public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		
		[DllImport("user32.dll")]
		public static extern bool SetForegroundWindow(IntPtr hWnd);
		
		[DllImport("user32.dll")]
		public static extern bool IsIconic(IntPtr hWnd);
		
		[DllImport("user32.dll")]
		public static extern bool IsZoomed(IntPtr hWnd);
		
	
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		
		[DllImport("user32.dll")]
		public static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
		
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
		
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
		public static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}
		public enum WindowState
		{
			SW_HIDE = 0,
			SW_SHOWNORMAL = 1,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWMAXIMIZED = 3,
			SW_SHOWNOACTIVATE = 4,
			SW_RESTORE = 9,
			SW_SHOWDEFAULT = 10
		}
		public static string GetWindowModuleFileNameFromHandle(IntPtr hWnd)
		{
			var r = string.Empty;
			uint processId = 0;
			const int nChars = 1024;
			StringBuilder filename = new StringBuilder(nChars);
			User32.GetWindowThreadProcessId(hWnd, out processId);
			IntPtr hProcess = Kernel32.OpenProcess(1040, 0, processId);
			PSAPI.GetModuleFileNameEx(hProcess, IntPtr.Zero, filename, nChars);
			Kernel32.CloseHandle(hProcess);
			r = filename.ToString();
			if (r.StartsWith("?"))
				r = "C" + r.Substring(1, r.Length - 1);
			return r;
		}
		public static IEnumerable<ShowableWindow> EnumerateWindows()
		{
			return User32.EnumerateWindows(null);
		}
		public static IEnumerable<ShowableWindow> EnumerateWindows(int? matchProcessId)
		{
			var windows = new List<ShowableWindow>();
			var ewp = new User32.EnumWindowsProc((hWnd, lParam) =>
			{
				if (!User32.IsWindowVisible(hWnd) && !matchProcessId.HasValue)
					return true;

				StringBuilder title = new StringBuilder(256);
				User32.GetWindowText(hWnd, title, 256);
				var fileName = User32.GetWindowModuleFileNameFromHandle(new IntPtr(hWnd));
				if (title.Length == 0)
					return true;

				uint processId = 0;
				User32.GetWindowThreadProcessId(new IntPtr(hWnd), out processId);
				var window = new ShowableWindow(title.ToString(), (IntPtr)hWnd, (int)processId, fileName);
				windows.Add(window);
				if (matchProcessId.HasValue && processId == matchProcessId.Value)
					return false;
				return true;
			});
			User32.EnumWindows(ewp, 0);
			return windows;
		}
		public static IEnumerable<ShowableWindow> GetShowableWindows()
		{
			var windows = User32.EnumerateWindows();
			return windows;
		}

		public static void ActivateWindow(string name)
		{
			//looping through enumerate windows is faster and more reliable then calling Process.GetCurrentProcess().MainModuleWindowHanlde
			//go figure
			var window = User32.EnumerateWindows(App.ProcessId).FirstOrDefault();
			if (window != null)
				window.ActiveOnLastActiveScreen();
		}


	}
}
