using System;

namespace Dex_Ads_API_Task.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Admin { get; set; }

    }
}
