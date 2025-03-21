using OpenQA.Selenium;
using System.Windows.Threading;

namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
	{

		private DispatcherTimer dispatcherTimer;

		private void InitScreenCaptureRenderTimer()
		{
			if ( dispatcherTimer != null )
			{
				DisposeScreenCaptureTimer();
			}

			dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Interval = TimeSpan.FromSeconds(3);
			dispatcherTimer.Tick += DispatherScreenCaptureRenderTimer_Tick;
			dispatcherTimer.Start();

		}

		private void DisposeScreenCaptureTimer()
		{
			if (dispatcherTimer == null)
			{
				return;
			}
			dispatcherTimer.Stop();
			dispatcherTimer.Tick -= DispatherScreenCaptureRenderTimer_Tick;
			dispatcherTimer = null;
		}

		private void DispatherScreenCaptureRenderTimer_Tick(object? sender, EventArgs e)
		{
			var bmpimage = DoScreenCapture();
			//if (bmpimage != null)
			//{
			//	//ImageCapture.Source = bmpimage;
			//}

			if ( IsGenerationInProgress )
			{

                try
				{
                    var progress_element = webSenderSelenium1.webdriver.FindElement( By.XPath( "//*[@id=\"img2img_results_panel\"]/div[1]/div[1]" ) );
					var progressVis = progress_element.Enabled;
                    var progress = progress_element.Text;
					Debug.WriteLine( "PROGRESS: " + progress );
					Debug.WriteLine( "PROGRESS: " + progressVis );

					pane_progressGen.Visibility = Visibility.Visible;
                    textBlock_progressGen.Text = progress;

                    if ( string.IsNullOrEmpty(progress))
					{
                        pane_progressGen.Visibility = Visibility.Collapsed;
                        GenerationProgressEmptyCount++;

						if (GenerationProgressEmptyCount > 4)
						{
                            IsGenerationInProgress = false;
							GenerationProgressEmptyCount = 0;
							imageGallery1.ResetImages();
							imageGallery1.SetCurrentImageIndexAsLast();
						}
                    }
					

                }
                catch ( NoSuchElementException )
				{
                    pane_progressGen.Visibility = Visibility.Collapsed;
                    textBlock_progressGen.Text = "";

                    IsGenerationInProgress = false;
                    GenerationProgressEmptyCount = 0;
                    imageGallery1.ResetImages();
					imageGallery1.SetCurrentImageIndexAsLast();
				}

            }
		}



	}







}
