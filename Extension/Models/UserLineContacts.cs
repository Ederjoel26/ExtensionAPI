namespace Extension.Models
{
    public class UserLineContacts
    {
        public string UserName { get; set; }
        public List<ContactsCount> ContactsCount { get; set; } = new List<ContactsCount>();
    }
}
