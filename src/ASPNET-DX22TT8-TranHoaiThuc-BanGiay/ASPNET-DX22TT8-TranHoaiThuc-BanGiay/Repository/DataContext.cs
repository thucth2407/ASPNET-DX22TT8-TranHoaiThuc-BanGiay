using ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Repository
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<ProductModel> Products { get; set; }        
        public DbSet<UserModel> Users { get; set; }
        public DbSet<VoucherModel> Voucher { get; set; }
        public DbSet<OrderModel> Orders { get; set; }
        public DbSet<OrderDetailModel> OrderDetails { get; set; }        
    }
}
