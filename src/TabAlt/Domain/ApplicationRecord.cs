using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;
using ReactiveUI;
using Tabalt.Domain;

namespace Tabalt.Domain
{
	public interface IRecord
	{
		ImageSource ImageSource { get; set; }
		long TimesSelected { get; }
		QueryResult Match(string query);
	}

	public class ApplicationRecord : IRecord
	{
		public string CommittedMemory { get; set; }
		public string WindowTitle { get; set; }
		public string ProcessPath { get; set; }
		public string ProcessName { get; set; }
		public ImageSource ImageSource { get; set; }
		public Process Process { get; set; }
		public ApplicationWindow Window { get; set; }

		public long TimesSelected { get; private set; }
		public QueryResult Match(string query) => QueryResult.NoMatch;
	}

	public class LaunchableRecord : IRecord
	{
		public ImageSource ImageSource { get; set; }
		public string Title { get; set; }
		public string FullPath { get; set; }
		public string Location { get; set; }

		public long TimesSelected { get; private set; }
		public QueryResult Match(string query) => QueryResult.NoMatch;
	}

	public class TabaltModel : ReactiveObject
	{
		public IObservable<string> Query { get; set; }

		public IReactiveCollection<ApplicationRecord> RunningApplications { get; set; }

		public IReactiveCollection<LaunchableRecord> LaunchableTargets { get; set; }

	}

}
