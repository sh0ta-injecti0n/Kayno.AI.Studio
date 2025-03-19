using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Kayno.AI.Studio
{
     public class RootObject
    {
        public string name { get; set; }
        public List<groups> groups { get; set; }
    }

    public class Item
    {
        public string name { get; set; }
        public List<groups> groups { get; set; }
    }

    public class groups
    {
        public string name { get; set; }
        public string color { get; set; }
        public string type { get; set; }
        // まれにtypeとかいうの出てくるのでいないと変換できずエラーになる
        public Dictionary<string, string> tags { get; set; }
    }

    public class YamlToTsvSerializer
    {
       
        public static void ConvertYamlToTsv( string yamlFilePath, string tsvFilePath )
        {
            if ( string.IsNullOrEmpty( yamlFilePath ) )
            {
                var dialog = new OpenFileDialog();
                var res = dialog.ShowDialog();
                yamlFilePath = res == true ? dialog.FileName : "";
            }

            if ( !File.Exists( yamlFilePath ) )
            {
                return;
            }

            // YAMLを読み込んでデシリアライズ
            var deserializer = new DeserializerBuilder().Build();
            var yaml = File.ReadAllText( yamlFilePath );  // YAMLファイルの読み込み
            var items = deserializer.Deserialize<List<Item>>( yaml );

            StringBuilder tsvData = new StringBuilder();
            //tsvData.AppendLine( "name\tgroups\tcolor\ttagsKey\ttagsValue" );
            //tsvData.AppendLine( "TPropertyName\tTPropertyValue\tTLabel\tTCategory\tTCategory2\tTComment" );
            var cols = "TPropertyName\tTPropertyValue\tTThumbPath\tTLabel\tTCategory\tTCategory2\tTPath\tTParentDir\tTTags\tTDescription\tTParameter\tTComment";
            tsvData.AppendLine( cols );
            // tagsKey  tagskey tagsVal name    group  color
            // Name     Value   Label   cate    cate2   comment

            var n = "";
            foreach ( var item in items )
            {
                foreach ( var group in item.groups )
                {
                    if ( group.tags == null) continue;
                    foreach ( var tag in group.tags )
                    {
                        //var cols = "TPropertyName\tTPropertyValue\tTThumbPath\tTLabel\tTCategory\tTCategory2\tTPath\tTParentDir\tTTags\tTDescription\tTParameter\tTComment";
                        var data = $"{tag.Key}\t{tag.Key}\t{n}\t{tag.Value}\t{item.name}\t{group.name}\t{n}\t{n}\t{n}\t{n}\t{n}\t{group.color}";
                        //tsvData.AppendLine( $"{tag.Key}\t{tag.Key}\t{n}\t{tag.Value}\t{item.name}\t{group.name}\t{group.color}" );
                        tsvData.AppendLine( data );
                    }
                }
            }

            File.WriteAllText( tsvFilePath, tsvData.ToString() );
        }


    }
}
