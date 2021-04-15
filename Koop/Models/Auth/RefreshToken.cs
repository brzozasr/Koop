using System;

namespace Koop.Models.Auth
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public int TokenExp { get; set; }
        public string RefreshT { get; set; }
        public int RefTokenExp { get; set; }
        public Guid UserId { get; set; }
    }
}