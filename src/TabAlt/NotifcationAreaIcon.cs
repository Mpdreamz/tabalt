using System;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Reflection;
using System.Windows.Forms;

namespace Tabalt
{
	public class NotifcationAreaIcon : IDisposable
	{
		private readonly NotifyIcon _notificationIcon;
		private readonly ContextMenu _notificationMenu;

		public NotifcationAreaIcon(EventHandler click, Action shutdown)
		{
			this._notificationMenu = this.CreateContextMenu(shutdown);
			this._notificationIcon = this.CreateIcon(click);
		}

		private ContextMenu CreateContextMenu(Action shutdown)
		{
			var exitMenuItem = new MenuItem("&Quit", (s, e) => shutdown());
			var menu = new ContextMenu();
			menu.MenuItems.Add(exitMenuItem);
			return menu;
		}

		private NotifyIcon CreateIcon(EventHandler click)
		{
			var x = Assembly.GetExecutingAssembly().Location;
			var ico = Path.Combine(Path.GetDirectoryName(x), "logo.ico");
			var icon = new NotifyIcon
			{
				Text = "tabalt - An alternative ALT TAB implementation",
				Icon = new Icon(ico),
				ContextMenu = this._notificationMenu,
				Visible = true
			};
			icon.Click += click;
			return icon;
		}

		public void Dispose()
		{
			this._notificationIcon.Dispose();
			this._notificationMenu.Dispose();
		}
	}
}
