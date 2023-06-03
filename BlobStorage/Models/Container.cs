using System.ComponentModel.DataAnnotations;

namespace BlobStorage.Models
{
    public class Container
    {
        [Required]
        public string Name { get; set; }
    }
}
