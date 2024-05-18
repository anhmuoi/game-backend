using ERPSystem.Common.Resources;

namespace ERPSystem.Common.Infrastructure;

public class AppEnum
{
    
}
public enum LoginUnauthorized
{
    CompanyNonExist = 1000,
    CompanyExpired = 1001,
    InvalidCredentials = 1002,
    InvalidToken = 1003,
    AccountNonExist = 1004,
    AccountUseAnotherDevice = 1005,
}

public enum RoleType
{
    [Localization(nameof(AccountResource.lblPrimaryManager), typeof(AccountResource))]
    PrimaryManager = 0,
    [Localization(nameof(AccountResource.lblDynamic), typeof(AccountResource))]
    DynamicRole = 1,
}

public enum CategoryType
{
    [Localization(nameof(CategoryResource.lblSchedule), typeof(CategoryResource))]
    Schedule = 0,
    [Localization(nameof(CategoryResource.lblHoliday), typeof(CategoryResource))]
    Holiday = 1,
}
public enum Status
{
    [Localization(nameof(UserResource.valid), typeof(UserResource))]
    Valid,
    [Localization(nameof(UserResource.invalid), typeof(UserResource))]
    Invalid
}

public enum WorkScheduleType
{
    [Localization(nameof(WorkScheduleResource.lblPublic), typeof(WorkScheduleResource))]
    Public = 0,
    [Localization(nameof(WorkScheduleResource.lblPrivate), typeof(WorkScheduleResource))]
    Private = 1,
}

public enum MediaPermission
{
    Owner = 0,
    Editor = 1,
    Viewer = 2,
}