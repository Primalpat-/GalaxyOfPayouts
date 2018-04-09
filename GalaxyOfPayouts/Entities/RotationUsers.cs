using System;
using System.Collections.Generic;

namespace GalaxyOfPayouts.Entities
{
    public partial class RotationUsers
    {
        public int RotationId { get; set; }
        public decimal UserId { get; set; }
        public int NextRank { get; set; }

        public virtual Rotations Rotation { get; set; }
        public virtual Users User { get; set; }
    }
}
