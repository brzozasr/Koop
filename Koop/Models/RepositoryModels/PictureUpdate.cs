using System;

namespace Koop.Models.RepositoryModels
{
    public class PictureUpdate
    {
        public Guid CategoryId { get; set; }
        public string Picture { get; set; }
    }
}