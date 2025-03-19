using OpenQA.Selenium;
using System.Windows.Threading;

namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
	{

		private DispatcherTimer timerRender;

		private void InitScreenCaptureRenderTimer()
		{
			if ( timerRender != null )
			{
				DisposeScreenCaptureTimer();
			}

			timerRender = new DispatcherTimer();
			timerRender.Interval = TimeSpan.FromSeconds(3);
			timerRender.Tick += DispatherScreenCaptureRenderTimer_Tick;
			timerRender.Start();

		}

		private void DisposeScreenCaptureTimer()
		{
			if (timerRender == null)
			{
				return;
			}
			timerRender.Stop();
			timerRender.Tick -= DispatherScreenCaptureRenderTimer_Tick;
			timerRender = null;
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
