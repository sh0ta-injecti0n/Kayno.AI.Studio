using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Kayno.AI.Studio
{
	public partial class ItemsControl1 : UserControl
	{

		ObservableCollection<PayloadTemplate> PayloadTemplates;

		public ItemsControl1()
		{
			InitializeComponent();
            Loaded += ItemsControl1_Loaded;
        }

		public ItemsControl1(object data)
		{
            InitializeComponent();
			DataContext = data;
            Loaded += ItemsControl1_Loaded;
        }

        private void ItemsControl1_Loaded( object sender, RoutedEventArgs e )
        {

            if ( DataContext == null ) return;

            PayloadTemplates = (ObservableCollection<PayloadTemplate>)DataContext;

            listView_filter.ItemsSource = PayloadTemplates.DistinctBy( i => i.TCategory2 );
            listView_items.ItemsSource = PayloadTemplates;

            //var bind = new Binding();
            //BindingOperations.SetBinding( image_thumb, Image.SourceProperty, bind );
        }

        private void listView_filter_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
			try
			{
                var items = listView_filter.SelectedItems;
                var newlist = new List<PayloadTemplate>();
                foreach ( PayloadTemplate item in items )
                {
                    var ls = PayloadTemplates.Where( i => i.TCategory2 == item.TCategory2 ).ToList();
                    newlist.AddRange( ls );
                }
                listView_items.ItemsSource = new ObservableCollection<PayloadTemplate>( newlist );

            }
            catch ( Exception ex )
			{
			}
			}

        private void ButtonBack_Click( object sender, RoutedEventArgs e )
        {
            try
            {
                var po = (Popup)Parent;
                po.IsOpen = false;
            }
            catch
            { 
            }
        }
    }









}
