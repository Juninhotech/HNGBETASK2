namespace HNGTASK2.Models
{
    public class UserOrganisation
    {
        public string UserId { get; set; }
        public User User { get; set; } // Navigation property

        public string OrgId { get; set; }
        public Organisation Organisation { get; set; } // Navigation property
    }

}
