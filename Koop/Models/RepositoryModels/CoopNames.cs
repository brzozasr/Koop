using System;

namespace Koop.Models.RepositoryModels
{
    public class CoopNames
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{LastName} {FirstName}";
    }
}