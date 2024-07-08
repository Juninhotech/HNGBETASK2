using System.ComponentModel.DataAnnotations;

namespace HNGTASK2.Models
{
    public class User
    {
        public User()
        {
            UserOrganisations = new HashSet<UserOrganisation>();
        }
        [Key]
        public string UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string Phone { get; set; }
        // Navigation property for many-to-many relationship
        public ICollection<UserOrganisation> UserOrganisations { get; set; }
    }
}
