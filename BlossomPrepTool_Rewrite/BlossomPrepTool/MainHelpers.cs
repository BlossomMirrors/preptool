using System;

namespace BlossomPrepTool
{
    internal static class MainHelpers
    {

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
    }
}