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
using Tabalt.Win32;
using Tabalt.Domain;
using System.Windows.Controls.Primitives;

namespace Tabalt
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private bool DeActivatedWhileSwitching { get; set; }
		private System.Windows.Forms.NotifyIcon _notificationIcon;
		private System.Windows.Forms.ContextMenu _notificationMenu;


		public MainWindow()
		{
			TabaltHooks.AltTabHook();
			TabaltHooks.AltTabPressed += this.OnAltTabPressed;
			TabaltHooks.ActivationRequested += this.OnActivationRequested;

			InitializeComponent();


			this._notificationMenu = new System.Windows.Forms.ContextMenu();
			var exitMenuItem = new System.Windows.Forms.MenuItem("&Quit", (s, e)=> Application.Current.Shutdown());
			this._notificationMenu.MenuItems.Add(exitMenuItem);

			this._notificationIcon = new System.Windows.Forms.NotifyIcon();
			this._notificationIcon.Text = "tabalt - An alternative ALT TAB implementation";
			this._notificationIcon.Icon = new System.Drawing.Icon("logo.ico");
			this._notificationIcon.Click += new EventHandler(_notificationIcon_Click);
			this._notificationIcon.ContextMenu = this._notificationMenu;
			this._notificationIcon.Visible = true;

		}

		public void ListApplications()
		{
			if (this.lvApplications != null)
			{ 
				this.lvApplications.ItemsSource = ApplicationWindows.ApplicationRecords;
				this.lvApplications.Width = this.lvApplications.Width + 1;
				this.lvApplications.Width = this.lvApplications.Width - 1;
			}
		}


		private void OnAltTabPressed()
		{
			this.ListApplications();
		}
		private void OnActivationRequested()
		{
			this.BringToFront();
		}

		private void BringToFront()
		{
			var window = ApplicationWindows.FindWindowByCaption("tabalt-unique-window-name-xblah");
			if (window != null)
				window.ActivateOnLastActiveScreen();
			this.Show();
			this.Activate();
			this.txtFilter.Clear();
			this.txtFilter.Focus();
			this.FocusInput();
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			this.IsSpecialKeyHandled(e, true);
		}

		private void txtFilter_KeyUp(object sender, KeyEventArgs e)
		{
			if (this.IsSpecialKeyHandled(e))
				return;

			if ((e.Key == Key.Up || e.Key == Key.Down) && lvApplications.Items.Count > 0)
			{
				this.FocusListView(() =>
				{
					if (e.Key == Key.Down)
					{
						lvApplications.SelectedIndex = Math.Min(lvApplications.Items.Count - 1, lvApplications.SelectedIndex + 1);
					}
					if (e.Key == Key.Up)
					{
						lvApplications.SelectedIndex = Math.Max(0, lvApplications.SelectedIndex - 1);
					}
					lvApplications.SelectedItem = lvApplications.Items[lvApplications.SelectedIndex];
					if (lvApplications.SelectedItem != null)
						((ListViewItem)lvApplications.ItemContainerGenerator.ContainerFromItem(lvApplications.SelectedItem)).Focus();
				});
				return;
			}
			this.lvApplications.Items.Filter = (o) =>
			{
				var p = (ApplicationRecord)o;
				var text = string.Concat(p.ProcessName," ",p.WindowTitle," ",p.ProcessName);		
				var regex = @"\b" + string.Join(".*?",txtFilter.Text.Split(new []{' '}).Select(t=> Regex.Escape(t)));
				return Regex.IsMatch(text, regex, RegexOptions.IgnoreCase);
			};


			StringBuilder sb = new StringBuilder();

			this.lvApplications.SelectedIndex = (this.lvApplications.Items.Count > 1 && string.IsNullOrEmpty(this.txtFilter.Text)) ? 1 : 0;
		
			this.UpdateBigIconFromSelection();
		}

		private void UpdateBigIconFromSelection()
		{
			var sel = this.lvApplications.SelectedItem;
			if (sel != null)
			{
				var p = (ApplicationRecord)sel;
				try
				{
					this.BigIcon.Source = System.Drawing.Icon.ExtractAssociatedIcon(p.ProcessPath).ToImageSource();
				}
				catch
				{

				}
			}
			else
			{ 
				var	uriSource = new Uri(@"/Tabalt;component/logo.png", UriKind.Relative);
				this.BigIcon.Source = new BitmapImage(uriSource);
			}
		}



		private void lvApplications_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keyboard.Modifiers != ModifierKeys.None)
				return;

			if (e.Key != Key.Up && e.Key != Key.Down)
			{
				this.FocusInput();
			}
		}

		private void lvApplications_KeyUp(object sender, KeyEventArgs e)
		{
			this.IsSpecialKeyHandled(e);
			this.UpdateBigIconFromSelection();
		}

		private bool IsSpecialKeyHandled(KeyEventArgs e, bool keyUp = false)
		{

			if (keyUp && e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.Control)
			{
				if (this.lvApplications.SelectedItems == null)
					return true;
				foreach (var item in this.lvApplications.SelectedItems)
				{
					((ListViewItem)lvApplications.ItemContainerGenerator.ContainerFromItem(item)).Visibility = System.Windows.Visibility.Hidden;
					((ApplicationRecord)item).Window.Close();
				}
				this.ListApplications();
				this.lvApplications.SelectedIndex = 0;
				return true;
			}

			if (e.Key == Key.Escape)
			{
				Hide();
				return true;
			}
			if (e.Key == Key.Enter && keyUp)
			{
				var val = this.lvApplications.SelectedValue;
				if (val != null)
				{
					DeActivatedWhileSwitching = true;
					if (Keyboard.Modifiers == ModifierKeys.Control)
						((ApplicationRecord)val).Window.StartNewInstance();
					else
						((ApplicationRecord)val).Window.Activate();
				}
				Hide();
				DeActivatedWhileSwitching = false;
				return true;
			}
			return false;
		}

		void OnClose(object sender, CancelEventArgs args)
		{
			User32KeyboardHook.UnHook();
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
			FocusInput();
		}
		void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			if ((bool)args.NewValue)
			{
				this.Activate();
				FocusInput();
			}
		}
		void FocusInput()
		{
			this.Dispatcher.BeginInvoke((Action)delegate
				{
					this.txtFilter.Focus();
					this.txtFilter.ReleaseMouseCapture();
					if (this.txtFilter.Text.Length == 0)
						this.txtFilter.Select(Math.Max((this.txtFilter.Text ?? string.Empty).Length - 1, 0), 0);
					Keyboard.Focus(this.txtFilter);
				}, DispatcherPriority.Render);
		}
		void FocusListView(Action afterFocus)
		{
			this.Dispatcher.BeginInvoke((Action)delegate
			{
				this.lvApplications.Focus();
				Keyboard.Focus(this.lvApplications);
				if (afterFocus != null)
					afterFocus();
			}, DispatcherPriority.Render);
		}


		private void _notificationIcon_Click(object sender, EventArgs e)
		{
			this.ListApplications();
			this.BringToFront();
		}

		private void Window_Activated(object sender, EventArgs e)
		{
			this.FocusInput();
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			if (!this.DeActivatedWhileSwitching)
				this.Hide();

			this.DeActivatedWhileSwitching = false;
		}
	}
}
