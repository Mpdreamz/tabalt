using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace TabAlt
{
	internal static class IconUtilities
	{
		private const int SHGFI_ICON = 0x100;
		private const int SHGFI_SMALLICON = 0x1;
		private const int SHGFI_LARGEICON = 0x0;

		[DllImport("gdi32.dll", SetLastError = true)]
		private static extern bool DeleteObject(IntPtr hObject);

		[DllImport("Shell32.dll")]
		public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);
 
		public static ImageSource ToImageSource(this Icon icon)
		{
			Bitmap bitmap = icon.ToBitmap();
			IntPtr hBitmap = bitmap.GetHbitmap();

			ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());

			if (!DeleteObject(hBitmap))
				throw new Win32Exception();

			return wpfBitmap;
		}

		public static ImageSource LoadSmallIcon(string processFilePath)
		{
			SHFILEINFO shinfo = new SHFILEINFO();
			IntPtr hBitmap = SHGetFileInfo(processFilePath, 0, ref shinfo, Marshal.SizeOf(shinfo), SHGFI_ICON | SHGFI_SMALLICON);
			System.Drawing.Icon ico = System.Drawing.Icon.FromHandle(shinfo.hIcon);
			return ico.ToImageSource();
		}
	}

	internal struct SHFILEINFO
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
}
