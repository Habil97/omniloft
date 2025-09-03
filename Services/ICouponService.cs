using Verveo.Entities;

public interface ICouponService
{
    Coupon GetValidCoupon(string code, decimal orderTotal);
    void UseCoupon(int couponId);
}