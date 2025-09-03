using Verveo.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Verveo.DataAccess
{
    public class EfProductReviewRepository : IProductReviewRepository
    {
        private readonly VerveoDbContext _db;
        public EfProductReviewRepository(VerveoDbContext db)
        {
            _db = db;
        }

        public List<ProductReview> GetAll() => _db.ProductReviews.ToList();

        public List<ProductReview> GetByProductId(int productId)
            => _db.ProductReviews.Where(r => r.ProductId == productId).ToList();

        public void Add(ProductReview review)
        {
            _db.ProductReviews.Add(review);
            _db.SaveChanges();
        }

        public void Approve(int reviewId)
        {
            var review = _db.ProductReviews.FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                review.IsApproved = true;
                _db.SaveChanges();
            }
        }

        public void Delete(int reviewId)
        {
            var review = _db.ProductReviews.FirstOrDefault(r => r.Id == reviewId);
            if (review != null)
            {
                _db.ProductReviews.Remove(review);
                _db.SaveChanges();
            }
        }

        public ProductReview GetById(int id)
        {
            return _db.ProductReviews.FirstOrDefault(r => r.Id == id);
        }

        public void Update(ProductReview review)
        {
            _db.ProductReviews.Update(review);
            _db.SaveChanges();
        }
    }
}