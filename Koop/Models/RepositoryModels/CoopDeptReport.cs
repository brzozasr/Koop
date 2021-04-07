using System;

namespace Koop.Models.RepositoryModels
{
    public class CoopDeptReport
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public double? Debt { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}