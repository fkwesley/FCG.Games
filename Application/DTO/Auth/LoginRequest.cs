using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO.Auth
{
    public class LoginRequest
    {
        [Required]
        [MaxLength(60)]
        public string UserId { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
