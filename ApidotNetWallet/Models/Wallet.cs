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
        public Guid Id { get; set; }
        // from the group model (Entity framework will connect the Primarykey and forign key)
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
        public virtual Currency Currency { get; set; }
        public Guid CurrencyId { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}
