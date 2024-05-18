using AutoMapper;
using ERPSystem.Common;
using ERPSystem.DataAccess.Models;
using ERPSystem.DataModel.Category;
using ERPSystem.Repository;
using Microsoft.Extensions.Logging;
using System.Linq.Dynamic.Core;
using ERPSystem.Common.Infrastructure;

namespace ERPSystem.Service.Services;

public interface ICategoryService
{
    Dictionary<string, object> GetInit();
    bool IsExistedName(string name, int ignoreId);
    bool IsExistedColor(string color, int ignoreId);
    List<CategoryListModel> Gets(CategoryFilterModel filter, out int totalRecords, out int recordsFiltered);
    Category? GetById(int id);
    bool Add(CategoryAddModel model);
    bool Edit(CategoryEditModel model);
    bool Delete(List<int> ids);
}

public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = ApplicationVariables.LoggerFactory.CreateLogger<CategoryService>();
    }

    public bool IsExistedName(string name, int ignoreId)
    {
        var category = _unitOfWork.CategoryRepository.GetByName(name);
        return category != null && category.Id != ignoreId;
    }

    public bool IsExistedColor(string color, int ignoreId)
    {
        var category = _unitOfWork.AppDbContext.Category.FirstOrDefault(m => m.Color.Equals(color));
        return category != null && category.Id != ignoreId;
    }

    public Dictionary<string, object> GetInit()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();
        
        // category type
        var types = EnumHelper.ToEnumList<CategoryType>();
        data.Add("types", types);
        
        return data;
    }

    public List<CategoryListModel> Gets(CategoryFilterModel filter, out int totalRecords, out int recordsFiltered)
    {
        var data = _unitOfWork.CategoryRepository.Gets();
        totalRecords = data.Count();

        if (!string.IsNullOrEmpty(filter.Name))
        {
            data = data.Where(m => m.Name.ToLower().Contains(filter.Name.ToLower()));
        }
        if (filter.Types is { Count: > 0 })
        {
            data = data.Where(m => filter.Types.Contains(m.Type));
        }

        recordsFiltered = data.Count();
        data = data.OrderBy($"{filter.SortColumn} {filter.SortDirection}");
        data = data.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize);
        return data.AsEnumerable().Select(_mapper.Map<CategoryListModel>).ToList();
    }

    public Category? GetById(int id)
    {
        return _unitOfWork.CategoryRepository.GetById(id);
    }

    public bool Add(CategoryAddModel model)
    {
        bool result = false;
        try
        {
            var category = _mapper.Map<Category>(model);
            _unitOfWork.CategoryRepository.Add(category);
            _unitOfWork.Save();
            result = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            result = false;
        }
        
        return result;
    }

    public bool Edit(CategoryEditModel model)
    {
        bool result = false;
        try
        {
            var category = _unitOfWork.CategoryRepository.GetById(model.Id);
            if (category == null)
                throw new Exception($"Can not get category by id = {model.Id}");
            
            _mapper.Map(model, category);
            _unitOfWork.CategoryRepository.Update(category);
            _unitOfWork.Save();
            result = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            result = false;
        }
        
        return result;
    }

    public bool Delete(List<int> ids)
    {
        bool result = false;
        try
        {
            _unitOfWork.CategoryRepository.Delete(m => ids.Contains(m.Id));
            _unitOfWork.Save();
            result = true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message + ex.StackTrace);
            result = false;
        }
        
        return result;
    }
}