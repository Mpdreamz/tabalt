﻿using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Windows.Threading;
using ReactiveUI;
using Tabalt.Win32;
using Tabalt.Domain;

namespace Tabalt
{
	public partial class MainWindow : Window, IViewFor<TabaltModel>
	{
		object IViewFor.ViewModel
		{
			get { return ViewModel; }
			set { ViewModel = (TabaltModel)value; }
		}

		public TabaltModel ViewModel { get; set; }
		private NotifcationAreaIcon NotifcationAreaIcon { get; }

		private bool DeActivatedWhileSwitching { get; set; }

		public MainWindow()
		{
			TabaltHooks.AltTabPressed += this.OnAltTabPressed;
			TabaltHooks.ActivationRequested += this.OnActivationRequested;

			InitializeComponent();
			this.NotifcationAreaIcon = new NotifcationAreaIcon(_notificationIcon_Click, Application.Current.Shutdown);
		}

		public void ListApplications()
		{
			if (this.lvApplications == null) return;
			this.lvApplications.ItemsSource = ApplicationWindows.ApplicationRecords;
			this.lvApplications.Width = this.lvApplications.Width + 1;
			this.lvApplications.Width = this.lvApplications.Width - 1;
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
			window?.ActivateOnLastActiveScreen();
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
			if (this.IsSpecialKeyHandled(e, true))
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
						((ListViewItem) lvApplications.ItemContainerGenerator.ContainerFromItem(lvApplications.SelectedItem)).Focus();
				});
				return;
			}
			this.lvApplications.Items.Filter = (o) =>
			{
				var p = (ApplicationRecord) o;
				var text = string.Concat(p.ProcessName.Replace(".exe", ""), " ", p.WindowTitle);
				var regex = @"\b" + string.Join(".*?", txtFilter.Text.Split(new[] {' '}).Select(Regex.Escape));
				return Regex.IsMatch(text, regex, RegexOptions.IgnoreCase);
			};

			var sb = new StringBuilder();
			if (this.lvApplications.SelectedIndex < 0)
			{
				if (this.txtFilter.Text.Length > 0)
					this.lvApplications.SelectedIndex = 0;
				else
					this.lvApplications.SelectedIndex = (this.lvApplications.Items.Count > 1) ? 1 : 0;
			}

			this.UpdateBigIconFromSelection();
		}

		private void UpdateBigIconFromSelection()
		{
			var sel = this.lvApplications.SelectedItem;
			if (sel != null)
			{
				var p = (ApplicationRecord) sel;
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
				var uriSource = new Uri(@"/Tabalt;component/logo.png", UriKind.Relative);
				this.BigIcon.Source = new BitmapImage(uriSource);
			}
		}


		private void lvApplications_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.FocusInput();
		}
		private void lvApplications_KeyDown(object sender, KeyEventArgs e)
		{
			if (Keyboard.Modifiers == ModifierKeys.None)
			{
				this.IsSpecialKeyHandled(e, false);
				return;
			}
			this.FocusInput();
		}

		private void lvApplications_KeyUp(object sender, KeyEventArgs e)
		{
			this.IsSpecialKeyHandled(e, true);
			this.UpdateBigIconFromSelection();
		}

		private bool IsSpecialKeyHandled(KeyEventArgs e, bool keyUp = false)
		{
			if (keyUp && e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.Control)
				return KillSelectedApplications();

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
						((ApplicationRecord) val).Window.StartNewInstance();
					else
						((ApplicationRecord) val).Window.Activate();
				}
				Hide();
				DeActivatedWhileSwitching = false;
				return true;
			}
			return false;
		}

		private bool KillSelectedApplications()
		{
			if (this.lvApplications.SelectedItems == null)
				return true;
			foreach (var item in this.lvApplications.SelectedItems)
			{
				var listviewItem = ((ListViewItem) lvApplications.ItemContainerGenerator.ContainerFromItem(item));
				if (listviewItem == null)
					continue;
				listviewItem.Visibility = System.Windows.Visibility.Hidden;
				((ApplicationRecord) item).Window.Close();
			}
			this.ListApplications();
			this.lvApplications.SelectedIndex = 0;
			return true;
		}

		private void OnClose(object sender, CancelEventArgs args)
		{
			User32KeyboardHook.UnHook();
			this.NotifcationAreaIcon.Dispose();
		}

		private WindowState _storedWindowState = WindowState.Normal;

		private void OnStateChanged(object sender, EventArgs args)
		{
			if (WindowState == System.Windows.WindowState.Minimized)
				Hide();
			else
				_storedWindowState = WindowState;
			FocusInput();
		}

		private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
		{
			if (!(bool) args.NewValue) return;
			this.Activate();
			FocusInput();
		}

		private void FocusInput()
		{
			this.Dispatcher.BeginInvoke((Action) delegate
			{
				this.txtFilter.Focus();
				this.txtFilter.ReleaseMouseCapture();
				if (this.txtFilter.Text.Length == 0)
					this.txtFilter.Select(Math.Max((this.txtFilter.Text ?? string.Empty).Length - 1, 0), 0);
				Keyboard.Focus(this.txtFilter);
			}, DispatcherPriority.Render);
		}

		private void FocusListView(Action afterFocus)
		{
			this.Dispatcher.BeginInvoke((Action) delegate
			{
				this.lvApplications.Focus();
				Keyboard.Focus(this.lvApplications);
				afterFocus?.Invoke();
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
