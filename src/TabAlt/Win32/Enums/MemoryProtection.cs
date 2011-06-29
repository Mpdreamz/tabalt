using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TabAlt.Win32.Enums
{
	internal class MemoryProtection
	  {
		  public const UInt32 PAGE_NOACCESS          =  0x01;
		  public const UInt32 PAGE_READONLY          =  0x02;
		  public const UInt32 PAGE_READWRITE         =  0x04;
		  public const UInt32 PAGE_WRITECOPY         =  0x08;
		  public const UInt32 PAGE_EXECUTE           =  0x10;
		  public const UInt32 PAGE_EXECUTE_READ      =  0x20;
		  public const UInt32 PAGE_EXECUTE_READWRITE =  0x40;
		  public const UInt32 PAGE_EXECUTE_WRITECOPY =  0x80;
		  public const UInt32 PAGE_GUARD             = 0x100;
		  public const UInt32 PAGE_NOCACHE           = 0x200;
		  public const UInt32 PAGE_WRITECOMBINE      = 0x400;
	  }
}
