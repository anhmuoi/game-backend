using System.IO.Compression;
using System.Text;
using FluentScheduler;
using ERPSystem.Common;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using ERPSystem.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ERPSystem.Service;

public class CronjobService : Registry
{
    public CronjobService(IConfiguration configuration)
    {
        // Every Day
        Schedule(() => new AutoAddDailyReport(configuration)).ToRunEvery(1).Days().At(0, 0); // 0h00
    }
}

public class AutoAddDailyReport : IJob
{
    private readonly IConfiguration _configuration;
    
    public AutoAddDailyReport(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public void Execute()
    {
        IUnitOfWork unitOfWork = HelperService.CreateUnitOfWork(_configuration);
        try
        {
      
            
            var userList = unitOfWork.UserRepository.GetAll().ToList();
            foreach(var user in userList)
            {
                DailyReport dailyReport = new DailyReport();
                dailyReport.UserId = user.Id;
                dailyReport.Title = "";
                dailyReport.Content = "";
                dailyReport.Date = DateTime.UtcNow;
                dailyReport.CreatedBy = user.AccountId.Value;
                dailyReport.UpdatedBy = user.AccountId.Value;
                
                unitOfWork.DailyReportRepository.Add(dailyReport);
                unitOfWork.Save();
            }
            
          
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            unitOfWork.Dispose();
        }
    }
}

