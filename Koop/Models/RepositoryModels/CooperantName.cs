using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Koop.Models.RepositoryModels
{
    public class CooperantName
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        [NotMapped] public string FullName => $"{FirstName} {LastName}";
    }
}