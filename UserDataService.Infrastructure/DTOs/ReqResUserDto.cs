using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDataService.Infrastructure.DTOs
{
    public class ReqResUserDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
    }
}
