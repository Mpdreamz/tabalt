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

namespace TabAlt
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		ObservableCollection<ProcessView> _ProcessViewCollection = new ObservableCollection<ProcessView>();
		public ObservableCollection<ProcessView> ProcessViewCollection { get { return _ProcessViewCollection; } }
		public MainWindow()
		{
			this.ListApplications();
			InitializeComponent();
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
			var visibleProcesses =

				from p in Process.GetProcesses(".")
				where !string.IsNullOrEmpty(p.MainWindowTitle)
				let img = IconUtilities.LoadSmallIcon(p.MainModule.FileName)
				select
					new ProcessView
					{
						Process = p,
						ImageSource = img,
						ProcessName = p.ProcessName,
						WindowTitle = p.MainWindowTitle,
						CommittedMemory = p.PrivateMemorySize64.ToString()

					};
			return visibleProcesses;
		}

		private void Window_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close();
			}
		}

		private void textBox1_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				this.Close();
				return;
			}
			this.lvApplications.Items.Filter = (o)=>
			{
				var p = (ProcessView)o;

				return Regex.IsMatch(p.ProcessName + " " + p.WindowTitle, @"\b"+textBox1.Text, RegexOptions.IgnoreCase);
			};

			this.lvApplications.SelectedIndex = 0;
			var sel = this.lvApplications.SelectedItem;
			if (sel != null)
			{
				var p = (ProcessView)sel;
				this.BigIcon.Source = System.Drawing.Icon.ExtractAssociatedIcon(p.Process.MainModule.FileName).ToImageSource();
				
			}
		}
	}
}
