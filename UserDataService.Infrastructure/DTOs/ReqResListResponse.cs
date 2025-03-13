using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserDataService.Infrastructure.DTOs
{
    public class ReqResListResponse<T>
    {
        public int Page { get; set; }
        public int Per_page { get; set; }
        public int Total { get; set; }
        public int Total_pages { get; set; }
        public List<T> Data { get; set; } = new List<T>();
    }
}
