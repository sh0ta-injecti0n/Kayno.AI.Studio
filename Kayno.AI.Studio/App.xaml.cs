using System.Configuration;
using System.Data;
using System.Windows;

namespace Kayno.AI.Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

		public App()
		{
			if ( Pref.Default.App_UseSelectedTextGradientColor )
			{
				AppContext.SetSwitch("Switch.System.Windows.Controls.Text.UseAdornerForTextboxSelectionRendering", false);
				// for TextBox "SelectionTextBrush" Property
			}
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			DispatcherUnhandledException += (o, args) =>
			{
				args.Handled = true;
				// 例外処理の中断

				//Environment.Exit(1);
			};

			AppDomain.CurrentDomain.UnhandledException += (o, args) =>
			{
				//Environment.Exit(1);
			};

			TaskScheduler.UnobservedTaskException += (o, args) =>
			{
				// ログ出力の実装
				args.SetObserved();
			};
		}


	}


}
