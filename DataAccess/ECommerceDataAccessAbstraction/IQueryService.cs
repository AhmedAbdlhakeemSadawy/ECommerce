using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccessAbstraction
{
    public interface IQueryService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
    }
}
