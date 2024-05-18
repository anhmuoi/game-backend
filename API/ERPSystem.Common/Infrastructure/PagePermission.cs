using System.Reflection;

namespace ERPSystem.Common.Infrastructure;

public class PagePermission
{
    public static Dictionary<string, Dictionary<string, bool>> GetAllPermssions()
    {
        Dictionary<string, Dictionary<string, bool>>
            permissionList = new Dictionary<string, Dictionary<string, bool>>();

        #region Add permission to Dictionary

        // Add user page permission
        permissionList.Add(new UserPage().GetPageName(), new UserPage().GetPermissions());
        // Add Category Page permission
        permissionList.Add(new CategoryPage().GetPageName(), new CategoryPage().GetPermissions());
        // Add Department Page permission
        permissionList.Add(new DepartmentPage().GetPageName(), new DepartmentPage().GetPermissions());
        // Add DailyReport Page permission
        permissionList.Add(new DailyReportPage().GetPageName(), new DailyReportPage().GetPermissions());
        // Add Folder Page permission
        permissionList.Add(new FolderPage().GetPageName(), new FolderPage().GetPermissions());
        // Add Meeting Page permission
        permissionList.Add(new MeetingPage().GetPageName(), new MeetingPage().GetPermissions());
        // Add Purchase Record Page permission
        permissionList.Add(new PurchaseRecordPage().GetPageName(), new PurchaseRecordPage().GetPermissions());
        // Add Role Page permission
        permissionList.Add(new RolePage().GetPageName(), new RolePage().GetPermissions());
        // Add Setting Page permission
        permissionList.Add(new SettingPage().GetPageName(), new SettingPage().GetPermissions());
        // Add Supplier Page permission
        permissionList.Add(new SupplierPage().GetPageName(), new SupplierPage().GetPermissions());
        // Add Schedule Page permission
        permissionList.Add(new SchedulePage().GetPageName(), new SchedulePage().GetPermissions());
        // Add WorkLog Page permission
        permissionList.Add(new WorkLogPage().GetPageName(), new WorkLogPage().GetPermissions());
        // Add Barcode Page permission
        permissionList.Add(new BarcodePage().GetPageName(), new BarcodePage().GetPermissions());

        #endregion

        return permissionList;
    }
    public class Page
    {
        #region page list

        public const string User = "User";
        public const string Category = "Category";
        public const string DailyReport = "DailyReport";
        public const string Department = "Department";
        public const string Folder = "Folder";
        public const string Meeting = "Meeting";
        public const string PurchaseRecord = "PurchaseRecord";
        public const string Role = "Role";
        public const string Setting = "Setting";
        public const string Supplier = "Supplier";
        public const string WorkLog = "WorkLog";
        public const string Schedule = "Schedule";
        public const string Barcode = "Barcode";

        #endregion
        public string GetValue(string key)
        {
            return GetType().GetField(key).GetValue("").ToString();
        }
    }
    public class ActionName
    {
        #region action list

        public const string View = "View";
        public const string Add = "Add";
        public const string Edit = "Edit";
        public const string Delete = "Delete";

        #endregion
    }
    public class PagePermissionClass
    {
        public Dictionary<string, bool> GetPermissions()
        {
            var pageName = this.GetType().Name.Replace("Page", "");

            foreach(FieldInfo fi in typeof(Page).GetFields())
            {
                if (fi.Name == pageName)
                {
                    pageName = fi.GetValue((Object)fi.Name).ToString();
                    break;
                }
            }

            Dictionary<string, bool> permissions = new Dictionary<string, bool>();
            
            foreach (FieldInfo fi in GetType().GetFields())
            {
                permissions.Add(fi.Name + pageName, Convert.ToBoolean(fi.GetValue((object)fi.Name).ToString()));
            }
            
            return permissions;
        }

        public string GetPageName()
        {
            var pageName = this.GetType().Name.Replace("Page", "");

            return pageName;
        }

        public string GetPermission(string action)
        {
            var pageName = this.GetType().Name.Replace("Page", "");

            foreach (FieldInfo fi in GetType().GetFields())
            {
                if (fi.Name == action)
                    return fi.Name + new Page().GetValue(pageName);
            }

            return null;
        }
    }
    public class UserPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class SchedulePage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class CategoryPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class DailyReportPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class DepartmentPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class FolderPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class MeetingPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class PurchaseRecordPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class RolePage : PagePermissionClass
    {
        public const bool View = false;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class SettingPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class SupplierPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = false;
        public const bool Edit = false;
        public const bool Delete = false;
    }
    public class WorkLogPage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = true;
        public const bool Edit = true;
        public const bool Delete = true;
    }
    public class BarcodePage : PagePermissionClass
    {
        public const bool View = true;
        public const bool Add = true;
        public const bool Edit = true;
        public const bool Delete = true;
    }
}