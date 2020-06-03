using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace homestuff.interfaces
{

    public interface IRepository<T>
    {
        Task<T> GetOneAsync(string id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T obj);
    }
}
