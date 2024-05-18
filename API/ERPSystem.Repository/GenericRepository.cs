using System.Linq.Expressions;
using ERPSystem.Common.Infrastructure;
using ERPSystem.DataAccess.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ERPSystem.Repository;

public interface IGenericRepository<T> where T : class
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Delete(Expression<Func<T, bool>> where);
    void DeleteRange(IEnumerable<T> entities);
    void DeleteRange(params T[] entities);
    T? GetById(int id);
    T? Get(Expression<Func<T?, bool>> where);
    IQueryable<T> Gets();
    IQueryable<T> Gets(Expression<Func<T, bool>> where);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetMany(Func<T, bool> where);
    int Count(Expression<Func<T, bool>> where);
}

public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly Type _type;
    private readonly HttpContext _httpContext;
    private readonly AppDbContext _dbContext;
    private readonly DbSet<T> _dbSet;

    protected GenericRepository(AppDbContext dbContext, IHttpContextAccessor contextAccessor)
    {
        _dbContext = dbContext;
        _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        _dbSet = _dbContext.Set<T>();
        _httpContext = contextAccessor?.HttpContext ?? new DefaultHttpContext();
        _type = typeof(T);
    }

    public virtual void Add(T entity)
    {
        SetCreated(entity);
        SetUpdated(entity);
        _dbSet.Add(entity);
    }

    public virtual void Update(T entity)
    {
        try
        {
            SetUpdated(entity);
            _dbSet.Attach(entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    public virtual void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual void DeleteRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual void DeleteRange(params T[] entities)
    {
        _dbSet.RemoveRange(entities);
    }

    public virtual void Delete(Expression<Func<T, bool>> where)
    {
        IEnumerable<T> objects = _dbSet.Where(where).AsEnumerable();
        foreach (T obj in objects)
        {
            _dbSet.Remove(obj);
        }
    }

    public virtual T? GetById(int id)
    {
        return _dbSet.Find(id);
    }

    public virtual IQueryable<T> Gets()
    {
        return _dbSet;
    }

    public virtual IQueryable<T> Gets(Expression<Func<T, bool>> where)
    {
        return _dbSet.Where(where);
    }

    public virtual IEnumerable<T> GetAll()
    {
        return _dbSet.AsNoTracking().ToList();
    }

    public virtual IEnumerable<T> GetMany(Func<T, bool> where)
    {
        return _dbSet.AsNoTracking().AsEnumerable().Where(where);
    }

    public virtual T? Get(Expression<Func<T?, bool>> where)
    {
        return _dbSet.AsNoTracking().FirstOrDefault(where);
    }

    public int Count(Expression<Func<T, bool>> where)
    {
        return _dbSet.Count(where);
    }

    private bool HasProperty(string property)
    {
        return _type.GetProperty(property) != null;
    }

    private static void SetProperty(T entity, string property, object? value)
    {
        if(value is not null)
        {
            entity.GetType().GetProperty(property)?.SetValue(entity, int.TryParse(value.ToString(), out int number) ? number : value);
        }
    }

    private void SetCreated(T entity)
    {
        if (HasProperty(Constants.CommonFields.CreatedBy))
        {
            var accountId = _httpContext?.User?.Claims?.FirstOrDefault(m => m.Type == Constants.ClaimName.AccountId)?.Value;

            if(!string.IsNullOrEmpty(accountId))
            {
                SetProperty(entity, Constants.CommonFields.CreatedBy, accountId);
            }
        }
        if (HasProperty(Constants.CommonFields.CreatedOn) && Convert.ToDateTime(entity.GetType().GetProperty(Constants.CommonFields.CreatedOn)?.GetValue(entity)) == DateTime.MinValue)
        {
            SetProperty(entity, Constants.CommonFields.CreatedOn, DateTime.UtcNow);
        }
    }

    private void SetUpdated(T entity) 
    {
        if (HasProperty(Constants.CommonFields.UpdatedBy))
        {
            var accountId = _httpContext?.User?.Claims?.FirstOrDefault(m => m.Type == Constants.ClaimName.AccountId)?.Value;

            if (!string.IsNullOrEmpty(accountId))
            {
                SetProperty(entity, Constants.CommonFields.UpdatedBy, accountId);
            }
        }
        if (HasProperty(Constants.CommonFields.UpdatedOn) && Convert.ToDateTime(entity.GetType().GetProperty(Constants.CommonFields.UpdatedOn)?.GetValue(entity)) == DateTime.MinValue)
        {
            SetProperty(entity, Constants.CommonFields.UpdatedOn, DateTime.UtcNow);
        }
    }
}