using System.ComponentModel.DataAnnotations;

namespace HNGTASK2.Dtos
{
    public class CreateOrganisationDto
    {
        [Required]
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
