using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenQA.Selenium;

namespace Kayno.AI.Studio
{
    public partial class MainWindow : Window
    {
        #region ## WndProc 

        public enum HotKeyModifier : uint
        {
            MOD_ALT = 0x1,
            MOD_CONTROL = 0x2,
            MOD_SHIFT = 0x4,
            MOD_WIN = 0x8
        }

        public enum HotKeyMessage : uint
        {
            WM_HOTKEY = 0x0312
        }

        [DllImport( "user32.dll", SetLastError = true )]
        static extern bool RegisterHotKey(
            IntPtr hWnd,
            int id,
            uint fsModifiers,
            uint vk
        );

        [DllImport( "user32.dll", SetLastError = true )]
        static extern bool UnregisterHotKey(
            IntPtr hWnd,
            int id
        );

        private const int WM_HOTKEY = 0x0312;
        private const int HOTKEY_ID = 1;
        private IntPtr wind;
        HwndSource source;
        const int WM_LBUTTONDOWN = 0x00000201;
		const int WM_LBUTTONUP = 0x00000202;
		const int WM_RBUTTONDOWN = 0x00000204;

        public static extern IntPtr GetMessage( IntPtr lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax );

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );

            // ウィンドウハンドルを取得
            wind = new WindowInteropHelper( this ).Handle;

            // グローバルホットキーを登録 (例: Ctrl + Shift + A)
            RegisterHotKey( wind, HOTKEY_ID, (uint)( HotKeyModifier.MOD_CONTROL | HotKeyModifier.MOD_SHIFT ), (uint)KeyInterop.VirtualKeyFromKey( Key.A ) );
        }

        private void RegisterGlobalHotkey()
		{
            //source = HwndSource.FromHwnd( new WindowInteropHelper( this ).Handle );
            //source.AddHook( new HwndSourceHook( WndProc ) );
            
            wind = new WindowInteropHelper( this ).Handle ;

            // ホットキーを登録 (例: Ctrl + Shift + A)
            var mods = ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt;
			var keycode = KeyInterop.VirtualKeyFromKey( Key.F11 );
            RegisterHotKey( wind, HOTKEY_ID, (uint)mods, (uint)keycode );
		}


		private void UnregisterGlobalHotkey()
		{
            UnregisterHotKey( wind, HOTKEY_ID );
            

            HwndSource source = HwndSource.FromHwnd( new WindowInteropHelper( this ).Handle );
            if ( source != null )
            {
            	source.RemoveHook( WndProc );
                source.Dispose();
            }
        }


        private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            if ( msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID )
            {
                // ホットキーが押されたときの処理
                MessageBox.Show( "グローバルホットキー (Ctrl + Shift + A) が押されました！" );
                handled = true;
            }

            // 親ウィンドウ外でマウスクリックしてもOKなように

            try
            {
                if ( msg == WM_LBUTTONDOWN )
                {
                    Debug.WriteLine( " Capture : MouseL down Hook " );

                    if ( IsDefiningRegion )
                    {
                        handled = true;
                    }
                }

                if ( msg == WM_LBUTTONUP )
                {
                    Debug.WriteLine( " Capture : MouseL up Hook " );

                    if ( IsDefiningRegion )
                    {
                        releaseMouseEx();
                        handled = true;
                    }

                }

                if ( msg == WM_RBUTTONDOWN )
                {
                    Debug.WriteLine( " Capture : Cancel. " );

                    if ( IsDefiningRegion )
                    {
                        releaseMouseEx();
                        handled = true;
                    }

                }

            }
            catch ( ArgumentOutOfRangeException )
            {
                Console.WriteLine( "Invalid Index" );
            }
            return IntPtr.Zero;
        }


		#endregion

	}
}
