

using System.ComponentModel.DataAnnotations;

namespace Cat.Client.Care
{
    public class DefaultServer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
