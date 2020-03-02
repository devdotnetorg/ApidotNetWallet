using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApidotNetWallet.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Укажите Email пользователя")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Укажите имя пользователя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Укажите пароль пользователя")]
        public string Password { get; set; }
        [JsonIgnore]
        public virtual ICollection<Wallet> Wallets { get; set; }
        public bool ShouldSerializePassword() => false;
    }
}
