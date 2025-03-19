using Kayno.AI.Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


public static class CommandHelper
{

	/// <summary>
	/// お手軽にCommandを作成します。
	/// </summary>
	/// <param name="commandName">コマンドの名称。nameof()でも可</param>
	/// <param name="text">can be empty</param>
	/// <param name="executedName">デフォルトでは コマンド名_Executed のメソッドを自動割り当て</param>
	/// <param name="canExecute">Nullable</param>
	/// <remarks>MainWindowの中でCreateCommandでCommand を作成し、イベントをexecutedに割り当てればOK</remarks>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static RoutedUICommand CreateCommand(string commandName, string text = "", string executedName = "_Executed", CanExecuteRoutedEventHandler? canExecute = null)
	{
		// 呼び出し元の変数名（コマンド名）を取得
		//if (string.IsNullOrEmpty( commandName ) )
		//{
		// commandName = new StackTrace().GetFrame( 1 ).GetMethod().Name;
		//}
		// ↑❌️ 全自動宣言とはいかず……

		var command = new RoutedUICommand( text, commandName, typeof(Window) );
		// 本来はMainWindowの型を指定

		var mainWindow = Application.Current.MainWindow;
		// MainWindowのインスタンスを取得

		if ( mainWindow != null )
		{
			var executedMethod = mainWindow.GetType().GetMethod( commandName + executedName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			// コマンド名に対応する "Executed" メソッドを取得 (CMD_Test → CMD_Test_Executed)

			if ( executedMethod != null )
			{
				var executedDelegate = (ExecutedRoutedEventHandler)Delegate.CreateDelegate( typeof( ExecutedRoutedEventHandler ), mainWindow, executedMethod );

				if ( canExecute == null )
				{
					canExecute = ( s, e ) => e.CanExecute = true;
				}
				mainWindow.CommandBindings.Add( new CommandBinding( command, executedDelegate, canExecute ) );
				// コマンドバインディングを登録
			}
			else
			{
				throw new InvalidOperationException( $"Method '{commandName}_Executed' not found in {mainWindow.GetType().Name}" );
			}
		}

		return command;
	}



}