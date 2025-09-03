using System.Collections.Generic;
using Verveo.Entities;
using Verveo.DataAccess;
using System.Linq;

namespace Verveo.Services
{
    public class ReturnRequestService
    {
        private readonly IReturnRequestRepository _repo;
        public ReturnRequestService(IReturnRequestRepository repo)
        {
            _repo = repo;
        }

        public void Add(ReturnRequest request) => _repo.Add(request);
        public void Update(ReturnRequest request) => _repo.Update(request);
        public void Delete(int id) => _repo.Delete(id);
        public ReturnRequest GetById(int id) => _repo.GetById(id);
        public IEnumerable<ReturnRequest> GetAll() => _repo.GetAll();
        public IEnumerable<ReturnRequest> GetByUserId(int userId) => _repo.GetByUserId(userId);
        public IEnumerable<ReturnRequest> GetByOrderId(int orderId) => _repo.GetByOrderId(orderId);

        public void CreateRequest(int userId, int orderId, string type, string reason)
        {
            var req = new ReturnRequest
            {
                UserId = userId,
                OrderId = orderId,
                Type = type,
                Reason = reason,
                Status = ReturnStatus.Beklemede,
                CreatedAt = DateTime.Now
            };
            _repo.Add(req);
        }

        public IEnumerable<ReturnRequest> GetUserRequests(int userId) => _repo.GetByUserId(userId);

        public IEnumerable<ReturnRequest> GetAllRequests() => _repo.GetAll();

        public void UpdateStatus(int id, ReturnStatus status)
        {
            var req = _repo.GetAll().FirstOrDefault(r => r.Id == id);
            if (req != null)
            {
                req.Status = status;
                _repo.Update(req);
            }
        }

        public List<ReturnRequest> GetRequestsByUser(int userId)
        {
            return _repo.GetAll()
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }
    }
}