using System;
using System.Linq;
using Verveo.Entities;
using Verveo.DataAccess;

public class CouponService : ICouponService
{
    private readonly VerveoDbContext _db;

    public CouponService(VerveoDbContext db)
    {
        _db = db;
    }

    public Coupon GetValidCoupon(string code, decimal orderTotal)
    {
        var now = DateTime.Now;
        return _db.Coupons.FirstOrDefault(c =>
            c.Code == code &&
            c.IsActive &&
            c.ValidFrom <= now &&
            c.ValidTo >= now &&
            (c.MinimumOrderAmount == null || orderTotal >= c.MinimumOrderAmount) &&
            (c.UsageLimit == null || c.UsedCount < c.UsageLimit)
        );
    }

    public void UseCoupon(int couponId)
    {
        var coupon = _db.Coupons.Find(couponId);
        if (coupon != null)
        {
            coupon.UsedCount++;
            _db.SaveChanges();
        }
    }
}