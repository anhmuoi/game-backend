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
    public class DriverResource {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DriverResource() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("ERPSystem.Common.Resources.DriverResource", typeof(DriverResource).Assembly);
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
        
        public static string msgFolderNameExisted {
            get {
                return ResourceManager.GetString("msgFolderNameExisted", resourceCulture);
            }
        }
        
        public static string msgNotPermissionInFolder {
            get {
                return ResourceManager.GetString("msgNotPermissionInFolder", resourceCulture);
            }
        }
        
        public static string msgNotPermissionDeleteFolder {
            get {
                return ResourceManager.GetString("msgNotPermissionDeleteFolder", resourceCulture);
            }
        }
        
        public static string msgNotPermissionDeleteFile {
            get {
                return ResourceManager.GetString("msgNotPermissionDeleteFile", resourceCulture);
            }
        }
        
        public static string msgFileNameExisted {
            get {
                return ResourceManager.GetString("msgFileNameExisted", resourceCulture);
            }
        }
        
        public static string msgPermissionDenied {
            get {
                return ResourceManager.GetString("msgPermissionDenied", resourceCulture);
            }
        }
        
        public static string lblMyDrive {
            get {
                return ResourceManager.GetString("lblMyDrive", resourceCulture);
            }
        }
        
        public static string lblSharedWithMe {
            get {
                return ResourceManager.GetString("lblSharedWithMe", resourceCulture);
            }
        }
    }
}
