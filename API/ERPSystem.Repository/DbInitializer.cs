using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ERPSystem.Repository;

public class DbInitializer
{
    public static void Initialize(IUnitOfWork unitOfWork, IConfiguration configuration)
    {
        //Database migrate
        unitOfWork.AppDbContext.Database.Migrate();
        
        // Add default role
        unitOfWork.RoleRepository.AddDefaultRole();
        unitOfWork.Save();
        
        // Add default account
        var account = unitOfWork.AccountRepository.AddDefaultAccount(
            configuration[Constants.Settings.DefaultAccountUsername],
            configuration[Constants.Settings.DefaultAccountPassword]);
        unitOfWork.Save();
        
        // Add default user
        if (unitOfWork.AppDbContext.User.FirstOrDefault(m => m.AccountId == account.Id) == null)
        {
            unitOfWork.UserRepository.Add(new User()
            {
                Name = account.UserName,
                AccountId = account.Id,
                Email = account.UserName,
            });
            unitOfWork.Save();
        }
        
        // init folder default for user
        var rootFolderIds = unitOfWork.AppDbContext.Folder.Include(m => m.UserFolder)
            .Where(m => !m.ParentId.HasValue).Select(m => m.Id).AsEnumerable();
        var userAlreadyRootFolder = unitOfWork.AppDbContext.UserFolder.Where(m => rootFolderIds.Contains(m.FolderId)).Select(m => m.UserId).AsEnumerable();
        
        var userIdsNeedCreateRootFolder = unitOfWork.UserRepository.Gets().Where(m => !userAlreadyRootFolder.Contains(m.Id)).Select(m => m.Id).AsEnumerable();
        foreach (int userId in userIdsNeedCreateRootFolder)
        {
            // create folder in server
            bool result = FileHelpers.CreateFolder($"{Constants.MediaConfig.BaseFolderData}/{userId}");
            if (result)
            {
                // add folder
                var folderItem = new Folder()
                {
                    Name = $"{userId}",
                    Description = $"{userId}",
                };
                unitOfWork.FolderRepository.Add(folderItem);
                unitOfWork.Save();
            
                // add permission
                unitOfWork.UserFolderRepository.Add(new UserFolder()
                {
                    UserId = userId,
                    FolderId = folderItem.Id,
                    PermissionType = (int)MediaPermission.Owner,
                });
                unitOfWork.Save();
            }
        }
    }
}