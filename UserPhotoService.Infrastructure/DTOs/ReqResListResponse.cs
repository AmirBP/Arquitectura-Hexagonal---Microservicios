using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserPhotoService.Infrastructure.DTOs
{
    public class ReqResListResponse<T>
    {
        public int Page { get; set; }
        public int Per_Page { get; set; }
        public int Total { get; set; }
        public int Total_Pages { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
