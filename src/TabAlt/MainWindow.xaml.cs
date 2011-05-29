using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Drawing;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Security;
using System.Runtime.InteropServices;
using System.Management;
using System.ComponentModel;
using System.Windows.Threading;
using System.Threading;

namespace TabAlt
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool DeActivatedWhileSwitching { get; set; }
		private System.Windows.Forms.NotifyIcon _notificationIcon;

		ObservableCollection<ProcessView> _ProcessViewCollection = new ObservableCollection<ProcessView>();
		public ObservableCollection<ProcessView> ProcessViewCollection { get { return _ProcessViewCollection; } }
		public MainWindow()
		{
			GlobalAltTabHook.Hook(() =>
			{
				WindowsEnumeration.ActivateWindow("tabalt");
				this.Show();
				this.Activate();
			});
			this.ListApplications();
			InitializeComponent();
			this.txtFilter.Focus();

			this._notificationIcon = new System.Windows.Forms.NotifyIcon();
			this._notificationIcon.Text = "tabalt - An alternative ALT TAB implementation";
			this._notificationIcon.Icon = new System.Drawing.Icon("logo.ico");
			this._notificationIcon.Click += new EventHandler(_notificationIcon_Click);
			this._notificationIcon.Visible = true;

		}

		public void ListApplications()
		{
			var processes = this.FilterApplication("");
			foreach (var p in processes)
			{
				this._ProcessViewCollection.Add(p);
			}
		}
	
		public IEnumerable<ProcessView> FilterApplication(string startsWith)
		{
			var windows =  WindowsEnumeration.GetShowableWindows();
			
			var visibleProcesses = from w in windows
				where !string.IsNullOrEmpty(w.Title) && w.Process != null
				let img = IconUtilities.LoadSmallIcon(w.Process.MainModule.FileName)
				select
					new ProcessView
					{
						Process = w.Process,
						ImageSource = img,
						ProcessName = w.Process.ProcessName,
						WindowTitle = w.Title,
						CommittedMemory = w.Process.PrivateMemorySize64.ToString(),
						Window = w,

					};
			return visibleProcesses;
		}
		[SuppressUnmanagedCodeSecurity]
		internal static class UnsafeNativeMethods
		{
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			internal static extern int GetWindowText(IntPtr hWnd, [Out] StringBuilder lpString, int nMaxCount);
			[DllImport("user32.dll", SetLastError = true)]
			internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
			[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
			internal static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

		}
		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			this.KeyDown(e);	
		}

		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			if (this.KeyDown(e))
				return;

			if (e.Key == Key.Up
			|| e.Key == Key.Down)
			{
				KeyEventArgs e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, e.Key);
				e1.RoutedEvent = Keyboard.KeyDownEvent;
				this.lvApplications.RaiseEvent(e1);
				return;
			}
			this.lvApplications.Items.Filter = (o)=>
			{
				var p = (ProcessView)o;

				return Regex.IsMatch(p.ProcessName + " " + p.WindowTitle, @"\b"+txtFilter.Text, RegexOptions.IgnoreCase);
			};


			StringBuilder sb = new StringBuilder();

			
			this.lvApplications.SelectedIndex = 0;
			var sel = this.lvApplications.SelectedItem;
			if (sel != null)
			{
				var p = (ProcessView)sel;
				this.BigIcon.Source = System.Drawing.Icon.ExtractAssociatedIcon(p.Process.MainModule.FileName).ToImageSource();
				
			}
		}

		private void lvApplications_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Up
			&& e.Key != Key.Down)
			{
				//this.FocusInput();
				KeyEventArgs e1 = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, e.Key);
				e1.RoutedEvent = Keyboard.KeyDownEvent;
				this.txtFilter.RaiseEvent(e1);
				return;
			}
		}

		private void lvApplications_KeyUp(object sender, KeyEventArgs e)
		{
			this.KeyDown(e);
		}

		private bool KeyDown(KeyEventArgs e)
		{
			
			if (e.Key == Key.Escape)
			{
				Hide();
				return true;
			}
			if (e.Key == Key.Enter)
			{
				var val = this.lvApplications.SelectedValue;
				if (val != null)
				{
					DeActivatedWhileSwitching = true;
					((ProcessView)val).Window.Activate();
				}
				Hide();
				DeActivatedWhileSwitching = false;
				return true;
			}
			return false;
		}

		void OnClose(object sender, CancelEventArgs args)
		{
			GlobalAltTabHook.UnHook();
			this._notificationIcon.Visible = false;
			this._notificationIcon.Dispose();
			this._notificationIcon = null;
		}

		private System.Windows.WindowState m_storedWindowState = System.Windows.WindowState.Normal;
		void OnStateChanged(object sender, EventArgs args)
		{
			if (WindowState == System.Windows.WindowState.Minimized)
			{
				Hide();
			}
			else
				m_storedWindowState = WindowState;
			FocusInputAndActivateWindow();
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			if ((bool)args.NewValue)
			{
				this.Activate();
				FocusInputAndActivateWindow();
			}
		}
		void FocusInputAndActivateWindow()
		{
			this.Dispatcher.BeginInvoke((Action)delegate
				{
					//var b = this.Activate();
					//while (!this.IsActive) this.Activate();				
					
					this.txtFilter.Focus();
					this.txtFilter.SelectAll();
					this.txtFilter.ReleaseMouseCapture();

					Keyboard.Focus(this.txtFilter);
				}, DispatcherPriority.Render);
		}
		void FocusInput()
		{
			this.Dispatcher.BeginInvoke((Action)delegate
			{
				this.txtFilter.Focus();
				Keyboard.Focus(this.txtFilter);
			}, DispatcherPriority.Render);
		}


		private void _notificationIcon_Click(object sender, EventArgs e)
		{
			this.ShowActivated = true;
			this.Show();
			WindowState = m_storedWindowState;
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			this.FocusInputAndActivateWindow();
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			if (!this.DeActivatedWhileSwitching)
				this.Hide();

			this.DeActivatedWhileSwitching = false;
		}
	}
}
