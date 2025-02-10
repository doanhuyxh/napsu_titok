using System.ComponentModel.DataAnnotations;

namespace napsu_titok.Models
{
    public class Package
    {
        [Key]
        public int Id { get; set; }
        public double Amount { set; get; }
        public double Coins {set;get;} 
        public double Promotions { set;get;} 
    }
}
