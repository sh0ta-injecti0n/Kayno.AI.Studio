using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Kayno.AI.Studio
{
  public class Payload : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged( string propertyName )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
			Debug.WriteLine( "OnPropertyChanged" );
		}

		#region ## Properties 

		public string PropertyName { get; set; }

		private object? _propertyValue;
		public object? PropertyValue
		{
			get => _propertyValue;
			set
			{
				if ( _propertyValue != value )
				{
					_propertyValue = value;
					OnPropertyChanged( nameof( PropertyValue ) );
				}
			}
		}

		public string? Label { get; set; }
		public string? LabelSrc { get; set; }
		public string? Tag { get; set; }
		public SeleniumBySelector? WebSelectorKey { get; set; }
		public string? WebSelectorValue { get; set; }
		public UISelector? UI { get; set; }
		public bool? UI_IsPinned { get; set; }
		public bool? UI_IsVisible { get; set; }
		public double? UI_SliderMinVal { get; set; }
		public double? UI_SliderMaxVal { get; set; }
		public double? UI_SliderSnapValue { get; set; }
		public string? UI_ItemsSourcePath { get; set; }
		public ObservableCollection<PayloadTemplate>? UI_ItemsSource { get; set; }
		public string? UI_ItemsSourceFilter { get; set; }
		public string Command { get; set; }
		public string? Comments { get; set; }


		#endregion

		public Payload() 
		{
			
		}

	}

	public enum UISelector
	{
		CheckBox = 10,
		Button = 11,
		Slider = 20,
		TextBlock = 30,
		TextBox = 31,
		SplitButton = 40,
		List = 41,
		Expander = 90,
		DropArea = 91,
		
		None = 0
	}

	public enum SeleniumBySelector
	{
		ID = 10,
		Class = 11,
		LinkText = 30,
		LinkTextPartial = 31,
		CSS_Selector = 40,
		TagName = 41,
		XPath = 90,

		None = 0
	}






}
