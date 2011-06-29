using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TabAlt.Win32.Enums
{
	internal class MemAllocationType
	{
		public const UInt32 COMMIT = 0x1000;
		public const UInt32 RESERVE = 0x2000;
		public const UInt32 DECOMMIT = 0x4000;
		public const UInt32 RELEASE = 0x8000;
		public const UInt32 FREE = 0x10000;
		public const UInt32 PRIVATE = 0x20000;
		public const UInt32 MAPPED = 0x40000;
		public const UInt32 RESET = 0x80000;
		public const UInt32 TOP_DOWN = 0x100000;
		public const UInt32 WRITE_WATCH = 0x200000;
		public const UInt32 PHYSICAL = 0x400000;
		public const UInt32 LARGE_PAGES = 0x20000000;
		public const UInt32 FOURMB_PAGES = 0x80000000;
	}
}
