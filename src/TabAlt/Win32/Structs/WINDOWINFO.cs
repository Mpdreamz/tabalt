using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Tabalt.Win32.Structs
{
	public struct WINDOWINFO
	{
		public uint cbSize;
		public RECT rcWindow;
		public RECT rcClient;
		public uint dwStyle;
		public WindowStylesEx dwExStyle;
		public uint dwWindowStatus;
		public uint cxWindowBorders;
		public uint cyWindowBorders;
		public ushort atomWindowType;
		public ushort wCreatorVersion;

		public WINDOWINFO(Boolean? filler)
			: this()
		{
			cbSize = (UInt32)(Marshal.SizeOf(typeof(WINDOWINFO)));
		}
	}
}
