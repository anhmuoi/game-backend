﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ERPSystem.Common.Resources {
    using System;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class WorkLogResource {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public WorkLogResource() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("ERPSystem.Common.Resources.WorkLogResource", typeof(WorkLogResource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        public static string lblStartDate {
            get {
                return ResourceManager.GetString("lblStartDate", resourceCulture);
            }
        }
        
        public static string lblEndDate {
            get {
                return ResourceManager.GetString("lblEndDate", resourceCulture);
            }
        }
        
        public static string lblTitle {
            get {
                return ResourceManager.GetString("lblTitle", resourceCulture);
            }
        }
        
        public static string msgCanNotDeleteRecordOfAnotherUser {
            get {
                return ResourceManager.GetString("msgCanNotDeleteRecordOfAnotherUser", resourceCulture);
            }
        }
        
        public static string msgCanNotEditRecordOfAnotherUser {
            get {
                return ResourceManager.GetString("msgCanNotEditRecordOfAnotherUser", resourceCulture);
            }
        }
    }
}
