using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace TabAlt
{
	
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
	internal class Window32API
	{
		[DllImport("user32.dll")]
		internal static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
		[DllImport("user32.dll")]
		internal static extern bool SetForegroundWindow(IntPtr hWnd);
		[DllImport("user32.dll")]
		internal static extern bool IsIconic(IntPtr hWnd);
		[DllImport("user32.dll")]
		internal static extern bool IsZoomed(IntPtr hWnd);
		[DllImport("user32.dll")]
		internal static extern IntPtr GetForegroundWindow();
		[DllImport("user32.dll")]
		internal static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		[DllImport("user32.dll")]
		internal static extern IntPtr AttachThreadInput(IntPtr idAttach, IntPtr idAttachTo, int fAttach);
		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);
		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
		internal static extern void MoveWindow(IntPtr hwnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int Left;        // x position of upper-left corner
			public int Top;         // y position of upper-left corner
			public int Right;       // x position of lower-right corner
			public int Bottom;      // y position of lower-right corner
		}
	}
	public class ShowableWindow
	{
		public IntPtr hWnd { get; private set; }
		public string Title { get; private set; }
		public Process Process { get; private set; }

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
						if (Window32API.ShowWindowAsync(this.hWnd, (int)WindowState.SW_SHOWMAXIMIZED))
							_visible = true;
					}
					else
					{
						if (Window32API.ShowWindowAsync(this.hWnd, (int)WindowState.SW_SHOWNORMAL))
							_visible = true;
					}
				}
				//hide the window

				if (value == false)
				{
					_wasMaximized = Window32API.IsZoomed(this.hWnd);
					if (Window32API.ShowWindowAsync(this.hWnd, (int)WindowState.SW_HIDE))
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

		public ShowableWindow(string Title, IntPtr hWnd, Process Process)
		{
			this.Title = Title ?? string.Empty;
			this.hWnd = hWnd;
			this.Process = Process;
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
			var hwnd = Window32API.GetForegroundWindow();
			if (this.hWnd == hwnd)
				return;
			
			Window32API.RECT lastRect;
			Window32API.RECT newRect;
			if (!Window32API.GetWindowRect(new HandleRef(null, hwnd), out lastRect)
				|| !Window32API.GetWindowRect(new HandleRef(null, this.hWnd), out newRect))
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

				Window32API.MoveWindow(this.hWnd, x, y, width, height, false);
			}
			this.Activate();

		}
		private Screen GetScreenFromRect(Window32API.RECT rect)
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


		public void Activate()
		{
			if (this.hWnd == Window32API.GetForegroundWindow())
				return;

			IntPtr ThreadID1 = Window32API.GetWindowThreadProcessId(Window32API.GetForegroundWindow(), IntPtr.Zero);
			IntPtr ThreadID2 = Window32API.GetWindowThreadProcessId(this.hWnd, IntPtr.Zero);

			if (ThreadID1 != ThreadID2)
			{
				Window32API.AttachThreadInput(ThreadID1, ThreadID2, 1);
				Window32API.SetForegroundWindow(this.hWnd);
				Window32API.AttachThreadInput(ThreadID1, ThreadID2, 0);
			}
			else
			{
				Window32API.SetForegroundWindow(this.hWnd);
			}

			if (Window32API.IsIconic(this.hWnd))
			{
				Window32API.ShowWindowAsync(this.hWnd, (int)WindowState.SW_RESTORE);
			}
			else
			{
				if (Window32API.IsZoomed(this.hWnd))
					Window32API.ShowWindowAsync(this.hWnd, (int)WindowState.SW_SHOWMAXIMIZED);
				else
					Window32API.ShowWindowAsync(this.hWnd, (int)WindowState.SW_SHOWNORMAL);
			}
		}
	}
}
