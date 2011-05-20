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

	[STAThread]
	public static void Main()
	{
		//var k = new DisableKeys();
		_hookID = SetHook(_proc);
		//k.DisableKeyboardHook();
		Application.Run();
		UnhookWindowsHookEx(_hookID);
		//k.ReleaseKeyboardHook();

	}

	private static IntPtr SetHook(LowLevelKeyboardProc proc)
	{
		using (Process curProcess = Process.GetCurrentProcess())
		using (ProcessModule curModule = curProcess.MainModule)
		{
			return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
				GetModuleHandle(curModule.ModuleName), 0);
		}	}

	private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

	private static IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
	{
		bool forward = true;

		if ((nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN) || wParam == (IntPtr)WM_SYSKEYDOWN)
		{
			int vkCode = lParam.vkCode;
			if (vkCode == 9 && lParam.flags == 32)
			{
				return new IntPtr(1);
			}
		}
		return CallNextHookEx(_hookID, nCode, wParam, ref lParam);
	}

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UnhookWindowsHookEx(IntPtr hhk);

	[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, 	IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

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
/*
public class DisableKeys
{

	private delegate int LowLevelKeyboardProcDelegate(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam);

	[DllImport("user32.dll", EntryPoint = "SetWindowsHookExA", CharSet = CharSet.Ansi)]

	private static extern int SetWindowsHookEx(int idHook, LowLevelKeyboardProcDelegate lpfn, int hMod, int dwThreadId);

	[DllImport("user32.dll")]
	private static extern int UnhookWindowsHookEx(int hHook);

	[DllImport("user32.dll", EntryPoint = "CallNextHookEx", CharSet = CharSet.Ansi)]

	private static extern int CallNextHookEx(int hHook, int nCode, int wParam, );

	const int WH_KEYBOARD_LL = 13;

	private int intLLKey;

	
	private int LowLevelKeyboardProc(int nCode, int wParam, ref KBDLLHOOKSTRUCT lParam)
	{
		bool blnEat = false; switch (wParam)
		{
			case 256:
			case 257:
			case 260:
			case 261:
				//Alt+Tab, Alt+Esc, Ctrl+Esc, Windows Key                           
				if (((lParam.vkCode == 9) && (lParam.flags == 32)) ||
					((lParam.vkCode == 27) && (lParam.flags == 32)) ||
					((lParam.vkCode == 27) && (lParam.flags == 0)) ||
					((lParam.vkCode == 91) && (lParam.flags == 1)) ||
					((lParam.vkCode == 92) && (lParam.flags == 1)) ||
					((true) && (lParam.flags == 32)))
				{
					blnEat = true;
				}
				break;
		} if (blnEat) return 1; else return CallNextHookEx(0, nCode, wParam, ref lParam);
	}
	public void DisableKeyboardHook()
	{
		intLLKey = SetWindowsHookEx(WH_KEYBOARD_LL, new LowLevelKeyboardProcDelegate(LowLevelKeyboardProc), System.Runtime.InteropServices.Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]).ToInt32(), 0);
	}
	public void ReleaseKeyboardHook()
	{
		intLLKey = UnhookWindowsHookEx(intLLKey);
	}

}
*/
