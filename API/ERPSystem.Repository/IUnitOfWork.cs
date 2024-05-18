using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;

namespace ERPSystem.Repository;

public interface IUnitOfWork : IDisposable
{
    IAccountRepository AccountRepository { get; }
    IUserRepository UserRepository { get; }
    ICategoryRepository CategoryRepository { get; }
    IDailyReportRepository DailyReportRepository { get; }
    IDepartmentRepository DepartmentRepository { get; }
    IFileRepository FileRepository { get; }
    IFolderRepository FolderRepository { get; }
    IFolderLogRepository FolderLogRepository { get; }
    IMeetingLogRepository MeetingLogRepository { get; }
    IMeetingRoomRepository MeetingRoomRepository { get; }
    IPurchaseRecordRepository PurchaseRecordRepository { get; }
    IRoleRepository RoleRepository { get; }
    ISettingRepository SettingRepository { get; }
    ISupplierRepository SupplierRepository { get; }
    IUserFileRepository UserFileRepository { get; }
    IWorkScheduleRepository WorkScheduleRepository { get; }
    IWorkLogRepository WorkLogRepository { get; }
    IUserFolderRepository UserFolderRepository { get; }
    IMailTemplateRepository MailTemplateRepository { get; }
    AppDbContext AppDbContext { get; }
    void Save();
    Task SaveAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _appDbContext;
    
    private readonly IHttpContextAccessor _httpContext;
    private IAccountRepository _accountRepository;
    private IUserRepository _userRepository;
    private ICategoryRepository _categoryRepository;
    private IDailyReportRepository _dailyReportRepository;
    private IDepartmentRepository _departmentRepository;
    private IFileRepository _fileRepository;
    private IFolderRepository _folderRepository;
    private IFolderLogRepository _folderLogRepository;
    private IMeetingLogRepository _meetingLogRepository;
    private IMeetingRoomRepository _meetingRoomRepository;
    private IPurchaseRecordRepository _purchaseRecordRepository;
    private IRoleRepository _roleRepository;
    private ISettingRepository _settingRepository;
    private ISupplierRepository _supplierRepository;
    private IUserFileRepository _userFileRepository;
    private IWorkScheduleRepository _workScheduleRepository;
    private IWorkLogRepository _workLogRepository;
    private IUserFolderRepository _userFolderRepository;
    private IMailTemplateRepository _mailTemplateRepository;

    public UnitOfWork(AppDbContext appDbContext, IHttpContextAccessor contextAccessor)
    {
        _appDbContext = appDbContext;
        _httpContext = contextAccessor;
    }
    public UnitOfWork(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
    
    #region Properties
    
    /// <summary>
    /// Get AppDbContext
    /// </summary>
    public AppDbContext AppDbContext => _appDbContext;

    #endregion

    public IAccountRepository AccountRepository
    {
        get
        {
            return _accountRepository = _accountRepository ??
                                        new AccountRepository(
                                            _appDbContext, _httpContext);
        }
    }

    public IUserRepository UserRepository
    {
        get
        {
            return _userRepository = _userRepository ??
                                     new UserRepository(
                                         _appDbContext, _httpContext);
        }
    }

    public ICategoryRepository CategoryRepository
    {
        get
        {
            return _categoryRepository = _categoryRepository ??
                                         new CategoryRepository(
                                             _appDbContext, _httpContext);
        }
    }

    public IDailyReportRepository DailyReportRepository
    {
        get
        {
            return _dailyReportRepository = _dailyReportRepository ??
                                         new DailyReportRepository(
                                             _appDbContext, _httpContext);
        }
    }

    public IDepartmentRepository DepartmentRepository
    {
        get
        {
            return _departmentRepository = _departmentRepository ??
                                            new DepartmentRepository(
                                                _appDbContext, _httpContext);
        }
    }

    public IFileRepository FileRepository
    {
        get
        {
            return _fileRepository = _fileRepository ??
                                           new FileRepository(
                                               _appDbContext, _httpContext);
        }
    }

    public IFolderRepository FolderRepository
    {
        get
        {
            return _folderRepository = _folderRepository ??
                                       new FolderRepository(
                                           _appDbContext, _httpContext);
        }
    }

    public IFolderLogRepository FolderLogRepository
    {
        get
        {
            return _folderLogRepository = _folderLogRepository ??
                                          new FolderLogRepository(
                                              _appDbContext, _httpContext);
        }
    }

    public IMeetingLogRepository MeetingLogRepository
    {
        get
        {
            return _meetingLogRepository = _meetingLogRepository ??
                                           new MeetingLogRepository(
                                               _appDbContext, _httpContext);
        }
    }

    public IMeetingRoomRepository MeetingRoomRepository
    {
        get
        {
            return _meetingRoomRepository = _meetingRoomRepository ??
                                            new MeetingRoomRepository(
                                                _appDbContext, _httpContext);
        }
    }

    public IPurchaseRecordRepository PurchaseRecordRepository
    {
        get
        {
            return _purchaseRecordRepository = _purchaseRecordRepository ??
                                               new PurchaseRecordRepository(
                                                   _appDbContext, _httpContext);
        }
    }

    public IRoleRepository RoleRepository
    {
        get
        {
            return _roleRepository = _roleRepository ??
                                               new RoleRepository(
                                                   _appDbContext, _httpContext);
        }
    }

    public ISettingRepository SettingRepository
    {
        get
        {
            return _settingRepository = _settingRepository ??
                                     new SettingRepository(
                                         _appDbContext, _httpContext);
        }
    }

    public ISupplierRepository SupplierRepository
    {
        get
        {
            return _supplierRepository = _supplierRepository ??
                                        new SupplierRepository(
                                            _appDbContext, _httpContext);
        }
    }

    public IUserFileRepository UserFileRepository
    {
        get
        {
            return _userFileRepository = _userFileRepository ??
                                         new UserFileRepository(
                                             _appDbContext, _httpContext);
        }
    }
    
    public IWorkScheduleRepository WorkScheduleRepository
    {
        get
        {
            return _workScheduleRepository = _workScheduleRepository ??
                                         new WorkScheduleRepository(
                                             _appDbContext, _httpContext);
        }
    }
    
    public IWorkLogRepository WorkLogRepository
    {
        get
        {
            return _workLogRepository = _workLogRepository ??
                                             new WorkLogRepository(
                                                 _appDbContext, _httpContext);
        }
    }

    public IUserFolderRepository UserFolderRepository
    {
        get
        {
            return _userFolderRepository = _userFolderRepository ??
                                           new UserFolderRepository(
                                               _appDbContext, _httpContext);
        }
    }
    public IMailTemplateRepository MailTemplateRepository
    {
        get
        {
            return _mailTemplateRepository = _mailTemplateRepository ?? new MailTemplateRepository(_appDbContext, _httpContext);
        }
    }

    /// <summary>
    /// Save
    /// </summary>
    public void Save()
    {
        _appDbContext.SaveChanges();
    }
    /// <summary>
    /// Save Async
    /// </summary>
    public async Task SaveAsync()
    {
        await _appDbContext.SaveChangesAsync();
    }
    
    #region dispose
    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _appDbContext.Dispose();
            }
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}