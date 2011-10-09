using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Tabalt.Win32
{
	public static class Shell32
	{
		public const int SHGFI_ICON = 0x100;
		public const int SHGFI_SMALLICON = 0x1;
		public const int SHGFI_LARGEICON = 0x0;

		[DllImport("Shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref Shell32.SHFILEINFO psfi, int cbFileInfo, uint uFlags);

		public struct SHFILEINFO
		{
			// Handle to the icon representing the file
			public IntPtr hIcon;
			// Index of the icon within the image list
			public int iIcon;
			// Various attributes of the file
			public uint dwAttributes;
			// Path to the file
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string szDisplayName;
			// File type
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public string szTypeName;
		};
		public static ImageSource LoadSmallIcon(string processFilePath)
		{
			var shinfo = new Shell32.SHFILEINFO();

			IntPtr hBitmap = Shell32.SHGetFileInfo(processFilePath, 0, ref shinfo, Marshal.SizeOf(shinfo), Shell32.SHGFI_ICON | Shell32.SHGFI_SMALLICON);
			if (hBitmap == (IntPtr)0)
				return null;

			System.Drawing.Icon ico = System.Drawing.Icon.FromHandle(shinfo.hIcon);
			var source = ico.ToImageSource();
			return source;
		}
	}
}
