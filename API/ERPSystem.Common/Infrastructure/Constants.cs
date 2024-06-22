namespace ERPSystem.Common.Infrastructure;

public class Constants
{
    public const string Description = "Description";
    public const string WorkLogKey = "work";
    public const string MeetingLogKey = "meeting";

    public class Protocol
    { 
        public const string Notification = "NOTIFICATION";
    }
    public class RabbitMqConfig
    {
        public const int MaxChannelsPerConnection = 2000;
        public const string ExchangeName = "amq.topic";
        public const int MessageTimeToLive = 600000;

        public const string TopicExportMember = ".topic.export_member";
        public const string ExportDataToFile = "export_data_to_file";

        public const string TypeExportMember = "EXPORT_MEMBER";
        public const string NotificationTopic = ".topic.notification";

    }
    public class Settings
    {
        public const string JwtSection = "Jwt";
        public const string DateTimeServerFormat = "DateTimeServerFormat";
        public const string DefaultConnection = "DefaultConnection";
        public const string ERPSystemDataAccess = "ERPSystem.DataAccess";
        public const string DefaultEnvironmentConnection = "ConnectionStrings__DefaultConnection";
        public const string DefineQueueConnectionSettings = "QueueConnectionSettings";

        public const string MailSettings = "MailSettings";
        public const string MailDevelopSettings = "MailDevelopSettings";
        public const string AppSettingsPort = "AppSettings:Port";
        public const string ResourcesDir = "Resources";
        public const string DefineHostApiConfig = "ApiConfig:Host";
        public const string DateTimeFormatDefault = "MM.dd.yyyy HH:mm:ss";
        public const string DateFormatDefault = "MM.dd.yyyy";
        public const string TimeFormatDefault = "HH:mm";
        public const string DateTimeUrlFormatDefault = "yyyy-MM-dd HH:mm:ss";
        public const string DefaultAccountUsername = "DefaultAccount:Username";
        public const string DefaultAccountPassword = "DefaultAccount:Password";
        public const string DefineQueueSetting = "DefineQueueConnectionSettings";
        public const string QueueConnectionSettingsHost = "QueueConnectionSettings:Host";
        public const string QueueConnectionSettingsVirtualHost = "QueueConnectionSettings:VirtualHost";
        public const string QueueConnectionSettingsPort = "QueueConnectionSettings:Port";
        public const string QueueConnectionSettingsUserName = "QueueConnectionSettings:UserName";
        public const string QueueConnectionSettingsPassword = "QueueConnectionSettings:Password";
        public const string QueueEnvironmentConnectionSettingsHost = "QueueConnectionSettings__Host";
        public const string QueueEnvironmentConnectionSettingsVirtualHost = "QueueConnectionSettings__VirtualHost";
        public const string QueueEnvironmentConnectionSettingsPort = "QueueConnectionSettings__Port";
        public const string QueueEnvironmentConnectionSettingsUserName = "QueueConnectionSettings__UserName";
        public const string QueueEnvironmentConnectionSettingsPassword = "QueueConnectionSettings__Password";
    }
    public class Policy
    {
        public const string PrimaryAdmin = "PrimaryAdmin";
        public const string Dynamic = "Dynamic";
        public const string PrimaryAdminAndDynamic = "PrimaryAdminAndDynamic";
    }

    public class CommonFields
    {
        public const string CreatedBy = "CreatedBy";
        public const string CreatedOn = "CreatedOn";
        public const string UpdatedBy = "UpdatedBy";
        public const string UpdatedOn = "UpdatedOn";
    }

    public class ClaimName
    {
        public const string UserName = "UserName";
        public const string AccountId = "AccountId";
        public const string UserId = "UserId";
        public const string Role = "Role";
        public const string Timezone = "Timezone";
        public const string Language = "Language";
    }

    public class Swagger
    {
        public const string V1 = "v1";
        public const string CorsPolicy = "CorsPolicy";
        public const string Title = "Duali ERP services - Duali ERP HTTP API";

        public const string Description =
            "Swagger aides in development across the entire API lifecycle, from design and documentation, to test and deployment.";
    }
    

    public class Route
    {
        public const string ApiDashboard = "/dashboard";
        
        public const string ApiLogin = "/login";
        public const string ApiLoginByAddress = "/login-by-address";
        public const string ApiRefreshToken = "/refreshToken";
        public const string ApiResetPassword = "/accounts/reset-password";
        public const string ApiChangePasswordNoLogin = "/accounts/change-password-no-login";
        public const string ApiForgotPassword = "/accounts/forgot-password";
        public const string ApiChangePassword = "/accounts/change-password";

        public const string ApiDriverInit = "/driver/init";
        public const string ApiFolders = "/folders";
        public const string ApiFoldersId = "/folders/{id}";
        public const string ApiFoldersIdDocument = "/folders/{id}/document";
        public const string ApiFoldersIdDownload = "/folders/{id}/download";
        public const string ApiFoldersIdShare = "/folders/{id}/share";
        public const string ApiFoldersIdNotShare = "/folders/{id}/not-share";
        public const string ApiFiles = "/files";
        public const string ApiFilesId = "/files/{id}";
        public const string ApiFilesIdDownload = "/files/{id}/download";
        public const string ApiFilesIdShare = "/files/{id}/share";
        public const string ApiFilesIdNotShare = "/files/{id}/not-share";
        public const string ApiDriverSharedWithMe = "/driver/shared-with-me";
        
        public const string ApiRoles = "/roles";
        public const string ApiRolesId = "/roles/{id}";
        public const string ApiDefaultRoles = "/roles/default";
        public const string ApiChangeRoleSettingDefault = "roles/{id}/default-setting";
        public const string ApiRolesMyPermission = "/roles/my-permission";

        public const string ApiCategoriesInit = "/categories/init";
        public const string ApiCategories = "/categories";
        public const string ApiCategoriesId = "/categories/{id}";
        
        public const string ApiDepartments = "/departments";
        public const string ApiDepartmentsId = "/departments/{id}";
        public const string ApiDepartmentsIdJoinGroup = "/departments/{id}/join-group";
        public const string ApiDepartmentsIdConfirmJoinGroup = "/departments/{id}/confirm-join";
        public const string ApiDepartmentsIdAssign = "/departments/{id}/assign";
        public const string ApiDepartmentsIdUnAssign = "/departments/{id}/un-assign";
        public const string ApiDepartmentsTree = "/departments-tree";

        public const string ApiWorkSchedulesInit = "/schedules/init";
        public const string ApiWorkSchedules = "/schedules";
        public const string ApiWorkSchedulesId = "/schedules/{id}";
        
        public const string ApiWorkLogs = "/work-logs";
        public const string ApiWorkLogsId = "/work-logs/{id}";
        public const string ApiMeetingLogs = "/meeting-logs";
        public const string ApiMeetingLogsId = "/meeting-logs/{id}";
        public const string ApiMeetingRooms = "/meeting-rooms";
        public const string ApiMeetingRoomsId = "/meeting-rooms/{id}";
        
        public const string ApiDailyReportsInit = "/daily-reports/init";
        public const string ApiDailyReports = "/daily-reports";
        public const string ApiDailyReportsId = "/daily-reports/{id}";
        public const string ApiDailyReportsByUserIdAndDate = "/daily-reports/by-user";
        
        public const string ApiFolderLogInit = "/folder-logs/init";
        public const string ApiFolderLogSearch = "/folder-logs/search";
        public const string ApiFolderLogs = "/folder-logs";
        public const string ApiFolderLogsId = "/folder-logs/{id}";
        public const string ApiFolderLogsIds = "/folder-logs/multi";

        public const string ApiUsersInit = "/users/init";
        public const string ApiUsers = "/users";
        public const string ApiUsersId = "/users/{id}";
        public const string ApiUsersBalance = "/users/update-balance";
        public const string ApiUsersOutGroup = "/users/{id}/out-group";
        public const string ApiAccountProfile = "/accounts-profile";
        public const string ApiAccountsAvatar = "/accounts/avatar";
        public const string ErrorPage = "/error/systemerror";
        public const string ApiImageStatic = "/static/images/{rootFolder}/{fileName}";

        public const string ApiItemNfts = "/item-nfts";
        public const string ApiItemNftsId = "/item-nfts/{id}";
        public const string ApiFriendUsers = "/friends";
        public const string ApiFriendUsersId = "/friends/{id}";
        public const string ApiFriendsIdAddFriend = "/friends/add-friend";
        public const string ApiFriendsIdConfirmAddFriend = "/friends/confirm-add";

        public const string ApiBalanceHistorys = "/balances";
        public const string ApiBalanceHistorysChart = "/balances/chart";
        public const string ApiBalanceHistorysId = "/balances/{id}";

        public const string ApiItemNftUsers = "/item-nft-users";
        public const string ApiItemNftUsersId = "/item-nft-users/{id}";
        public const string ApiItemNftUsersUse = "/item-nft-users/use";
        public const string ApiAssignItemNftUsers = "/item-nft-users/assign";
    }
    /// <summary>
    /// Config for logger system
    /// </summary>
    public class Logger
    {
        public const string LogFile = "Logging:LogDir";
        public const string Logging = "Logging";
        public const string FileDebugChannel = "debug_channel.log";
    }
    public class Message
    {
        public const string RefreshTokenSuccess = "Refresh Token Success";
    }
    public class ImageConfig
    {
        public const string BaseFolderUser = "images/user";
        public const string BaseFolderItemNft = "images/nft";
    }
    public class MediaConfig
    {
        public const string BaseFolderData = "data";

        public const string FolderAliasId = "folder";
        public const string FileAliasId = "file";
        public const string SplitText = "-";
    }

    public enum TypeEmailTemplate
    {
        WelcomeUserEmail = 5,
    }
    public enum VariableEmailTemplate
    {
      
        LogoImageId,
        ImageQRCode,
        AndroidIcon,
        IosIcon,
        UserNameWelcome,
        EmailWelcome,
        Token,
        UserName,
        PassCode,
        PasswordDefault,
        FireCrackerIcon,
        GetItOnGooglePlayIcon,
        DownloadOnAppStoreIcon,
        CompanyLogo,
        TimeIcon,
        LocationIcon,
        DoorIcon,
        PhoneIcon,
        ContactVisitTarget,
        
    }
    public class Image 
    {
        public const string DefaultHeaderPng = "data:image/png;base64,";
    }
    public class Link
    {
        public const string Android_Download = "https://play.google.com/store/apps/details?id=com.demasterpro";
        public const string IOS_Download = "https://apps.apple.com/us/app/id1500403553";
    }

    #region HtmlDecToEntityMap

    public static readonly Dictionary<string, string> HtmlDecToEntityMap = new Dictionary<string, string>()
    {
        {"&amp;", "&#38;"},
        {"&lt;", "&#60;"},
        {"&gt;", "&#62;"},
        {"&nbsp;", "&#160;"},
        {"&iexcl;", "&#161;"},
        // {"&cent;", "&#162;"},
        // {"&pound;", "&#163;"},
        // {"&curren;", "&#164;"},
        // {"&yen;", "&#165;"},
        // {"&brvbar;", "&#166;"},
        // {"&sect;", "&#167;"},
        // {"&uml;", "&#168;"},
        // {"&copy;", "&#169;"},
        // {"&ordf;", "&#170;"},
        // {"&laquo;", "&#171;"},
        // {"&not;", "&#172;"},
        // {"&shy;", "&#173;"},
        // {"&reg;", "&#174;"},
        // {"&macr;", "&#175;"},
        // {"&deg;", "&#176;"},
        // {"&plusmn;", "&#177;"},
        // {"&sup2;", "&#178;"},
        // {"&sup3;", "&#179;"},
        // {"&acute;", "&#180;"},
        // {"&micro;", "&#181;"},
        // {"&para;", "&#182;"},
        // {"&middot;", "&#183;"},
        // {"&cedil;", "&#184;"},
        // {"&sup1;", "&#185;"},
        // {"&ordm;", "&#186;"},
        // {"&raquo;", "&#187;"},
        // {"&frac14;", "&#188;"},
        // {"&frac12;", "&#189;"},
        // {"&frac34;", "&#190;"},
        // {"&iquest;", "&#191;"},
        {"&Agrave;", "&#192;"},
        {"&Aacute;", "&#193;"},
        {"&Acirc;", "&#194;"},
        {"&Atilde;", "&#195;"},
        {"&Auml;", "&#196;"},
        {"&Aring;", "&#197;"},
        {"&AElig;", "&#198;"},
        {"&Ccedil;", "&#199;"},
        {"&Egrave;", "&#200;"},
        {"&Eacute;", "&#201;"},
        {"&Ecirc;", "&#202;"},
        {"&Euml;", "&#203;"},
        {"&Igrave;", "&#204;"},
        {"&Iacute;", "&#205;"},
        {"&Icirc;", "&#206;"},
        {"&Iuml;", "&#207;"},
        {"&ETH;", "&#208;"},
        {"&Ntilde;", "&#209;"},
        {"&Ograve;", "&#210;"},
        {"&Oacute;", "&#211;"},
        {"&Ocirc;", "&#212;"},
        {"&Otilde;", "&#213;"},
        {"&Ouml;", "&#214;"},
        {"&times;", "&#215;"},
        {"&Oslash;", "&#216;"},
        {"&Ugrave;", "&#217;"},
        {"&Uacute;", "&#218;"},
        {"&Ucirc;", "&#219;"},
        {"&Uuml;", "&#220;"},
        {"&Yacute;", "&#221;"},
        {"&THORN;", "&#222;"},
        {"&szlig;", "&#223;"},
        {"&agrave;", "&#224;"},
        {"&aacute;", "&#225;"},
        {"&acirc;", "&#226;"},
        {"&atilde;", "&#227;"},
        {"&auml;", "&#228;"},
        {"&aring;", "&#229;"},
        {"&aelig;", "&#230;"},
        {"&ccedil;", "&#231;"},
        {"&egrave;", "&#232;"},
        {"&eacute;", "&#233;"},
        {"&ecirc;", "&#234;"},
        {"&euml;", "&#235;"},
        {"&igrave;", "&#236;"},
        {"&iacute;", "&#237;"},
        {"&icirc;", "&#238;"},
        {"&iuml;", "&#239;"},
        {"&eth;", "&#240;"},
        {"&ntilde;", "&#241;"},
        {"&ograve;", "&#242;"},
        {"&oacute;", "&#243;"},
        {"&ocirc;", "&#244;"},
        {"&otilde;", "&#245;"},
        {"&ouml;", "&#246;"},
        // {"&divide;", "&#247;"},
        // {"&oslash;", "&#248;"},
        {"&ugrave;", "&#249;"},
        {"&uacute;", "&#250;"},
        {"&ucirc;", "&#251;"},
        {"&uuml;", "&#252;"},
        {"&yacute;", "&#253;"},
        // {"&thorn;", "&#254;"},
        // {"&yuml;", "&#255;"},
        // {"&fnof;", "&#402;"},
        {"&Alpha;", "&#913;"},
        {"&Beta;", "&#914;"},
        // {"&Gamma;", "&#915;"},
        {"&Delta;", "&#916;"},
        {"&Epsilon;", "&#917;"},
        {"&Zeta;", "&#918;"},
        {"&Eta;", "&#919;"},
        // {"&Theta;", "&#920;"},
        {"&Iota;", "&#921;"},
        {"&Kappa;", "&#922;"},
        // {"&Lambda;", "&#923;"},
        {"&Mu;", "&#924;"},
        {"&Nu;", "&#925;"},
        // {"&Xi;", "&#926;"},
        {"&Omicron;", "&#927;"},
        // {"&Pi;", "&#928;"},
        {"&Rho;", "&#929;"},
        // {"&Sigma;", "&#931;"},
        {"&Tau;", "&#932;"},
        {"&Upsilon;", "&#933;"},
        // {"&Phi;", "&#934;"},
        // {"&Chi;", "&#935;"},
        // {"&Psi;", "&#936;"},
        // {"&Omega;", "&#937;"},
        // {"&alpha;", "&#945;"},
        // {"&beta;", "&#946;"},
        {"&gamma;", "&#947;"},
        // {"&delta;", "&#948;"},
        // {"&epsilon;", "&#949;"},
        // {"&zeta;", "&#950;"},
        // {"&eta;", "&#951;"},
        // {"&theta;", "&#952;"},
        // {"&iota;", "&#953;"},
        {"&kappa;", "&#954;"},
        // {"&lambda;", "&#955;"},
        // {"&mu;", "&#956;"},
        // {"&nu;", "&#957;"},
        // {"&xi;", "&#958;"},
        {"&omicron;", "&#959;"},
        // {"&pi;", "&#960;"},
        // {"&rho;", "&#961;"},
        // {"&sigmaf;", "&#962;"},
        // {"&sigma;", "&#963;"},
        // {"&tau;", "&#964;"},
        // {"&upsilon;", "&#965;"},
        // {"&phi;", "&#966;"},
        // {"&chi;", "&#967;"},
        // {"&psi;", "&#968;"},
        // {"&omega;", "&#969;"},
        // {"&thetasym;", "&#977;"},
        // {"&upsih;", "&#978;"},
        // {"&piv;", "&#982;"}
    };

    #endregion
    
}