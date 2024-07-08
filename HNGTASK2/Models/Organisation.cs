using System.ComponentModel.DataAnnotations;

namespace HNGTASK2.Models
{
    public class Organisation
    {
        public Organisation()
        {
            UserOrganisations = new HashSet<UserOrganisation>();
        }

        [Key]
        public string OrgId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        // Navigation property for many-to-many relationship
        public ICollection<UserOrganisation> UserOrganisations { get; set; }
    }
}
