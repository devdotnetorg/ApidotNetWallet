using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Models
{
    public class Token
    {
        [Key]
        public int Id { get; set; }
        // from the group model (Entity framework will connect the Primarykey and forign key)
        public virtual User User { get; set; }
        public int UserId { get; set; }
        [Required]
        public Guid Value { get; set; }
        public DateTime ExpiredDate { get; set; }
    }
}
