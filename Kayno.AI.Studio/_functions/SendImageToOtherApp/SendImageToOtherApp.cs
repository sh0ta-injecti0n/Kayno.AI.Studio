using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kayno.AI.Studio
{

    public class RunningAppInfo
    {
        public string DisplayName { get; set; } // ウィンドウタイトルまたはプロセス名
        public string ProcessPath { get; set; } // .exe のフルパス
    }

    public partial class MainWindow : Window
    {

        /// <summary>
        /// 現在起動中のアプリ一覧
        /// </summary>
        /// <returns></returns>
        public List<RunningAppInfo> GetRunningApps()
        {
            List<RunningAppInfo> apps = new List<RunningAppInfo>();

            // 実行中のプロセスを取得
            Process[] processes = Process.GetProcesses();

            foreach ( Process process in processes )
            {
                try
                {
                    // システムプロセスやアクセス権限がないプロセスはスキップ
                    if ( process.MainWindowTitle == "" && process.ProcessName.StartsWith( "System" ) )
                        continue;

                    // ウィンドウタイトルが空の場合はプロセス名を表示
                    string displayName = string.IsNullOrWhiteSpace( process.MainWindowTitle )
                        ? process.ProcessName
                        : process.MainWindowTitle;

                    // メインの.exeパスを取得
                    string processPath = "";
                    if ( process.MainModule != null )
                    {
                        processPath = process.MainModule.FileName;
                    }

                    if ( !string.IsNullOrEmpty( processPath ) )
                    {
                        apps.Add( new RunningAppInfo
                        {
                            DisplayName = displayName,
                            ProcessPath = processPath
                        } );
                    }
                }
                catch ( Exception )
                {
                    // アクセス権限エラーなどを無視
                }
            }

            return apps.Distinct().ToList(); // 重複を排除
        }

    }
}
