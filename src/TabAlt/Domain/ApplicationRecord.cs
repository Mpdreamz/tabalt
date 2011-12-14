using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Diagnostics;
using Tabalt.Domain;

namespace Tabalt.Domain
{
	public class ApplicationRecord
	{
		public string CommittedMemory { get; set; }
		public string WindowTitle { get; set; }
		public string ProcessPath { get; set; }
		public string ProcessName { get; set; }
		public ImageSource ImageSource { get; set; }
		public Process Process { get; set; }
		public ApplicationWindow Window { get; set; }
	}
}
