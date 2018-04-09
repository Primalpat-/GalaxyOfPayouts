using System;
using System.Collections.Generic;

namespace GalaxyOfPayouts.Entities
{
    public partial class Rotations
    {
        public Rotations()
        {
            RotationUsers = new HashSet<RotationUsers>();
        }

        public int Id { get; set; }
        public decimal ChannelId { get; set; }
        public string Timezone { get; set; }
        public DateTime? LastNotification { get; set; }

        public virtual ICollection<RotationUsers> RotationUsers { get; set; }
    }
}
