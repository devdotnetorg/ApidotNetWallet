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
        [EmailAddress]
        public string Email { get; set; }
        [StringLength(50, ErrorMessage = "Длина {0} должна быть не менее {2} символов.", MinimumLength = 4)]
        [Required(ErrorMessage = "Укажите имя пользователя")]
        public string Name { get; set; }
        [StringLength(50, ErrorMessage = "Длина {0} должна быть не менее {2} символов.", MinimumLength = 6)]
        [Required(ErrorMessage = "Укажите пароль пользователя")]
        public string Password { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        public virtual ICollection<Wallet> Wallets { get; set; }
        public bool ShouldSerializePassword() => false;
    }
}
