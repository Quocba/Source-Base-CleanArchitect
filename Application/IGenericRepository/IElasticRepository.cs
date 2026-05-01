using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.IGenericRepository
{
    public interface IElasticRepository<T> where T : class
    {
        Task IndexAsync(T document, string Name);
        Task UpdateAsync(T document, Guid id);
        Task DeleteAsync(Guid id);
        Task<List<T>> SearchAsync(string keyword);
        Task<Tuple<List<T>, long>> SearchPagingAsync(string keyword, int pageNumber, int pageSize);
    }
}
