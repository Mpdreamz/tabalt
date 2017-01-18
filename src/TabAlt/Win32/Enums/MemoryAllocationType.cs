using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tabalt.Win32.Enums
{
	internal class MemAllocationType
	{
		public const uint COMMIT = 0x1000;
		public const uint RESERVE = 0x2000;
		public const uint DECOMMIT = 0x4000;
		public const uint RELEASE = 0x8000;
		public const uint FREE = 0x10000;
		public const uint PRIVATE = 0x20000;
		public const uint MAPPED = 0x40000;
		public const uint RESET = 0x80000;
		public const uint TOP_DOWN = 0x100000;
		public const uint WRITE_WATCH = 0x200000;
		public const uint PHYSICAL = 0x400000;
		public const uint LARGE_PAGES = 0x20000000;
		public const uint FOURMB_PAGES = 0x80000000;
	}
}
