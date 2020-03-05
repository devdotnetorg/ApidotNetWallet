using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Models
{
    public class Currency
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [StringLength(5)]
        public string Code { get; set; }
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }

    }
}
