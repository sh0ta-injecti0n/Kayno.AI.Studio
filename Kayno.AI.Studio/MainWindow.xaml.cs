
global using System;
global using System.Collections.ObjectModel;
global using System.Diagnostics;
global using System.Net;
global using System.Net.Http;
global using System.Text;
global using System.Text.Json;
global using System.Windows;
global using System.Windows.Controls;
global using System.Windows.Controls.Primitives;
global using System.Windows.Input;
global using System.Windows.Interop;
global using System.Windows.Media.Imaging;
global using System.IO;
global using System.Runtime.InteropServices;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using Microsoft.Win32;

global using DRAW = System.Drawing;
//global using Pref = Kayno.AI.Studio.Properties.Settings;
global using PropRes = Kayno.AI.Studio.Properties.Resources;

using OpenQA.Selenium;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Data;
using System.Configuration;
using System.Security.Policy;

namespace Kayno.AI.Studio
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public WebSenderSelenium webSenderSelenium1 { get; set; } = new WebSenderSelenium();

		public MainWindow()
		{
			InitializeComponent();

            InitializeCommandBindings();
			Loaded += MainWindow_Loaded;
			Closing += MainWindow_Closing;

            //appSettings = appSettings.Load();
            //appSettings.ApplyToWindow(this);
            //this.LoadWindowSettings();
            AppSettings.Instance.ApplyToWindow(this);
            // 1st


			LoadPreferences();
			// 2nd
		}

		private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			await LoadPayload();
			// 3rd
			// 先に読み込むこと

			if (payloads_All != null && payloads_All.Any())
			{
				var url = payloads_All.First(i => i.PropertyName.Contains("url")).PropertyValue.ToString();
				webSenderSelenium1.InitWebData(url);
			}
			// SDWebUIを先に開いておく

			//TogglePinPane(false);
		}

		protected override async void OnContentRendered(EventArgs e)
		{
			await InitScreenCapture();
			base.OnContentRendered( e );
		}

		private async void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
		{
			DisposeScreenCaptureTimer();
			webSenderSelenium1.QuitSelenium();

			SavePayload();
            //appSettings.UpdateFromWindow(this);
            //appSettings.Save();
            //this.SaveWindowSettings();  // 拡張メソッドで設定を保存
            //await SavePreferences();
            AppSettings.Instance.UpdateFromWindow(this);
            AppSettings.Instance.Save();
		}

        #region ## Application Preferences

        /// <summary>
        /// stable-diffusion > models のフォルダ。
        /// </summary>
        public string Path_SDModel
        {
            get
            {
                return Path.Combine( AppSettings.Instance.Pref_Main_Path_SD, "models" );
            }
        }

        /// <summary>
        /// stable-diffusion > embeddings のフォルダ。
        /// </summary>
        public string Path_SDEmbeddings
        {
            get
            {
                return Path.Combine( AppSettings.Instance.Pref_Main_Path_SD, "embeddings" );
            }
        }

        /// <summary>
        /// stable-diffusion > outputs のフォルダ。
        /// </summary>
        public string Path_SDOutput
        {
            get
            {
                return Path.Combine( AppSettings.Instance.Pref_Main_Path_SD, "outputs" );
            }
        }


        private void LoadPreferences()
        {
            // 設定ファイルを読み込む

            if ( string.IsNullOrEmpty( AppSettings.Instance.Pref_Main_Path_SD )
                || !Directory.Exists( AppSettings.Instance.Pref_Main_Path_SD ) )
            {
                var msg = Properties.Resources.Dialog_InitialLaunch;
                MessageBox.Show( msg );

                var r = new OpenFolderDialog();
                r.Title = Properties.Resources.Dialog_DefineSDPath;
                var res = r.ShowDialog();
                if ( res == false ) return;

                //config.AppSettings.Settings[ nameof(AppSettings.Instance.Pref_Main_Path_SD) ].Value = r.FolderName;
                AppSettings.Instance.Pref_Main_Path_SD = r.FolderName;
                // app.configで管理する場合はconfigのほうを使う
            }

            imageGallery1.SourceDirectory = Path.Combine( Path_SDOutput, "img2img-images", DateTime.Today.ToString( "yyyy-MM-dd" ) );
            imageGallery1.FilterFileName = "*.*";
            imageGallery1.ResetImages();

        }


        private async Task SavePreferences()
        {
            //Pref.Default.Save();
        }

        #endregion

        private void _test__KeyDown( object sender, KeyEventArgs e )
		{
			if (e.Key == System.Windows.Input.Key.Enter)
			{
				var st = ((TextBox)sender).Text;

				try
				{
					var element = webSenderSelenium1.webdriver.FindElement( By.XPath( st ) );
					element.Click();
				}
				catch (Exception ex)
				{
					Debug.WriteLine( ex.Message );
				}
			}
		}

    }









}