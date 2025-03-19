using Kayno.AI.Studio.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Kayno.AI.Studio
{
    public partial class MainWindow : Window
    {

		/*
		 * Source:
		 * https://mseeeen.msen.jp/recover-window-bounds-with-wpf/
		 * 
		 */

		protected override void OnInitialized(EventArgs e)
		{
			RestoreWindowBounds();
			base.OnInitialized(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			// ウィンドウのサイズを保存
			SaveWindowBounds();

			base.OnClosing(e);
		}

		/// <summary>
		/// ウィンドウの位置・サイズを保存します。
		/// </summary>
		void SaveWindowBounds()
		{
			var settings = Settings.Default;
			settings.WindowMaximized = WindowState ==  WindowState.Maximized;
			WindowState = WindowState.Normal; // 最大化解除
			settings.WindowLeft = Left;
			settings.WindowTop = Top;
			settings.WindowWidth = Width;
			settings.WindowHeight = Height;
			settings.Save();
		}

		/// <summary>
		/// ウィンドウの位置・サイズを復元します。
		/// </summary>
		void RestoreWindowBounds()
		{
			var settings = Settings.Default;
			// 左
			if (settings.WindowLeft >= 0 &&
				(settings.WindowLeft + settings.WindowWidth) < SystemParameters.VirtualScreenWidth)
			{ Left = settings.WindowLeft; }
			// 上
			if (settings.WindowTop >= 0 &&
				(settings.WindowTop + settings.WindowHeight) < SystemParameters.VirtualScreenHeight)
			{ Top = settings.WindowTop; }
			// 幅
			if (settings.WindowWidth > 0 &&
				settings.WindowWidth <= SystemParameters.WorkArea.Width)
			{ Width = settings.WindowWidth; }
			// 高さ
			if (settings.WindowHeight > 0 &&
				settings.WindowHeight <= SystemParameters.WorkArea.Height)
			{ Height = settings.WindowHeight; }
			// 最大化
			if (settings.WindowMaximized)
			{
				// ロード後に最大化
				Loaded += (o, e) => WindowState = WindowState.Maximized;
			}
		}
	}
}
