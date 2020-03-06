using System;
using System.ComponentModel.DataAnnotations;

namespace ApidotNetWallet.Models
{
    /// <summary>
    /// Кошельки
    /// </summary>
    public class Wallet
    {
        [Key]
        public Guid Id { get; set; }
        public virtual User User { get; set; }
        public Guid UserId { get; set; }
        public virtual Currency Currency { get; set; }
        public Guid CurrencyId { get; set; }
        [Required]
        public decimal Value { get; set; }
    }
}
