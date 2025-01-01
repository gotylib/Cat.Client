using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cat.Client.Care
{
    public class ChatServer
    {
        [Key]
        public int Id {  get; set; }

        [Required]
        public string ChatName { get; set; }

        [Required]
        public int AdditionalId { get; set; }

        [Required]
        public string Url { get; set; }
    }
}
