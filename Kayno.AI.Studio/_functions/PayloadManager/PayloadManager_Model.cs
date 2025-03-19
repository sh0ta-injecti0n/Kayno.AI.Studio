using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Data;
using System.Windows.Media.Animation;

namespace Kayno.AI.Studio
{
	public partial class MainWindow : Window
	{

		/// <summary>
		/// 設定されたモデルのフォルダから、各種モデルのTSVファイルを更新します。
		/// </summary>
		/// <remarks>CurrentPaylaodCollection は初期化前のため使用不可</remarks>
		public void ResetModelSourceFiles()
		{
			// embeddingsだけmodelsフォルダに入ってないので、うっかり末尾にmodelsとか入れないこと

			//・sd
			//　・models
			//　　・Stable-diffusion
			//　　・Lora
			//　　・Vae
			//　　・ControlNet
			//　・embeddings

			if ( !Directory.Exists( Path_SDModel ) )
			{
				MessageBox.Show( "Directory not exist: " + Path_SDModel );
				return;
			}

			if (!Directory.Exists(Path_SDEmbeddings))
            {
                MessageBox.Show( "Directory not exist: " + Path_SDEmbeddings );
                return;
            }

            var allFiles = Directory.EnumerateFiles( Path_SDModel, "*.*", SearchOption.AllDirectories )
					.Where( f => f.EndsWith( ".safetensors" ) || f.EndsWith( ".ckpt" ) || f.EndsWith( ".pt" ) || f.EndsWith( ".pth" ) )
					.ToList();
			var embeddings = Directory.EnumerateFiles( Path_SDEmbeddings, "*.*", SearchOption.AllDirectories )
                    .Where( f => f.EndsWith( ".safetensors" ) || f.EndsWith( ".ckpt" ) || f.EndsWith( ".pt" ) || f.EndsWith( ".pth" ) )
                    .ToList();
			allFiles.AddRange( embeddings );

            var payloadTemplates = new ObservableCollection<PayloadTemplate>();

			foreach ( var filePath in allFiles )
			{

                var parentDir = Path.GetDirectoryName( filePath );
				var parnetDirName = new DirectoryInfo( parentDir ).Name;
                var pathParts = parentDir.Substring( Pref.Default.Main_Path_SD.Length ).Split( Path.DirectorySeparatorChar );
                var nextFolderName = pathParts.Length > 0 ? pathParts[ 0 ] : parnetDirName;
				// SDパスの次の階層のフォルダ

                var fileName = Path.GetFileName( filePath );
				//var parentDir = Path.GetDirectoryName( filePath );
				var relativePath = parentDir != null ? Path.GetRelativePath( Pref.Default.Main_Path_SD, parentDir ) : string.Empty;
				var category1 = nextFolderName;
				// e.g. models, embeddings
				var category2 = parnetDirName;
				// e.g. Stable-diffusion, Lora, controlnet, ...

				var name = filePath.ToLower().Contains( "controlnet" ) ? Path.GetFileNameWithoutExtension( fileName ) : fileName;
				// controlnetのモデルはファイル拡張子入れないで判断してるのでそれ用

                var payloadTemplate = new PayloadTemplate
				{
					TPropertyName = fileName,
					TPropertyValue = name,
					TThumbPath = null,
					TLabel = name,
					TPath = filePath,
					TParentDir = relativePath,
					TCategory = category1,
					TCategory2 = category2,
					TTags = category2.Split( "\\" ).ToList<string>(),
					TDescription = "=> LoadfromJson(filePath);",  // 必要に応じて設定
					TParameter = "",    // 必要に応じて設定
					TComment = ""          // 必要に応じて設定
				};

				payloadTemplates.Add( payloadTemplate );
			}

			var dataGlobal = Pref.Default.Main_PrefPayload_DataGlobal;
			dataGlobal = Path.GetFullPath( dataGlobal );
			var modelTsvPath = Path.Combine(dataGlobal, "models.tsv");

			TsvSerializer.SaveToTsv<PayloadTemplate>( modelTsvPath, payloadTemplates );
			// デバッグ・確認用

			var filters = Pref.Default.Main_Path_SDModelFolderFilters.ToLower().Split( "," );
			foreach (var filter in filters )
			{
				var path = filter.Replace(" ", "").Replace( "\\", "_" );
				var savepath = Path.Combine( dataGlobal, $"{Pref.Default.Main_PrefPayload_ItemsSourcePrefix}{path}.tsv");
				// Paylaodのコレクションより先に初期化されるのでPayloadからは読めないので注意

				var ls = payloadTemplates.Where( i => i.TParentDir.ToLower().Contains( filter ) ).ToList();

                if ( path.ToLower().Contains("vae"))
				{
					ls.Add( new PayloadTemplate { TPropertyName = "vae_none", TPropertyValue = "None", TLabel = "None" } );
					ls.Add( new PayloadTemplate { TPropertyName = "vae_automatic", TPropertyValue = "Automatic", TLabel = "Automatic" } );
                    // vaeのNoneとAutomaticを追加
                }

				TsvSerializer.SaveToTsv<PayloadTemplate>
				(
					savepath
					,
					new ObservableCollection<PayloadTemplate>
					(
						ls
					)
				);
			}

			return;

			// 本当はファイル操作系はtry-catchを使うべきなのかもしれない……
			// ただ全部囲むとモデル系はファイル数が数千を超えるケースも珍しくなく、
			// パフォーマンスが悪化しそうなので現在はやっていない

		}


		/// <summary>
		/// Prompt-all-in-oneのYamlタグファイルからTSVとしてソースを取得します。
		/// </summary>
		public void ResetPromptSourceFiles()
		{
            var sdpath = Pref.Default.Main_Path_SD;
            var yamlDir = @"extensions\sd-webui-prompt-all-in-one\group_tags\";
			var yamlfile = CultureInfo.CurrentUICulture.Name.Replace( "-", "_" ) + @".yaml";
            var filepath = Path.Combine( sdpath, yamlDir, yamlfile );
			if (!File.Exists(filepath))
			{
				yamlfile = @"ja_JP.yaml";
                filepath = Path.Combine( sdpath, yamlDir, yamlfile );
            }
            YamlToTsvSerializer.ConvertYamlToTsv( filepath, @".\DataGlobal\ItemsSource_-_prompt.tsv" );
        }


	}








}





