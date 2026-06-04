using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASPNET_DX22TT8_TranHoaiThuc_BanGiay.Models
{
    public class VoucherModel
    {
        [Key]        
        public int Id { get; set; }
        public string? Code { get; set; }        
        public DateTime Date { get; set; }
        public int? Status { get; set; }
    }
}
