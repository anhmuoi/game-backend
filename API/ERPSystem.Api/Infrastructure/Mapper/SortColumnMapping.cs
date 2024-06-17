using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Driver;

namespace ERPSystem.Api.Infrastructure.Mapper;

public static class SortColumnMapping
{
    public static string UserColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(User).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string ScheduleColumn(this string sortColumn, string columnDefault = "Title")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(WorkSchedule).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string WorkLogColumn(this string sortColumn, string columnDefault = "Title")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(WorkLog).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string MeetingLogColumn(this string sortColumn, string columnDefault = "Title")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(MeetingLog).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string FolderLogColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(FolderLog).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string DailyReportColumn(this string sortColumn, string columnDefault = "userId")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        if (sortColumn.ToLower() == "departmentname") return sortColumn;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(DailyReport).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    
    public static string CategoryColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(Category).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    
    public static string DepartmentColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(Department).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }

    public static string DocumentListModelColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(DocumentListModel).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string MeetingRoomColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            default: return typeof(DocumentListModel).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }

    public static string UserShareModelColumn(this string sortColumn, string columnDefault = "User.Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "Name": return "User.Name";
            case "Email": return "User.Email";
            case "DepartmentName": return "User.Department.Name";
            default: return typeof(UserFolder).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    
    public static string UserNotShareModelColumn(this string sortColumn, string columnDefault = "User.Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "DepartmentName": return "Department.Name";
            default: return typeof(User).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    
    public static string UserShareFileModelColumn(this string sortColumn, string columnDefault = "User.Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "Name": return "User.Name";
            case "Email": return "User.Email";
            case "DepartmentName": return "User.Department.Name";
            default: return typeof(UserFile).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string ItemNftColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "Name": return "Name";
            default: return typeof(ItemNft).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string ItemNftUserColumn(this string sortColumn, string columnDefault = "Name")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "Name": return "Name";
            default: return typeof(ItemNftUser).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string BalanceHistoryColumn(this string sortColumn, string columnDefault = "CreatedOn")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "CreatedOn": return "CreatedOn";
            default: return typeof(ItemNftUser).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
    public static string FriendUserColumn(this string sortColumn, string columnDefault = "CreatedOn")
    {
        if (string.IsNullOrEmpty(sortColumn)) return columnDefault;
        sortColumn = char.ToUpper(sortColumn[0]) + sortColumn.Substring(1);
        switch (sortColumn)
        {
            case "CreatedOn": return "CreatedOn";
            default: return typeof(FriendUser).GetProperty(sortColumn) == null ? columnDefault : sortColumn;
        }
    }
}