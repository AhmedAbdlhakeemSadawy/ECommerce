using ECommerceDataAccessDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceDataAccessAbstraction
{
    public interface IOrderRepository<TOrderDataDto> : IRepository<TOrderDataDto> where TOrderDataDto : OrderDataDto
    {
    }
}
