using System.Collections.Generic;
using Verveo.Entities;

namespace Verveo.DataAccess
{
    public interface IReturnRequestRepository
{
    void Add(ReturnRequest request);
    ReturnRequest GetById(int id);
    void Delete(int id);
    IEnumerable<ReturnRequest> GetByOrderId(int orderId);
    IEnumerable<ReturnRequest> GetByUserId(int userId);
    IEnumerable<ReturnRequest> GetAll();
    void Update(ReturnRequest request);
}
}