using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }
        // from the group model (Entity framework will connect the Primarykey and forign key)
        public virtual User User { get; set; }
        public int UserId { get; set; }
        public virtual Currency Currency { get; set; }
        public int CurrencyId { get; set; }
        [Required]
        public float Value { get; set; }
    }
}
