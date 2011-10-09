using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Tabalt.Win32
{
	public static class PSAPI
	{
		[DllImport("psapi.dll")]
		public static extern uint GetModuleFileNameEx(
			IntPtr hProcess, IntPtr hModule,
			[Out] StringBuilder lpBaseName,
			[In] [MarshalAs(UnmanagedType.U4)] int nSize
		);
	}

}
