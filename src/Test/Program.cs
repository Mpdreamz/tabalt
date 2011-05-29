using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public class InterceptKeys
{
	private const int WH_KEYBOARD_LL = 13;
	private const int WM_KEYDOWN = 0x0100;
	private const int WM_SYSKEYDOWN = 0x0104; 
	private static LowLevelKeyboardProc _proc = HookCallback;
	private static IntPtr _hookID = IntPtr.Zero;

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnhookWindowsHookEx(IntPtr hhk);

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

	[STAThread]
	public static void Main()
	{
		_hookID = InterceptKeys.SetHook(_proc);
		Application.Run();
		InterceptKeys.UnhookWindowsHookEx(_hookID);
	}

	private static IntPtr SetHook(LowLevelKeyboardProc proc)
	{
		using (Process curProcess = Process.GetCurrentProcess())
		using (ProcessModule curModule = curProcess.MainModule)
			return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
	}

	private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

	private static IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
	{
		if ((nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) || wParam == (IntPtr)WM_SYSKEYDOWN)
		{
			int vkCode = lParam.vkCode;
			if (vkCode == 9 && lParam.flags == 32)
			{	//suppress default ALT+TAB 
				return new IntPtr(1);
			}
		}
		return CallNextHookEx(_hookID, nCode, wParam, ref lParam);
	}

	
}
