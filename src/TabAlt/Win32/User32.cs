using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using TabAlt.Domain;
using System.Diagnostics;
using TabAlt.Win32.Enums;
using System.Drawing;
using TabAlt.Win32.Structs;

namespace TabAlt.Win32
{
	public static class User32
	{

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern UInt32 SendMessage(IntPtr hWnd, UInt32 msg, UInt32 wParam, UInt32 lParam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, IntPtr lParam);

		[DllImport("User32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll")]
		public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, UInt32 lParam);

		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern int GetWindowText(int hWnd, StringBuilder title, int size);

		[DllImport("user32.dll")]
		public static extern int GetWindowModuleFileName(int hWnd, StringBuilder title, int size);

		[DllImport("user32.dll")]
		public static extern int EnumWindows(EnumWindowsProc ewp, int lParam);
		public delegate bool EnumWindowsProc(int hWnd, int lParam);

		[DllImport("user32.dll", SetLastError = true)]
		static extern System.IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		static extern System.IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		public static extern IntPtr GetParent(IntPtr hWnd);

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

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

		[return: MarshalAs(UnmanagedType.Bool)]
		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);

		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, ref RECT lpPoints, UInt32 cPoints);



		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner


			public int Height { get { return Bottom - Top; } }
			public int Width { get { return Right - Left; } }
			public Size Size { get { return new Size(Width, Height); } }

			public RECT(int left, int top, int right, int bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}

		}
		public struct WINDOWINFO
		{
			public uint cbSize;
			public RECT rcWindow;
			public RECT rcClient;
			public uint dwStyle;
			public WindowStylesEx dwExStyle;
			public uint dwWindowStatus;
			public uint cxWindowBorders;
			public uint cyWindowBorders;
			public ushort atomWindowType;
			public ushort wCreatorVersion;

			public WINDOWINFO(Boolean? filler)
				: this()
			{
				cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
			}
		}
		public enum WindowState
		{
			SW_HIDE = 0,
			SW_SHOWNORMAL = 1,
			SW_SHOWMINIMIZED = 2,
			SW_SHOWMAXIMIZED = 3,
			SW_SHOWNOACTIVATE = 4,
			SW_SHOW = 5,
			SW_MINIMIZE = 6,
			SW_SHOWMINNOACTIVE = 7,
			SW_SHOWNA = 8,
			SW_RESTORE = 9,
			SW_SHOWDEFAULT = 10
		}
		public struct WINDOWPLACEMENT
		{
			public int length;
			public int flags;
			public int showCmd;
			public System.Drawing.Point ptMinPosition;
			public System.Drawing.Point ptMaxPosition;
			public System.Drawing.Rectangle rcNormalPosition;
		}
		internal class WM
		{
			public const uint CLOSE = 0x0010;
			public const uint GETICON = 0x007F;
			public const uint KEYDOWN = 0x0100;
			public const uint COMMAND = 0x0111;
			public const uint USER = 0x0400; // 0x0400 - 0x7FFF
			public const uint APP = 0x8000; // 0x8000 - 0xBFFF
		}
		internal class TB
		{
			public const uint GETBUTTON = WM.USER + 23;
			public const uint BUTTONCOUNT = WM.USER + 24;
			public const uint CUSTOMIZE = WM.USER + 27;
			public const uint GETBUTTONTEXTA = WM.USER + 45;
			public const uint GETBUTTONTEXTW = WM.USER + 75;
			public const uint WM_LBUTTONDBLCLK = 0x0203;
			public const uint TB_PRESSBUTTON = (WM.USER + 3);
		}
		private static IntPtr SystrayHwnd =
			User32.FindWindowEx(User32.FindWindow("Shell_TrayWnd", null), IntPtr.Zero, "TrayNotifyWnd", null);


		private static IntPtr GetNotificationToolbarWindowHandle()
		{
			IntPtr hShell = User32.FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Shell_TrayWnd", null);
			IntPtr hTray = User32.FindWindowEx(hShell, IntPtr.Zero, "TrayNotifyWnd", null);
			IntPtr hPager = User32.FindWindowEx(hTray, IntPtr.Zero, "SysPager", null);
			IntPtr hToolbar = User32.FindWindowEx(hPager, IntPtr.Zero, "ToolbarWindow32", null);
			return hToolbar;

		}
		
		public static IEnumerable<ShowableWindow> ListSystrayProcesses()
		{

			var pInt = User32.GetNotificationToolbarWindowHandle();
			if (pInt == IntPtr.Zero) return null;
			var hash = new HashSet<IntPtr>();
			var list = new List<ShowableWindow>();

			int count = (int)User32.SendMessage(pInt, TB.BUTTONCOUNT, IntPtr.Zero, IntPtr.Zero);

			for (int i = 0; i < count; i++)
			{
				TBBUTTON tbButton = new TBBUTTON();
				string text = String.Empty;
				IntPtr ipWindowHandle = IntPtr.Zero;

				NotificationAreaWindow notificationAreaWindow;
				bool b = GetTBButton(pInt, i, ref tbButton, ref text, ref ipWindowHandle, out notificationAreaWindow);
				if (!b)
					continue;
				if (hash.Contains(notificationAreaWindow.MainWindowHandle))
					continue;
				hash.Add(notificationAreaWindow.MainWindowHandle);	

				uint processId = 0;
				User32.GetWindowThreadProcessId(notificationAreaWindow.MainWindowHandle, out processId);
				if (processId == 0)
					continue;
				if (processId == App.ProcessId)
					continue;
				var fileName = User32.GetWindowModuleFileNameFromHandle(notificationAreaWindow.MainWindowHandle);
				if (fileName.ToLower().EndsWith("explorer.exe"))
					continue;

				var window = new ShowableWindow((int)processId, fileName,	notificationAreaWindow);
				list.Add(window);


			}
			return list;
			//          User32.SendMessage( hToolbar, TB.CUSTOMIZE, IntPtr.Zero, IntPtr.Zero );
		}
		public class NotificationAreaWindow
		{
			public RECT RECT { get; set; }
			public TBBUTTON TBBUTTON { get; set; }
			public IntPtr MainWindowHandle { get; set; }
			public IntPtr ToolBarIconHandle { get; set; }
			public string Text { get; set; }
		}

		const uint WM_USER = 1024;
		const uint TB_HIDEBUTTON = (WM_USER + 4);
		const uint TB_BUTTONCOUNT = (WM_USER + 24);
		const uint TB_GETBUTTON = (WM_USER + 23);
		const uint TB_GETITEMRECT = (WM_USER + 29);
		const uint TBSTATE_HIDDEN = 0x08;
		const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
		const uint MEM_COMMIT = 0x1000;
		const uint MEM_RELEASE = 0x8000;
		const uint PAGE_READWRITE = 0x04;

		private static RECT GetRect(IntPtr hwnd)
		{
			RECT rct;
			Rectangle myRect = new Rectangle();
			if (!GetWindowRect(new HandleRef(new object{}, hwnd), out rct))
			{
				return rct;
			}

			myRect.X = rct.Left;
			myRect.Y = rct.Top;
			myRect.Width = rct.Right - rct.Left + 1;
			myRect.Height = rct.Bottom - rct.Top + 1;

			return rct;
		}

		private static unsafe bool GetTBButton(IntPtr hToolbar, int i, ref TBBUTTON tbButton, ref string text, ref IntPtr ipWindowHandle, out NotificationAreaWindow notificationAreaWindow)
		{
			notificationAreaWindow = new NotificationAreaWindow();
			// One page
			const int BUFFER_SIZE = 0x1000;

			byte[] localBuffer = new byte[BUFFER_SIZE];

			uint processId = 0;
			UInt32 threadId = User32.GetWindowThreadProcessId(hToolbar, out processId);

			IntPtr hProcess = Kernel32.OpenProcess(ProcessRights.ALL_ACCESS, false, processId);
			if (hProcess == IntPtr.Zero) { Debug.Assert(false); return false; }

			IntPtr ipRemoteBuffer = Kernel32.VirtualAllocEx(
				hProcess,
				IntPtr.Zero,
				new UIntPtr(BUFFER_SIZE),
				MemAllocationType.COMMIT,
				MemoryProtection.PAGE_READWRITE);

			if (ipRemoteBuffer == IntPtr.Zero) { Debug.Assert(false); return false; }

			// TBButton
			fixed (TBBUTTON* pTBButton = &tbButton)
			{
				IntPtr ipTBButton = new IntPtr(pTBButton);

				int b = (int)User32.SendMessage(hToolbar, TB.GETBUTTON, (IntPtr)i, ipRemoteBuffer);
				if (b == 0) { Debug.Assert(false); return false; }

				Int32 dwBytesRead = 0;
				IntPtr ipBytesRead = new IntPtr(&dwBytesRead);

				bool b2 = Kernel32.ReadProcessMemory(
					hProcess,
					ipRemoteBuffer,
					ipTBButton,
					new UIntPtr((uint)sizeof(TBBUTTON)),
					ipBytesRead);

				if (!b2) { Debug.Assert(false); return false; }
			}

			// button text
			fixed (byte* pLocalBuffer = localBuffer)
			{
				IntPtr ipLocalBuffer = new IntPtr(pLocalBuffer);

				int chars = (int)User32.SendMessage(hToolbar, TB.GETBUTTONTEXTW, (IntPtr)tbButton.idCommand, ipRemoteBuffer);

				if (chars == -1) { Debug.Assert(false); return false; }

				Int32 dwBytesRead = 0;
				IntPtr ipBytesRead = new IntPtr(&dwBytesRead);

				bool b4 = Kernel32.ReadProcessMemory(
					hProcess,
					ipRemoteBuffer,
					ipLocalBuffer,
					new UIntPtr(BUFFER_SIZE),
					ipBytesRead);

				if (!b4) { Debug.Assert(false); return false; }

				text = Marshal.PtrToStringUni(ipLocalBuffer, chars);

				if (text == " ") text = String.Empty;
			}

			// window handle
			fixed (byte* pLocalBuffer = localBuffer)
			{
				IntPtr ipLocalBuffer = new IntPtr(pLocalBuffer);

				Int32 dwBytesRead = 0;
				IntPtr ipBytesRead = new IntPtr(&dwBytesRead);

				var ipRemoteData = tbButton.dwData;

				bool b4 = Kernel32.ReadProcessMemory(
					hProcess,
					ipRemoteData,
					ipLocalBuffer,
					new UIntPtr(4),
					ipBytesRead);

				if (!b4) { Debug.Assert(false); return false; }

				if (dwBytesRead != 4) { Debug.Assert(false); return false; }

				Int32 iWindowHandle = BitConverter.ToInt32(localBuffer, 0);
				if (iWindowHandle == -1) { Debug.Assert(false); }//return false; }

				ipWindowHandle = new IntPtr(iWindowHandle);
			}


			var rect = default(TabAlt.Win32.User32.RECT);
			uint dwTrayProcessID = 0;
			GetWindowThreadProcessId(hToolbar, out dwTrayProcessID);
			if (dwTrayProcessID <= 0) { return false; }
			IntPtr hTrayProc = Kernel32.OpenProcess(PROCESS_ALL_ACCESS, 0, dwTrayProcessID);
			if (hTrayProc == IntPtr.Zero) { return false; }
			IntPtr lpData = Kernel32.VirtualAllocEx(hTrayProc, IntPtr.Zero, Marshal.SizeOf(tbButton.GetType()), MEM_COMMIT, PAGE_READWRITE);
			if (lpData == IntPtr.Zero) { Kernel32.CloseHandle(hTrayProc); return false; }

			// Show tray icon if hidden
			//if ((tbButton.fsState & (byte)TBSTATE_HIDDEN) == (byte)TBSTATE_HIDDEN) SendMessage(hToolbar, TB_HIDEBUTTON, tbButton.idCommand, 1);
			// Get rectangle of tray icon
			Int32 dwBytesRead2 = -1;
			var rectNotifyIcon = new RECT(0, 0, 0, 0);
			byte[] byteBuffer3 = new byte[Marshal.SizeOf(rectNotifyIcon.GetType())];

			SendMessage(hToolbar, TB_GETITEMRECT, tbButton.idCommand, lpData);
			Kernel32.ReadProcessMemory(hTrayProc, lpData, byteBuffer3, Marshal.SizeOf(rectNotifyIcon.GetType()),	out dwBytesRead2);
			if (dwBytesRead2 < Marshal.SizeOf(rectNotifyIcon.GetType())) { return false; }

			IntPtr ptrOut2 = Marshal.AllocHGlobal(Marshal.SizeOf(rectNotifyIcon.GetType()));
			Marshal.Copy(byteBuffer3, 0, ptrOut2, byteBuffer3.Length);
			rectNotifyIcon = (RECT)Marshal.PtrToStructure(ptrOut2, typeof(RECT));
			//MapWindowPoints(hToolbar, IntPtr.Zero, ref rectNotifyIcon, 2);

			// Display coordinates
			var c = GetRect(hToolbar);
			System.Diagnostics.Debug.Print("{4} ICONS COORDINATES ARE: Top: {0}, Left: {1}, Bottom: {2}, Right: {3}",
			rectNotifyIcon.Top - c.Top,
			rectNotifyIcon.Left - c.Left,
			(rectNotifyIcon.Bottom - c.Bottom) + rectNotifyIcon.Height,
			rectNotifyIcon.Right - c.Right,
			text);
			Kernel32.VirtualFreeEx(hTrayProc, lpData, UIntPtr.Zero, MEM_RELEASE);
			Kernel32.CloseHandle(hTrayProc);
			rect = new RECT(
				rectNotifyIcon.Left - c.Left, 
				rectNotifyIcon.Top - c.Top, 
				rectNotifyIcon.Right - c.Right,
				(rectNotifyIcon.Bottom - c.Bottom) + rectNotifyIcon.Height);


			Kernel32.VirtualFreeEx(
				hProcess,
				ipRemoteBuffer,
				UIntPtr.Zero,
				MemAllocationType.RELEASE);

			Kernel32.CloseHandle(hProcess);
			notificationAreaWindow = new NotificationAreaWindow()
			{
				TBBUTTON = tbButton,
				MainWindowHandle = ipWindowHandle,
				ToolBarIconHandle = hToolbar,
				Text = text,
				RECT = rect
			};
			return true;
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
			Func<int, int, bool> filter = (hWnd, lParam) =>
			{
				var iHwnd = new IntPtr(hWnd);
				StringBuilder title = new StringBuilder(256);
				User32.GetWindowText(hWnd, title, 256);
				var fileName = User32.GetWindowModuleFileNameFromHandle(iHwnd);
				if (title.Length == 0)
					return true;

				var parentHwnd = User32.GetParent(iHwnd);

				if (!parentHwnd.Equals(new IntPtr(0)))
					return true;

				if (!User32.IsWindowVisible(hWnd) && !matchProcessId.HasValue)
				{
					var placement = new WINDOWPLACEMENT();
					User32.GetWindowPlacement(iHwnd, ref placement);
					WINDOWINFO info = new WINDOWINFO();
					info.cbSize = (uint)Marshal.SizeOf(info);
					User32.GetWindowInfo(iHwnd, ref info);
					if (!User32.IsIconic(iHwnd)
						&& placement.flags == 0)
					{
						return true;
					}
				}
				uint processId = 0;
				User32.GetWindowThreadProcessId(iHwnd, out processId);
				var window = new ShowableWindow(title.ToString(), iHwnd, (int)processId, fileName);
				windows.Add(window);
				if (matchProcessId.HasValue && processId == matchProcessId.Value)
					return false;
				return true;
			};
			var ewp = new User32.EnumWindowsProc(filter);
			foreach (var window in User32.ListSystrayProcesses())
			{
			;
				windows.Add(window);
			}
			
			User32.EnumWindows(ewp, 0);
			
			return windows.DistinctBy(w=>w.hWnd).DistinctBy(w=>w.ProcessId);
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
		public static IEnumerable<TSource> DistinctBy<TSource, TKey>
	 (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			HashSet<TKey> knownKeys = new HashSet<TKey>();
			foreach (TSource element in source)
			{
				if (knownKeys.Add(keySelector(element)))
				{
					yield return element;
				}
			}
		}

	}
}
