using System;

namespace CountingKs.Models
{
    public class AuthTokenModel
    {
        public DateTime Expiration { get; set; }
        public string Token { get; set; }
    }
}