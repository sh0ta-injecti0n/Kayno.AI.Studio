﻿//------------------------------------------------------------------------------
// <auto-generated>
//     このコードはツールによって生成されました。
//     ランタイム バージョン:4.0.30319.42000
//
//     このファイルへの変更は、以下の状況下で不正な動作の原因になったり、
//     コードが再生成されるときに損失したりします。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Kayno.AI.Studio.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.12.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("ItemsSource_-_")]
        public string Main_PrefPayload_ItemsSourcePrefix {
            get {
                return ((string)(this["Main_PrefPayload_ItemsSourcePrefix"]));
            }
            set {
                this["Main_PrefPayload_ItemsSourcePrefix"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\DataGlobal\\")]
        public string Main_PrefPayload_DataGlobal {
            get {
                return ((string)(this["Main_PrefPayload_DataGlobal"]));
            }
            set {
                this["Main_PrefPayload_DataGlobal"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\Data\\")]
        public string Main_PrefPayload_Data {
            get {
                return ((string)(this["Main_PrefPayload_Data"]));
            }
            set {
                this["Main_PrefPayload_Data"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Payload.tsv")]
        public string Main_PrefPayload_FileName {
            get {
                return ((string)(this["Main_PrefPayload_FileName"]));
            }
            set {
                this["Main_PrefPayload_FileName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double ScreenCaptureRectX {
            get {
                return ((double)(this["ScreenCaptureRectX"]));
            }
            set {
                this["ScreenCaptureRectX"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double ScreenCaptureRectY {
            get {
                return ((double)(this["ScreenCaptureRectY"]));
            }
            set {
                this["ScreenCaptureRectY"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("512")]
        public double ScreenCaptureRectWidth {
            get {
                return ((double)(this["ScreenCaptureRectWidth"]));
            }
            set {
                this["ScreenCaptureRectWidth"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("512")]
        public double ScreenCaptureRectHeight {
            get {
                return ((double)(this["ScreenCaptureRectHeight"]));
            }
            set {
                this["ScreenCaptureRectHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("models\\stable-diffusion,models\\VAE,models\\lora,models\\controlnet,embeddings")]
        public string Main_Path_SDModelFolderFilters {
            get {
                return ((string)(this["Main_Path_SDModelFolderFilters"]));
            }
            set {
                this["Main_Path_SDModelFolderFilters"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute(".\\extensions\\sd-webui-controlnet\\scripts\\preprocessor")]
        public string Main_Path_SD_CN_Preprocessors {
            get {
                return ((string)(this["Main_Path_SD_CN_Preprocessors"]));
            }
            set {
                this["Main_Path_SD_CN_Preprocessors"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool App_UseSelectedTextGradientColor {
            get {
                return ((bool)(this["App_UseSelectedTextGradientColor"]));
            }
            set {
                this["App_UseSelectedTextGradientColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string Main_Path_SD {
            get {
                return ((string)(this["Main_Path_SD"]));
            }
            set {
                this["Main_Path_SD"] = value;
            }
        }
    }
}
