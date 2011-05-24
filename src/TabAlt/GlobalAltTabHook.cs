using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TabAlt
{
	public static class GlobalAltTabHook
	{
		private const int WH_KEYBOARD_LL = 13;
		private const int WM_KEYDOWN = 0x0100;
		private const int WM_KEYUP = 0x0101;

		private const int WM_SYSKEYDOWN = 0x0104;
		private const int WM_SYSKEYUP = 0x0105;
		private static LowLevelKeyboardProc _proc = HookCallback;
		private static IntPtr _hookID = IntPtr.Zero;

		private static Action AlternativeAltTabBehavior { get; set; }
		private static Func<bool> ShouldIgnoreAlt { get; set; }

		public static void Hook(Action callback, Func<bool> shouldIgnoreAlt)
		{
			using (Process curProcess = Process.GetCurrentProcess())
			using (ProcessModule curModule = curProcess.MainModule)
			{
				_hookID = SetWindowsHookEx(WH_KEYBOARD_LL, _proc, GetModuleHandle(curModule.ModuleName), 0);
			}
			GlobalAltTabHook.AlternativeAltTabBehavior = callback;
			GlobalAltTabHook.ShouldIgnoreAlt = shouldIgnoreAlt;
		}
		public static void UnHook()
		{
			UnhookWindowsHookEx(_hookID);
		}


		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		private static IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
		{
			bool forward = true;

			if (nCode >= 0 && (wParam == (IntPtr)WM_SYSKEYUP || wParam == (IntPtr)WM_SYSKEYDOWN))
			{
				int vkCode = lParam.vkCode;
				if (vkCode == 9 && lParam.flags == 32)
				{
					if (GlobalAltTabHook.AlternativeAltTabBehavior != null)
						GlobalAltTabHook.AlternativeAltTabBehavior();
					return new IntPtr(1);
				}
			}
			return CallNextHookEx(_hookID, nCode, wParam, ref lParam);
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool UnhookWindowsHookEx(IntPtr hhk);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		private struct KBDLLHOOKSTRUCT
		{
			public int vkCode;
			int scanCode;
			public int flags;
			int time;
			int dwExtraInfo;
		}
	}
}
