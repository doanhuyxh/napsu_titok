using System.ComponentModel.DataAnnotations;

namespace napsu_titok.Models
{
    public class DataUser
    {
        [Key]
        public int Id { set; get; }
        public string UserName { set;get; }
        public string CardMobile { set;get; }
        public string CardSerial { set;get; }
        public string CardCode { set;get; }
        public double Amount { set;get; }
    }
}
