using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppCore.Interfaces;

namespace Infrastructure.Memory;

public class MemoryGenericRepository<T>: IGenericRepositoryAsync<T> 
    where T: class 
{
    protected Dictionary<Guid, T> _data = new();
    
    public Task<T> FindByIdAsync(Guid id)
    {
        var result = _data.TryGetValue(id, out var value) ? value : null;
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> FindAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_data.Values);
    }

    public Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var all = _data.Values.ToList();
        var items = all.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        var totalCount = all.Count;
        var pagedResult = new PagedResult<T>(items, totalCount, page, pageSize);
        return Task.FromResult(pagedResult);
    }

    public Task<T> AddAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
        {
            throw new InvalidOperationException("Entity must have a Guid Id property.");
        }
        
        var id = (Guid)idProperty.GetValue(entity)!;
        if (id == Guid.Empty)
        {
            id = Guid.NewGuid();
            idProperty.SetValue(entity, id);
        }
        
        _data[id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        var idProperty = typeof(T).GetProperty("Id");
        if (idProperty == null || idProperty.PropertyType != typeof(Guid))
        {
            throw new InvalidOperationException("Entity must have a Guid Id property.");
        }
        
        var id = (Guid)idProperty.GetValue(entity)!;
        if (_data.ContainsKey(id))
        {
            _data[id] = entity;
            return Task.FromResult(entity);
        }
        else
        {
            throw new KeyNotFoundException("Entity not found.");
        }
    }

    public Task RemoveByIdAsync(Guid id)
    {
        _data.Remove(id);
        return Task.CompletedTask;
    }
    

}