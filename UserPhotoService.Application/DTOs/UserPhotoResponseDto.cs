using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPhotoService.Application.DTOs
{
    public class UserPhotoResponseDto
    {
        public int UserId { get; set; }
        public string PhotoBase64 { get; set; } = string.Empty;
        public string PhotoType { get; set; } = string.Empty;
    }
}
