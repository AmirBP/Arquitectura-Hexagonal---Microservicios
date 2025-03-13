using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPhotoService.Domain.Entities
{
    public class UserPhoto
    {
        public int UserId { get; set; }
        public string PhotoBase64 { get; set; }
        public string PhotoType { get; set; }

        public UserPhoto(int userId, string photoBase64, string photoType)
        {
            UserId = userId;
            PhotoBase64 = photoBase64;
            PhotoType = photoType;
        }
    }
}
