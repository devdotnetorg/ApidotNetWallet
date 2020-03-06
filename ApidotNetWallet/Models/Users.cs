using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApidotNetWallet.Models
{
    /// <summary>
    /// Пользователи
    /// </summary>
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Укажите Email пользователя")]
        public string Email { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Укажите имя пользователя")]
        public string Name { get; set; }
        [StringLength(50)]
        [Required(ErrorMessage = "Укажите пароль пользователя")]
        public string Password { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
        public bool ShouldSerializePassword() => false;
    }
}
