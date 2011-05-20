using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

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
