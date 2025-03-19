namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
  {

		/*
		 * Commands をあなたのプロジェクトでもかんたんに使う方法:
		 * 0: Extensions にある CommandHelper.cs をコピー
		 * 1: CommandHelper.CreateCommand(～) で RoutedUICommand を宣言
		 * 2: ↑の「宣言名+_Executed」 でメソッドを実装
		 * 3: XAML 上のボタンなどに追加
		 *	   Command="{x:Static this:MainWindow.CMD_Sample}"
		 *	   
		 * +: お好みでキーボードショートカットなど InitializeCommandBindings() を カスタム
		 * +: InitializeCommandBindings() を MainWindow の Loaded などで実行
		 * 
		 */

		/*
		 * 余談:
		 * ちなみにグローバル翻訳は
		 * xmlns:Prop="clr-namespace:クラス名.Properties.Resources"
		 * を追加して、
		 * Content="{x:Static Prop.Resource.設定名}"
		 * で可能
		 * 
		 */

		// ↓　サンプル　↓

		//public static RoutedUICommand CMD_Sample = CommandHelper.CreateCommand( nameof( CMD_Sample ) );
		//
		//private async void CMD_Sample_Executed( object sender, ExecutedRoutedEventArgs e )
		//{
		//	Debug.WriteLine( "Event: " + e.ToString() );
		//	MessageBox.Show( "test" );
		//}

		//private void InitializeCommandBindings()
		//{
		//	var inputs = new InputBindingCollection
		//	{
		//		new KeyBinding(CMD_Sample, Key.Left, ModifierKeys.Alt)
		//		// add commands here
		//	};
		//	this.InputBindings.AddRange(inputs);
		//	// ショートカットへの登録 (任意)
		//}


	}
}
