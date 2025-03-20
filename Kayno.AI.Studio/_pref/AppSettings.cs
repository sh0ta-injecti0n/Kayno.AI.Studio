

public class AppSettings
{
	/// <summary>
	/// アプリケーションの設定を格納するインスタンス。
	/// </summary>
	public static AppSettings Instance => _instance ??= Load();
	private static AppSettings _instance;

	// ウィンドウの状態
	public double Left { get; set; } = 4;
	public double Top { get; set; } = 4;
	public double Width { get; set; } = 400;
	public double Height { get; set; } = 1080;
	public WindowState WindowState { get; set; }

	// Settings.settings から移行した設定
	public string Pref_Main_PrefPayload_ItemsSourcePrefix { get; set; } = "ItemsSource_-_";
	public string Pref_Main_PrefPayload_DataGlobal { get; set; } = ".\\DataGlobal\\";
	public string Pref_Main_PrefPayload_Data { get; set; } = ".\\Data\\";
	public string Pref_Main_PrefPayload_FileName { get; set; } = "Payload.tsv";
	public double Pref_ScreenCaptureRectX { get; set; } = 0;
	public double Pref_ScreenCaptureRectY { get; set; } = 0;
	public double Pref_ScreenCaptureRectWidth { get; set; } = 512;
	public double Pref_ScreenCaptureRectHeight { get; set; } = 512;
	public string Pref_Main_Path_SDModelFolderFilters { get; set; } = "models\\stable-diffusion,models\\VAE,models\\lora,models\\controlnet,embeddings";
	public string Pref_Main_Path_SD_CN_Preprocessors { get; set; } = ".\\extensions\\sd-webui-controlnet\\scripts\\preprocessor";
	public bool Pref_App_UseSelectedTextGradientColor { get; set; } = true;
	public string Pref_Main_Path_SD { get; set; } = string.Empty;


	public static string SettingsFilePath =>
	Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "app_settings.json");


	public static AppSettings Load()
	{
		if (!File.Exists(SettingsFilePath))
			return new AppSettings(); // デフォルト値で初期化

		try
		{
			string json = File.ReadAllText(SettingsFilePath);
			return JsonSerializer.Deserialize<AppSettings>(json);
		}
		catch
		{
			return new AppSettings(); // エラー時はデフォルト値
		}
	}

	public void Save()
	{
		try
		{
			string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
			File.WriteAllText(SettingsFilePath, json);
		}
		catch { /* エラーハンドリング */ }
	}


	/// <summary>
	/// ウィンドウ状態の適用
	/// </summary>
	/// <param name="window"></param>
	public void ApplyToWindow(Window window)
	{
		window.WindowState = WindowState == WindowState.Minimized ? WindowState.Normal : WindowState;

		if (window.WindowState == WindowState.Normal)
		{
			double screenWidth = SystemParameters.VirtualScreenWidth;
			double screenHeight = SystemParameters.VirtualScreenHeight;

			window.Left = Math.Max(0, Math.Min(Left, screenWidth - Width));
			window.Top = Math.Max(0, Math.Min(Top, screenHeight - Height));
			window.Width = Math.Min(Width, screenWidth);
			window.Height = Math.Min(Height, screenHeight);
		}
	}

	/// <summary>
	/// ウィンドウ状態の更新
	/// </summary>
	/// <param name="window"></param>
	public void UpdateFromWindow(Window window)
	{
		if (window.WindowState == WindowState.Maximized)
		{
			WindowState = WindowState.Maximized;
			return;
		}

		Left = window.Left;
		Top = window.Top;
		Width = window.Width;
		Height = window.Height;
		WindowState = window.WindowState;
	}

}
