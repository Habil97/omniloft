using Verveo.Entities;
using System.Collections.Generic;

public interface IProductReviewRepository
{
    List<ProductReview> GetByProductId(int productId);
    void Add(ProductReview review);
    void Approve(int reviewId);
    void Delete(int reviewId);
    List<ProductReview> GetAll();
    ProductReview GetById(int id);
    void Update(ProductReview review);
}