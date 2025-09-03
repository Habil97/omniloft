using Verveo.Entities;
using Verveo.DataAccess;
using System.Collections.Generic;

public class ProductReviewService
{
    private readonly IProductReviewRepository _repo;
    public ProductReviewService(IProductReviewRepository repo)
    {
        _repo = repo;
    }

    public List<ProductReview> GetApprovedReviews(int productId)
        => _repo.GetByProductId(productId).Where(r => r.IsApproved).ToList();

    public void AddReview(ProductReview review) => _repo.Add(review);

    public void ApproveReview(int reviewId) => _repo.Approve(reviewId);

    public void DeleteReview(int reviewId) => _repo.Delete(reviewId);

    public List<ProductReview> GetAllPendingReviews()
    {
        return _repo.GetAll().Where(r => !r.IsApproved).ToList();
    }

    public ProductReview GetById(int id)
    {
        return _repo.GetById(id);
    }

    public void UpdateReview(ProductReview review)
    {
        _repo.Update(review);
    }

    public List<ProductReview> GetByUserId(int userId)
    {
        return _repo.GetAll().Where(r => r.UserId == userId).ToList();
    }
}