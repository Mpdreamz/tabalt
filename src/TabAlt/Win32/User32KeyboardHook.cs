using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Tabalt.Win32
{
	public static class User32KeyboardHook
	{
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;

		private const int WM_SYSKEYDOWN = 0x0104;
		private const int WM_SYSKEYUP = 0x0105;


		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
		private static LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;
		private static Action AlternativeAltTabBehavior { get; set; }

		private struct KBDLLHOOKSTRUCT
		{
			public int vkCode;
			int scanCode;
			public int flags;
			int time;
			int dwExtraInfo;
		}

		public static void Hook(Action callback)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				_hookID = User32KeyboardHook.SetWindowsHookEx(WH_KEYBOARD_LL, _proc, Kernel32.GetModuleHandle(curModule.ModuleName), 0);
			}
			User32KeyboardHook.AlternativeAltTabBehavior = callback;
		}

		public static void UnHook()
		{
			User32KeyboardHook.UnhookWindowsHookEx(_hookID);
		}

		private static IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
		{
			if (nCode >= 0 && (wParam == (IntPtr)WM_SYSKEYUP || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				int vkCode = lParam.vkCode;
				if (vkCode == 9 && lParam.flags == 32)
				{
					if (User32KeyboardHook.AlternativeAltTabBehavior != null)
						User32KeyboardHook.AlternativeAltTabBehavior();
					return new IntPtr(1);
				}
			}
			return CallNextHookEx(_hookID, nCode, wParam, ref lParam);
		}
	}

}
