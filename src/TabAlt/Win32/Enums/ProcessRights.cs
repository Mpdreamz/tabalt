using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tabalt.Win32.Enums
{
	internal class ProcessRights
	{
		public const UInt32 TERMINATE = 0x0001;
		public const UInt32 CREATE_THREAD = 0x0002;
		public const UInt32 SET_SESSIONID = 0x0004;
		public const UInt32 VM_OPERATION = 0x0008;
		public const UInt32 VM_READ = 0x0010;
		public const UInt32 VM_WRITE = 0x0020;
		public const UInt32 DUP_HANDLE = 0x0040;
		public const UInt32 CREATE_PROCESS = 0x0080;
		public const UInt32 SET_QUOTA = 0x0100;
		public const UInt32 SET_INFORMATION = 0x0200;
		public const UInt32 QUERY_INFORMATION = 0x0400;
		public const UInt32 SUSPEND_RESUME = 0x0800;

		private const UInt32 STANDARD_RIGHTS_REQUIRED = 0x000F0000;
		private const UInt32 SYNCHRONIZE = 0x00100000;

		public const uint ALL_ACCESS = STANDARD_RIGHTS_REQUIRED | SYNCHRONIZE | 0xFFF;
	}
}
