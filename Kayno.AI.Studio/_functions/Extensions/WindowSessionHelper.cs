using System.IO;
using System.Text.Json;
using System.Windows;

public static class WindowSessionHelperExtensions
{
	private static string SettingsFilePath => Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "window_settings.json" );

	public static void LoadWindowSettings( this Window window )
	{
		if ( !File.Exists( SettingsFilePath ) )
			return;

		try
		{
			var json = File.ReadAllText( SettingsFilePath );
			var settings = JsonSerializer.Deserialize<WindowSettings>( json );

			if ( settings != null )
			{
				window.WindowState = settings.WindowState == WindowState.Minimized ? WindowState.Normal : settings.WindowState;

				if ( settings.WindowState == WindowState.Normal )
				{
					double screenWidth = SystemParameters.VirtualScreenWidth;
					double screenHeight = SystemParameters.VirtualScreenHeight;

					window.Left = Math.Max( 0, Math.Min( settings.Left, screenWidth - settings.Width ) );
					window.Top = Math.Max( 0, Math.Min( settings.Top, screenHeight - settings.Height ) );
					window.Width = Math.Min( settings.Width, screenWidth );
					window.Height = Math.Min( settings.Height, screenHeight );
				}
			}
		}
		catch { /* エラーハンドリング */ }
	}

	public static void SaveWindowSettings( this Window window )
	{
		if ( window.WindowState == WindowState.Maximized )
		{
			var settings = new WindowSettings { WindowState = WindowState.Maximized };
			SaveToFile( settings );
			return;
		}

		var normalSettings = new WindowSettings
		{
			Left = window.Left,
			Top = window.Top,
			Width = window.Width,
			Height = window.Height,
			WindowState = window.WindowState
		};

		SaveToFile( normalSettings );
	}

	private static void SaveToFile( WindowSettings settings )
	{
		try
		{
			var json = JsonSerializer.Serialize( settings, new JsonSerializerOptions { WriteIndented = true } );
			File.WriteAllText( SettingsFilePath, json );
		}
		catch { /* エラーハンドリング */ }
	}

	private class WindowSettings
	{
		public double Left { get; set; }
		public double Top { get; set; }
		public double Width { get; set; }
		public double Height { get; set; }
		public WindowState WindowState { get; set; }
	}
}
