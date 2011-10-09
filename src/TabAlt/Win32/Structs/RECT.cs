using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace Tabalt.Win32.Structs
{
	[StructLayout(LayoutKind.Sequential)]
	public struct RECT
	{
		public int Left;        // x position of upper-left corner
		public int Top;         // y position of upper-left corner
		public int Right;       // x position of lower-right corner
		public int Bottom;      // y position of lower-right corner


		public int Height { get { return Bottom - Top; } }
		public int Width { get { return Right - Left; } }
		public Size Size { get { return new Size(Width, Height); } }

		public RECT(int left, int top, int right, int bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

	}
}
