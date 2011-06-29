using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using TabAlt.Win32;
using TabAlt.Win32.Structs;

namespace TabAlt.Domain
{
	public class ShowableWindow
	{
		private User32.NotificationAreaWindow NotificationAreaWindow { get; set; }
		private TBBUTTON Button { get; set; }
		public IntPtr hWnd { get; private set; }
		public IntPtr NotificationIconHwnd { get; private set; }
		public string Title { get; private set; }
		private Process _process = null;
		public Process Process 
		{ 
			get
			{
				if (this._process == null)
				{
					try
					{
						this._process = Process.GetProcessById(this.ProcessId);
					}
					catch { }
				}
				return this._process;
			}
		}
		public int ProcessId { get; private set; }
		public string ProcessName { get; private set; }

		private bool _visible = true;
		private bool _wasMaximized = false;
		public bool Visible
		{
			get { return _visible; }
			set
			{
				//show the window

				if (value == true)
				{
					if (_wasMaximized)
					{
						if (User32.ShowWindowAsync(this.hWnd, (int)User32.WindowState.SW_SHOWMAXIMIZED))
							_visible = true;
					}
					else
					{
						if (User32.ShowWindowAsync(this.hWnd, (int)User32.WindowState.SW_SHOWNORMAL))
							_visible = true;
					}
				}
				//hide the window

				if (value == false)
				{
					_wasMaximized = User32.IsZoomed(this.hWnd);
					if (User32.ShowWindowAsync(this.hWnd, (int)User32.WindowState.SW_HIDE))
						_visible = false;
				}
			}
		}

		/// <summary>
		/// Constructs a Window Object
		/// </summary>
		/// <param name="Title">Title Caption</param>
		/// <param name="hWnd">Handle</param>
		/// <param name="Process">Owning Process</param>
		public ShowableWindow(string title, IntPtr hWnd, int processId, string processName)
		{
			this.Title = title ?? string.Empty;
			this.hWnd = hWnd;
			this.ProcessId = processId;
			this.ProcessName = processName;
		}
		public ShowableWindow(int processId, string processName, User32.NotificationAreaWindow notificationArea)
			: this(notificationArea.Text, notificationArea.MainWindowHandle, processId, processName)
		{
			this.Button = notificationArea.TBBUTTON;
			this.NotificationIconHwnd = notificationArea.ToolBarIconHandle;
			this.NotificationAreaWindow = notificationArea;
		}
		//Override ToString() 

		public override string ToString()
		{
			return this.Title ?? string.Empty;
		}

		/// <summary>

		/// Sets focus to this Window Object

		/// </summary>

		public void ActiveOnLastActiveScreen()
		{
			var hwnd = User32.GetForegroundWindow();
			if (this.hWnd == hwnd)
				return;

			User32.RECT lastRect;
			User32.RECT newRect;
			if (!User32.GetWindowRect(new HandleRef(null, hwnd), out lastRect)
				|| !User32.GetWindowRect(new HandleRef(null, this.hWnd), out newRect))
			{
				this.Activate();
				return;
			}
			
			var lastScreen = this.GetScreenFromRect(lastRect);
			var newScreen = this.GetScreenFromRect(newRect);
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
		private Screen GetScreenFromRect(User32.RECT rect)
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

		public static IntPtr MakeLParam(int LoWord, int HiWord)
		{
			return new IntPtr((HiWord << 16) | (LoWord & 0xffff));
		} 
		public void Activate()
		{
			if (this.NotificationAreaWindow != null)
			{
//				User32.PostMessage(this.NotificationIconHwnd,0x203, new IntPtr(0x0001), MakeLParam(this.NotificationAreaWindow.RECT.Left, 10));
				User32.SendMessage(this.NotificationIconHwnd, (User32.WM.USER + 3), new IntPtr(this.Button.idCommand), IntPtr.Zero);
			}
				
			else
			{
				if (this.hWnd == User32.GetForegroundWindow())
					return;

				IntPtr ThreadID1 = User32.GetWindowThreadProcessId(User32.GetForegroundWindow(), IntPtr.Zero);
				IntPtr ThreadID2 = User32.GetWindowThreadProcessId(this.hWnd, IntPtr.Zero);

				if (ThreadID1 != ThreadID2)
				{
					User32.AttachThreadInput(ThreadID1, ThreadID2, 1);
					User32.SetForegroundWindow(this.hWnd);
					User32.AttachThreadInput(ThreadID1, ThreadID2, 0);
				}
				else
				{
					User32.SetForegroundWindow(this.hWnd);
				}

				if (User32.IsIconic(this.hWnd))
				{
					User32.ShowWindowAsync(this.hWnd, (int)User32.WindowState.SW_RESTORE);
				}
				else
				{
					if (User32.IsZoomed(this.hWnd))
						User32.ShowWindowAsync(this.hWnd, (int)User32.WindowState.SW_SHOWMAXIMIZED);
					else
						User32.ShowWindowAsync(this.hWnd, (int)User32.WindowState.SW_SHOWNORMAL);
				}
			}
		}
	}
}
