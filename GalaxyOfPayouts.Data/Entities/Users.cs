using System.Collections.Generic;

namespace GalaxyOfPayouts.Data.Entities
{
    public partial class Users
    {
        public Users()
        {
            RotationUsers = new HashSet<RotationUsers>();
        }

        public decimal Id { get; set; }
        public string Username { get; set; }
        public string Nickname { get; set; }
        public string Mention { get; set; }

        public virtual ICollection<RotationUsers> RotationUsers { get; set; }
    }
}
