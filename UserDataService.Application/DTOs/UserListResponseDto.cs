using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDataService.Application.DTOs
{
    public class UserListResponseDto
    {
        public int Page { get; set; }
        public int TotalPages { get; set; }
        public List<UserDataResponseDto> Users { get; set; } = new List<UserDataResponseDto>();
    }
}
