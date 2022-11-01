using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAuth.Server.Models
{
    public class JWT
    {
        public int Id { get; set; }
        public string? Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public DateTime Expires { get; set; }
    }
}