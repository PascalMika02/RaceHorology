﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RaceHorology.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.4.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("120")]
        public uint AutomaticNiZTimeout {
            get {
                return ((uint)(this["AutomaticNiZTimeout"]));
            }
            set {
                this["AutomaticNiZTimeout"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TimingDevice_Type {
            get {
                return ((string)(this["TimingDevice_Type"]));
            }
            set {
                this["TimingDevice_Type"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string TimingDevice_Port {
            get {
                return ((string)(this["TimingDevice_Port"]));
            }
            set {
                this["TimingDevice_Port"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("192.168.4.1")]
        public string TimingDevice_Url {
            get {
                return ((string)(this["TimingDevice_Url"]));
            }
            set {
                this["TimingDevice_Url"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool TimingDevice_Debug_Dump {
            get {
                return ((bool)(this["TimingDevice_Debug_Dump"]));
            }
            set {
                this["TimingDevice_Debug_Dump"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("9999")]
        public uint AutomaticNaSStarters {
            get {
                return ((uint)(this["AutomaticNaSStarters"]));
            }
            set {
                this["AutomaticNaSStarters"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25")]
        public uint StartTimeIntervall {
            get {
                return ((uint)(this["StartTimeIntervall"]));
            }
            set {
                this["StartTimeIntervall"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string StartNumbersNotToBeAssigned {
            get {
                return ((string)(this["StartNumbersNotToBeAssigned"]));
            }
            set {
                this["StartNumbersNotToBeAssigned"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string HandTiming_LastDevice {
            get {
                return ((string)(this["HandTiming_LastDevice"]));
            }
            set {
                this["HandTiming_LastDevice"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string HandTiming_LastDevicePort {
            get {
                return ((string)(this["HandTiming_LastDevicePort"]));
            }
            set {
                this["HandTiming_LastDevicePort"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool AutoAddParticipants {
            get {
                return ((bool)(this["AutoAddParticipants"]));
            }
            set {
                this["AutoAddParticipants"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("")]
        public string _DataGridColumnVisibility {
            get {
                return ((string)(this["_DataGridColumnVisibility"]));
            }
            set {
                this["_DataGridColumnVisibility"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("False")]
        public bool Timing_DisplayPartcipantAssignment {
            get {
                return ((bool)(this["Timing_DisplayPartcipantAssignment"]));
            }
            set {
                this["Timing_DisplayPartcipantAssignment"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("Stabil")]
        public string UpdateChannel {
            get {
                return ((string)(this["UpdateChannel"]));
            }
            set {
                this["UpdateChannel"] = value;
            }
        }
    }
}
