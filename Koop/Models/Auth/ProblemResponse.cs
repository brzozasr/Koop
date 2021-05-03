using System;

namespace Koop.Models.Auth
{
    public class ProblemResponse
    {
        public string Detail { get; set; }
        public int Status { get; set; }
        public object Data { get; set; }
        public string UserId { get; set; }
    }
}