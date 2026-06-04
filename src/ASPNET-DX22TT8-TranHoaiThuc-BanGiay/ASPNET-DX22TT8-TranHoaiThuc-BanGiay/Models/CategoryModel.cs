using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models
{
    public class CategoryModel
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Không được bỏ trống tên danh mục")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [DefaultValue(1)]
        public int Status { get; set; }
    }
}
