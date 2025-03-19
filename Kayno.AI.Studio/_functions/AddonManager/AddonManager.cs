using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using IronPython.Hosting;
using Microsoft.Scripting.Hosting;

namespace Kayno.AI.Studio
{
    public partial class MainWindow: Window
    {

        public ScriptEngine ScriptEngine { get; set; }
        public ScriptScope ScriptScope { get; set; }

        public void InitPythonEngine()
        {
            ScriptEngine = Python.CreateEngine();
            ScriptScope = ScriptEngine.CreateScope();
        }

        public void ExecutePythonScript( string filepath )
        {
            if ( !File.Exists( filepath ) )
            {
                return;
            }

            try
            {
                ScriptEngine = Python.CreateEngine();
                ScriptScope = ScriptEngine.CreateScope();
                ScriptEngine.ExecuteFile( filepath, ScriptScope );

                //dynamic addon = ScriptScope.GetVariable( "addon" );
                //addon.Execute();
            }
            catch ( Exception ex )
            {
                Console.WriteLine( $"エラー: {ex.Message}" );
            }
        }


    }







}
