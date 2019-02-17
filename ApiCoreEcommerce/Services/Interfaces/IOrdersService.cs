using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Requests.Orders;
using ApiCoreEcommerce.Entities;

namespace ApiCoreEcommerce.Services.Interfaces
{
    public interface IOrdersService
    {
        Task<Tuple<int, List<Order>>> FetchPageFromUser(ApplicationUser user = null, int page = 1, int pageSize = 5);
        Task<Order> Create(CreateOrderDto form, ApplicationUser user);
        int GetTotalSum(Order order);

        Task<Order> FetchById(long id, bool includeAddress = false, bool includeUser = false,
            bool includeOrderItems = false);

        Task Delete(long id);
        Task<List<Order>> FetchAllFromUserId(long userId);
        Task<Tuple<int, List<Order>>> FetchPageFromUser(long userId, int page = 1, int pageSize = 5);
        void Create(Order order);

    }
}