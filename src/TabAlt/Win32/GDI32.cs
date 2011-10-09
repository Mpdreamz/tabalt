using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace Tabalt.Win32
{
	public static class GDI32
	{
		[DllImport("gdi32.dll", SetLastError = true)]
		public static extern bool DeleteObject(IntPtr hObject);

		public static ImageSource ToImageSource(this Icon icon)
		{
			Bitmap bitmap = icon.ToBitmap();
			IntPtr hBitmap = bitmap.GetHbitmap();

			ImageSource wpfBitmap = Imaging.CreateBitmapSourceFromHBitmap(
				hBitmap,
				IntPtr.Zero,
				Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());

			if (!GDI32.DeleteObject(hBitmap))
				throw new Win32Exception();

			return wpfBitmap;
		}
	}
}
