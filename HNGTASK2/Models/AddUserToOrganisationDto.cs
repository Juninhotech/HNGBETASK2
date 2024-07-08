using System.ComponentModel.DataAnnotations;

namespace HNGTASK2.Models
{
    public class AddUserToOrganisationDto
    {
        [Required]
        public string UserId { get; set; }
    }
}
