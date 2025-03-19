using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kayno.AI.Studio.Functions.Web
{
    public static class WebSenderSelenium_JSDropFile
    {

		/// <summary>
		/// Selenium と JS から、Gradio にも対応したファイルドロップ操作をシミュレートします。
		/// </summary>
		/// <param name="target"></param>
		/// <param name="filePath"></param>
		/// <param name="offsetX"></param>
		/// <param name="offsetY"></param>
		/// <exception cref="FileNotFoundException"></exception>
		public static void DropFile(IWebElement target, string filePath, double offsetX = 0, double offsetY = 0)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException(filePath);

			IWebDriver driver = ((WebElement)target).WrappedDriver;

			IJavaScriptExecutor jse = (IJavaScriptExecutor)driver;

			IWebElement input = (IWebElement)jse.ExecuteScript(JS_DROP_FILE, target, offsetX, offsetY);
			input.SendKeys(filePath);

			// 神コード
			// 参照:　↓　RemoteWebElementはないので WebElement に変更してOK
			// https://stackoverflow.com/questions/38829153/selenium-drag-and-drop-from-file-system-to-webdriver
		}

		const string JS_DROP_FILE = "for(var b=arguments[0],k=arguments[1],l=arguments[2],c=b.ownerDocument,m=0;;){var e=b.getBoundingClientRect(),g=e.left+(k||e.width/2),h=e.top+(l||e.height/2),f=c.elementFromPoint(g,h);if(f&&b.contains(f))break;if(1<++m)throw b=Error('Element not interractable'),b.code=15,b;b.scrollIntoView({behavior:'instant',block:'center',inline:'center'})}var a=c.createElement('INPUT');a.setAttribute('type','file');a.setAttribute('style','position:fixed;z-index:2147483647;left:0;top:0;');a.onchange=function(){var b={effectAllowed:'all',dropEffect:'none',types:['Files'],files:this.files,setData:function(){},getData:function(){},clearData:function(){},setDragImage:function(){}};window.DataTransferItemList&&(b.items=Object.setPrototypeOf([Object.setPrototypeOf({kind:'file',type:this.files[0].type,file:this.files[0],getAsFile:function(){return this.file},getAsString:function(b){var a=new FileReader;a.onload=function(a){b(a.target.result)};a.readAsText(this.file)}},DataTransferItem.prototype)],DataTransferItemList.prototype));Object.setPrototypeOf(b,DataTransfer.prototype);['dragenter','dragover','drop'].forEach(function(a){var d=c.createEvent('DragEvent');d.initMouseEvent(a,!0,!0,c.defaultView,0,0,0,g,h,!1,!1,!1,!1,0,null);Object.setPrototypeOf(d,null);d.dataTransfer=b;Object.setPrototypeOf(d,DragEvent.prototype);f.dispatchEvent(d)});a.parentElement.removeChild(a)};c.documentElement.appendChild(a);a.getBoundingClientRect();return a;";

	}




}
