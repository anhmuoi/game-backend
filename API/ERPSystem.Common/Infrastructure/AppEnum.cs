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
public enum ActionType
{
    Join = 0,
    Out = 1,
    Chat = 2,
    Start = 3,
    End = 4,
    StartFirst = 5,
    ChooseCard = 6,
}
public enum ReactType
{
    Like = 0,
    Heart = 1,
    Sad = 2,
    Smile = 3,
    Hate = 4,
}