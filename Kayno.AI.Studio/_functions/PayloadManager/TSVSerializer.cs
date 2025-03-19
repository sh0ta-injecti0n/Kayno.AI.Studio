using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kayno.AI.Studio
{


	public class TsvSerializer
	{

		// 汎用シリアライズ
		/// <summary>
		/// 任意のObservableCollectionからTSVファイルにシリアライズして保存します。
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="fileName"></param>
		/// <param name="items"></param>
		public static void SaveToTsv<T>( string fileName, ObservableCollection<T> items )
		{
			//FileParentDirPath = Directory.GetParent( fileName ).FullName;

			var properties = typeof( T ).GetProperties();
			var header = string.Join( "\t", properties.Select( p => p.Name ) );

			var lines = items.Select
			(
				item =>
				{
					return string.Join
					( "\t", properties.Select
						( prop =>
						{
							var value = prop.GetValue( item );

							// Tips:
							// prop.Name == nameof(Payload.調べたいプロパティ) でプロパティチェック可能
							// value で現在のプロパティの値、TSVでのセルに相当
							
							if ( value is List<string> list )
							{
								return string.Join( ",", list );
								// タグとか
							}

                            if ( value != null && value.ToString().Contains( Environment.NewLine ) )
							{
								if (!value.ToString().StartsWith('\"'))
								{
									value = "\"" + value.ToString();
                                }
								if (!value.ToString().EndsWith('\"'))
								{
									value = value.ToString() + "\"";
								}
								// PropertyValue内に改行があるとき、""でエスケープする (プロンプト系など)
								// (セミコロンがすでにないことを確認する)
							}

							return value?.ToString() ?? "";
						} )
					);
				} 
			);

			if (fileName.ToLower().EndsWith("vae.tsv"))
			{
				lines.Append( "None\tNone" );
				lines.Append( "Automatic\tAutomatic" );
            }

			// ディレクトリが存在しない場合、作成する
			if ( !Directory.Exists( Path.GetDirectoryName(fileName) ) )
			{
				Directory.CreateDirectory( Path.GetDirectoryName( fileName ) );
			}

			File.WriteAllLines( fileName, new[] { header }.Concat( lines ) );
			
		}


        // 汎用デシリアライズ
        /// <summary>
        /// TSVファイルから任意のObservableCollectionにシリアライズします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ObservableCollection<T> LoadFromTsv<T>( string filePath )
        {
            if ( string.IsNullOrEmpty( filePath ) ) return null;

            filePath = Path.GetFullPath( filePath );
            if ( !File.Exists( filePath ) ) { return null; }

            var items = new ObservableCollection<T>();
            var properties = typeof( T ).GetProperties().Where( p => p.CanWrite ).ToArray();

            //var utf8_encoding = new System.Text.UTF8Encoding( false );
            //var lines = File.ReadAllLines( filePath, utf8_encoding );

            using var Parser = new TextFieldParser( filePath );
            Parser.TextFieldType = FieldType.Delimited;
            Parser.SetDelimiters( "\t" );

            //var headerCols = lines[ 0 ].Split( '\t' ); // ヘッダー行

            var headerCols = Parser.ReadFields();
			// ヘッダー 行
			Debug.WriteLine( headerCols );

            while ( !Parser.EndOfData )
            {
				var columns = Parser.ReadFields();
                var item = Activator.CreateInstance<T>();


                for ( int j = 0; j < properties.Length && j < properties.Length; j++ )
                {
                    var property = properties[ j ];
                    string value = "";

                    try
                    {
                        value = columns[ headerCols.ToList().IndexOf( property.Name ) ];
						// プロパティ名と同じヘッダ名の値があるか探す

						if ( value.Contains( "prompt" ) )
						{
							Debug.WriteLine( $"{property.Name}: {value}" );
						}
                    }
                    catch
                    {
                        property.SetValue( item, null );
                        continue;
                    }
                    // プロパティと同じ名前の列から値を取得
                    //string value = columns[ j ] ;


                    Type propType = property.PropertyType;
                    if ( propType != typeof( ObservableCollection<PayloadTemplate> ) )
                    {
                        if ( string.IsNullOrEmpty( value ) || value.Equals( "None", StringComparison.OrdinalIgnoreCase ) )
                        {
                            property.SetValue( item, null );
                            continue;
                        }
                    }

                    // 型ごとの処理
                    if ( propType == typeof( string ) )
                    {
                        property.SetValue( item, value );
                    }
                    else if ( propType == typeof( bool? ) || propType == typeof( bool ) )
                    {
                        property.SetValue( item, bool.TryParse( value, out var boolValue ) ? boolValue : (bool?)null );
                    }
                    else if ( propType == typeof( double? ) || propType == typeof( double ) )
                    {
                        property.SetValue( item, double.TryParse( value, out var doubleValue ) ? doubleValue : (double?)null );
                    }
                    else if ( propType == typeof( int? ) || propType == typeof( int ) )
                    {
                        property.SetValue( item, int.TryParse( value, out var intValue ) ? intValue : (int?)null );
                    }
                    else if ( propType.IsEnum )
                    {
                        property.SetValue( item, Enum.TryParse( propType, value, true, out var enumValue ) ? enumValue : null );
                    }
                    else if ( Nullable.GetUnderlyingType( propType )?.IsEnum == true ) // Nullable Enum
                    {
                        var enumType = Nullable.GetUnderlyingType( propType );
                        property.SetValue( item, Enum.TryParse( enumType, value, true, out var enumValue ) ? enumValue : null );
                    }
                    else if ( propType == typeof( List<string> ) )
                    {
                        property.SetValue( item, value.Split( ',' ).ToList() );
                    }
                    else if ( propType == typeof( ObservableCollection<PayloadTemplate> ) )
                    {
                        // ObservableCollectionに変換する場合の処理
                        // UI_ItemsSourceを外部TSVから読み込む処理
                        Debug.WriteLine( $"{property.Name}, {value}" );

                        var pathProperty = properties.FirstOrDefault( p => p.Name == nameof( Payload.UI_ItemsSourcePath ) );
                        // まず UI_ItemsSourcePath のプロパティを取得

                        if ( pathProperty != null )
                        {
                            var pathValue = pathProperty.GetValue( item ) as string;
                            property.SetValue( item, LoadFromTsv<PayloadTemplate>( pathValue ) );
                        }

                    }
                    else
                    {
                        try
                        {
                            // その他の型は変換して設定
                            object convertedValue = Convert.ChangeType( value, propType );
                            property.SetValue( item, convertedValue );
                        }
                        catch
                        {
                            // 変換失敗時はnull
                            property.SetValue( item, null );
                        }
                    }

                    // キャスト(Parse)がめんどくさいがこれが一番堅実。なぜならEnum系が全滅するから
                }

                items.Add( item );
            }

            return items;
        }



        // 汎用デシリアライズ
        /// <summary>
        /// TSVファイルから任意のObservableCollectionにシリアライズします。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static ObservableCollection<T> LoadFromTsv2<T>( string filePath )
		{
			if ( string.IsNullOrEmpty( filePath ) ) return null;

			filePath = Path.GetFullPath( filePath );
			if ( !File.Exists( filePath ) ) { return null; }

			var items = new ObservableCollection<T>();
			var properties = typeof( T ).GetProperties().Where( p => p.CanWrite ).ToArray();

			var utf8_encoding = new System.Text.UTF8Encoding( false );
			var lines = File.ReadAllLines( filePath, utf8_encoding );

			var headerCols = lines[ 0 ].Split( '\t' ); // ヘッダー行

			for ( int i = 1; i < lines.Length; i++ ) // ヘッダ行をスキップ
			{
				var columns = lines[ i ].Split( '\t' );
				var item = Activator.CreateInstance<T>();

				for ( int j = 0; j < properties.Length && j < properties.Length; j++ )
				{
					var property = properties[ j ];
					string value = "";
					try
					{ 
						value = columns[ headerCols.ToList().IndexOf( property.Name ) ];
						// プロパティ名と同じヘッダ名の値があるか探す
					}
					catch
					{
						property.SetValue( item, null );
						continue;
					}
					// プロパティと同じ名前の列から値を取得
					//string value = columns[ j ] ;


					Type propType = property.PropertyType;
					if ( propType != typeof(ObservableCollection<PayloadTemplate>) )
					{
						if ( string.IsNullOrEmpty( value ) || value.Equals( "None", StringComparison.OrdinalIgnoreCase ) )
						{
							property.SetValue( item, null );
							continue;
						}
					}

					// 型ごとの処理
					if ( propType == typeof( string ) )
					{
						property.SetValue( item, value );
					}
					else if ( propType == typeof( bool? ) || propType == typeof( bool ) )
					{
						property.SetValue( item, bool.TryParse( value, out var boolValue ) ? boolValue : (bool?)null );
					}
					else if ( propType == typeof( double? ) || propType == typeof( double ) )
					{
						property.SetValue( item, double.TryParse( value, out var doubleValue ) ? doubleValue : (double?)null );
					}
					else if ( propType == typeof( int? ) || propType == typeof( int ) )
					{
						property.SetValue( item, int.TryParse( value, out var intValue ) ? intValue : (int?)null );
					}
					else if ( propType.IsEnum )
					{
						property.SetValue( item, Enum.TryParse( propType, value, true, out var enumValue ) ? enumValue : null );
					}
					else if ( Nullable.GetUnderlyingType( propType )?.IsEnum == true ) // Nullable Enum
					{
						var enumType = Nullable.GetUnderlyingType( propType );
						property.SetValue( item, Enum.TryParse( enumType, value, true, out var enumValue ) ? enumValue : null );
					}
					else if ( propType == typeof( List<string> ) )
					{
						property.SetValue( item, value.Split( ',' ).ToList() );
					}
					else if ( propType == typeof( ObservableCollection<PayloadTemplate> ) )
					{
						// ObservableCollectionに変換する場合の処理
						// UI_ItemsSourceを外部TSVから読み込む処理
						Debug.WriteLine( $"{property.Name}, {value}" );

						var pathProperty = properties.FirstOrDefault( p => p.Name == nameof( Payload.UI_ItemsSourcePath ) );
						// まず UI_ItemsSourcePath のプロパティを取得

						if ( pathProperty != null )
						{
							var pathValue = pathProperty.GetValue( item ) as string;
							property.SetValue( item, LoadFromTsv<PayloadTemplate>( pathValue ) );
						}

					}
					else
					{
						try
						{
							// その他の型は変換して設定
							object convertedValue = Convert.ChangeType( value, propType );
							property.SetValue( item, convertedValue );
						}
						catch
						{
							// 変換失敗時はnull
							property.SetValue( item, null );
						}
					}

					// キャスト(Parse)がめんどくさいがこれが一番堅実。なぜならEnum系が全滅するから
				}

				items.Add( item );
			}

			lines = null;

			return items;
		}






	}














}
