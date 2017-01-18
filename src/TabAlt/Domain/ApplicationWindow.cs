using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Tabalt.Win32;
using Tabalt.Win32.Structs;
using Tabalt.Win32.Enums;

namespace Tabalt.Domain
{
	public class ApplicationWindow
	{
		private User32.NotificationAreaWindow NotificationAreaWindow { get; }
		private TBBUTTON Button { get; }
		public IntPtr hWnd { get; }
		public IntPtr NotificationIconHwnd { get; }
		public string Title { get; }

		private Process _process = null;
		public Process Process
		{
			get
			{
				if (this._process != null) return this._process;
				try
				{
					this._process = Process.GetProcessById(this.ProcessId);
				}
				catch { }
				return this._process;
			}
		}
		public int ProcessId { get; private set; }
		public string ProcessName { get; private set; }

		private bool _visible = true;
		private bool _wasMaximized;
		public bool Visible
		{
			get { return _visible; }
			set
			{
				//show the window

				if (value)
				{
					if (_wasMaximized)
					{
						if (User32.ShowWindowAsync(this.hWnd, (int)WindowState.SW_SHOWMAXIMIZED))
							_visible = true;
					}
					else
					{
						if (User32.ShowWindowAsync(this.hWnd, (int)WindowState.SW_SHOWNORMAL))
							_visible = true;
					}
					return;
				}
				//hide the window

				_wasMaximized = User32.IsZoomed(this.hWnd);
				if (User32.ShowWindowAsync(this.hWnd, (int)WindowState.SW_HIDE))
					_visible = false;
			}
		}

		public ApplicationWindow(string title, IntPtr hWnd, int processId, string processName)
		{
			this.Title = title ?? string.Empty;
			this.hWnd = hWnd;
			this.ProcessId = processId;
			this.ProcessName = processName;
		}

		public ApplicationWindow(int processId, string processName, User32.NotificationAreaWindow notificationArea)
			: this(notificationArea.Text, notificationArea.MainWindowHandle, processId, processName)
		{
			this.Button = notificationArea.TBBUTTON;
			this.NotificationIconHwnd = notificationArea.ToolBarIconHandle;
			this.NotificationAreaWindow = notificationArea;
		}

		public override string ToString() => this.Title ?? string.Empty;

		public void ActivateOnLastActiveScreen()
		{
			var hwnd = User32.GetForegroundWindow();
			if (this.hWnd == hwnd)
				return;

			RECT lastRect;
			RECT newRect;
			if (!User32.GetWindowRect(new HandleRef(null, hwnd), out lastRect)
				|| !User32.GetWindowRect(new HandleRef(null, this.hWnd), out newRect))
			{
				this.Activate();
				return;
			}

			var lastScreen = GetScreenFromRect(lastRect);
			var newScreen = GetScreenFromRect(newRect);
			if (!lastScreen.Bounds.Equals(newScreen.Bounds))
			{
				var x = (newRect.Left - newScreen.Bounds.X) + lastScreen.Bounds.X;
				var y = (newRect.Top - newScreen.Bounds.Y) + lastScreen.Bounds.Y;
				var width = newRect.Right - newRect.Left;
				var height = newRect.Bottom - newRect.Top;

				User32.MoveWindow(this.hWnd, x, y, width, height, false);
			}
			this.Activate();

		}
		private static Screen GetScreenFromRect(RECT rect)
		{
			var x = rect.Left;
			var y = rect.Top;
			var width = rect.Right - rect.Left + 1;
			var height = rect.Bottom - rect.Top + 1;
			return Screen.FromRectangle(new System.Drawing.Rectangle
			{
				X = x,
				Y = y,
				Height = height,
				Width = width
			});
		}

		public static IntPtr MakeLParam(int loWord, int hiWord)
		{
			return new IntPtr((hiWord << 16) | (loWord & 0xffff));
		}
		public void Activate(IntPtr hWnd)
		{

			if (hWnd == User32.GetForegroundWindow())
				return;
			//cmd needs to be called synchronous.
			if (this.ProcessName.EndsWith("cmd.exe"))
			{
				if (User32.IsIconic(hWnd))
				{
					User32.ShowWindow(hWnd, (int)WindowState.SW_RESTORE);
				}
				User32.SetForegroundWindow(hWnd);
				return;
			}

			var threadId1 = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), IntPtr.Zero);
			var threadId2 = User32.GetWindowThreadProcessId(hWnd, IntPtr.Zero);

			if (threadId1 != threadId2)
			{
				User32.AttachThreadInput(threadId1, threadId2, 1);
				User32.SetForegroundWindow(hWnd);
				User32.AttachThreadInput(threadId1, threadId2, 0);
			}
			else
			{
				User32.SetForegroundWindow(hWnd);
			}

			if (User32.IsIconic(hWnd))
			{
				User32.ShowWindowAsync(hWnd, (int)WindowState.SW_RESTORE);
			}
			else
			{
				if (User32.IsZoomed(hWnd))
					User32.ShowWindowAsync(hWnd, (int)WindowState.SW_SHOWMAXIMIZED);
				else
					User32.ShowWindowAsync(hWnd, (int)WindowState.SW_SHOWNORMAL);
			}
		}

		public void Activate()
		{
			if (this.NotificationAreaWindow != null)
			{
				//				User32.PostMessage(this.NotificationIconHwnd,0x203, new IntPtr(0x0001), MakeLParam(this.NotificationAreaWindow.RECT.Left, 10));
				User32.SendMessage(this.NotificationIconHwnd, (User32.WM.USER + 3), new IntPtr(this.Button.idCommand), IntPtr.Zero);
			}
			this.Activate(this.hWnd);
		}
		public void Close()
		{
			User32.SendMessage(this.hWnd, User32.WM.CLOSE, IntPtr.Zero, IntPtr.Zero);
		}
		public void StartNewInstance()
		{
			var p = new Process
			{
				StartInfo =
				{
					RedirectStandardOutput = false,
					FileName = this.ProcessName,
					UseShellExecute = true,
					WindowStyle = ProcessWindowStyle.Normal
				}
			};
			p.Start();
			var hwnd = p.Handle;
			this.Activate(hwnd);

		}
	}
}
