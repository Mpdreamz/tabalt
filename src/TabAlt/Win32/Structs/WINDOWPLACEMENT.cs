using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Tabalt.Win32.Structs
{
	public struct WINDOWPLACEMENT
	{
		public int length;
		public int flags;
		public int showCmd;
		public System.Drawing.Point ptMinPosition;
		public System.Drawing.Point ptMaxPosition;
		public System.Drawing.Rectangle rcNormalPosition;
	}
}
