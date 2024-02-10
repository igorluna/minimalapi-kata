namespace minimal_kata.Data
{
    using minimal_kata.Models;

    public  class CouponStore
    {
        public static List<Coupon> CouponList = new List<Coupon> {
            new Coupon {Id=1, Name="10OFF", Percent = 10, IsActive=true},
            new Coupon {Id=2, Name="20OFF", Percent = 20, IsActive=false}
        };
    }
}