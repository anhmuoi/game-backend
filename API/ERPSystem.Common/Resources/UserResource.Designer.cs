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
    public class UserResource {
        
        private static System.Resources.ResourceManager resourceMan;
        
        private static System.Globalization.CultureInfo resourceCulture;
        
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public UserResource() {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public static System.Resources.ResourceManager ResourceManager {
            get {
                if (object.Equals(null, resourceMan)) {
                    System.Resources.ResourceManager temp = new System.Resources.ResourceManager("ERPSystem.Common.Resources.UserResource", typeof(UserResource).Assembly);
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
        
        public static string msgUserNameExisted {
            get {
                return ResourceManager.GetString("msgUserNameExisted", resourceCulture);
            }
        }
        public static string msgUserPhoneExisted {
            get {
                return ResourceManager.GetString("msgUserPhoneExisted", resourceCulture);
            }
        }
        public static string msgUserEmailExisted {
            get {
                return ResourceManager.GetString("msgUserEmailExisted", resourceCulture);
            }
        }
        
        public static string valid {
            get {
                return ResourceManager.GetString("valid", resourceCulture);
            }
        }
        public static string invalid {
            get {
                return ResourceManager.GetString("invalid", resourceCulture);
            }
        }
        public static string msgCanNotDeleteDepartmentManager {
            get {
                return ResourceManager.GetString("msgCanNotDeleteDepartmentManager", resourceCulture);
            }
        }
    }
}
