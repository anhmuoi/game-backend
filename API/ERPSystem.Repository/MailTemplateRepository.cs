using System;
using System.Collections.Generic;
using System.Linq;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Repository
{
    public interface IMailTemplateRepository : IGenericRepository<MailTemplate>
    {
        MailTemplate GetMailTemplateByType(int type);
        List<MailTemplate> GetAllMailTemplates();
    }

    public class MailTemplateRepository : GenericRepository<MailTemplate>, IMailTemplateRepository
    {
        private readonly AppDbContext _dbContext;

        public MailTemplateRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor) : base(dbContext, contextAccessor)
        {
            _dbContext = dbContext;
        }

        public MailTemplate GetMailTemplateByType(int type)
        {
            MailTemplate mailTemplate = _dbContext.MailTemplate.FirstOrDefault(m => m.Type == type);
            return mailTemplate;
        }

        public List<MailTemplate> GetAllMailTemplates()
        {
            return _dbContext.MailTemplate.ToList();
        }
    }
}