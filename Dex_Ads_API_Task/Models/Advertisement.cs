using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dex_Ads_API_Task.Models
{
    public class Advertisement
    {
        public Guid Id { get; set; }
        public int Number { get; set; }
        public Guid User { get; set; }
        public string Text { get; set; }
        public string Img { get; set; }
        public int Rating { get; set; }
        public DateTime Created { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
