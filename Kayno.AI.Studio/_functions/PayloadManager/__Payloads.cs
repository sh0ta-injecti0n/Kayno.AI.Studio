using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;

namespace Kayno.AI.Studio
{
	//public partial class MainWindow : Window
	public class Payloads : ObservableCollection<Payload>, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged( string propertyName )
		{
			PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( propertyName ) );
		}

		#region ## Properties

		public string DataDir { get; set; }

		#endregion


		public Payloads(string index)
		{
			DataDir = index;
			// 000, 001, ...

		}





	}







}
