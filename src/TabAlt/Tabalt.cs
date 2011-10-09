using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tabalt.Win32;

namespace Tabalt
{
	public static class TabaltHooks
	{
		public static event Action AltTabPressed = delegate {};

		public static event Action ActivationRequested = delegate { };


		static TabaltHooks()
		{
			
		}
		public static void AltTabHook()
		{
			User32KeyboardHook.Hook(() =>
			{
				AltTabPressed();
				ActivationRequested();
			});
		}
	}
}
