using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Tabalt.Win32.Structs
{
	//many thanks to Paul Accisano for finding out  the nitty grtty on this struct.
	//http://stackoverflow.com/questions/5495981/how-do-i-define-the-tbbutton-struct-in-c/5518286
	//x86 and x64 compatible
	[StructLayout(LayoutKind.Sequential)]
	public struct TBBUTTON
	{
		public int iBitmap;
		public int idCommand;
		[StructLayout(LayoutKind.Explicit)]
		private struct TBBUTTON_U
		{
			[FieldOffset(0)]
			public byte fsState;
			[FieldOffset(1)]
			public byte fsStyle;
			[FieldOffset(0)]
			private IntPtr bReserved;
		}
		private TBBUTTON_U union;
		public byte fsState { get { return union.fsState; } set { union.fsState = value; } }
		public byte fsStyle { get { return union.fsStyle; } set { union.fsStyle = value; } }
		public IntPtr dwData;
		public IntPtr iString;
	}
}
