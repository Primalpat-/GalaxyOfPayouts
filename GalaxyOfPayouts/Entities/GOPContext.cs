using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GalaxyOfPayouts.Entities
{
    public partial class GOPContext : DbContext
    {
        public virtual DbSet<RotationUsers> RotationUsers { get; set; }
        public virtual DbSet<Rotations> Rotations { get; set; }
        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=localhost;Database=GOP;User Id=GOPUser;Password=Roflmao1");
            //optionsBuilder.UseSqlServer(@"Server=inntecdevserver.cloudapp.net,31337;Database=GOP;User Id=GOPUser;Password=Roflmao1");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RotationUsers>(entity =>
            {
                entity.HasKey(e => new { e.RotationId, e.UserId })
                    .HasName("PK_RotationUsers");

                entity.ToTable("RotationUsers", "dbo");

                entity.Property(e => e.UserId).HasColumnType("numeric");

                entity.Property(e => e.NextRank).HasDefaultValueSql("0");

                entity.HasOne(d => d.Rotation)
                    .WithMany(p => p.RotationUsers)
                    .HasForeignKey(d => d.RotationId)
                    .HasConstraintName("FK_RotationUsers_Rotations");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.RotationUsers)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_RotationUsers_Users");
            });

            modelBuilder.Entity<Rotations>(entity =>
            {
                entity.ToTable("Rotations", "dbo");

                entity.Property(e => e.ChannelId).HasColumnType("numeric");

                entity.Property(e => e.LastNotification).HasColumnType("datetime");

                entity.Property(e => e.Timezone)
                    .IsRequired()
                    .HasColumnType("varchar(500)");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.ToTable("Users", "dbo");

                entity.Property(e => e.Id).HasColumnType("numeric");

                entity.Property(e => e.Mention)
                    .IsRequired()
                    .HasColumnType("varchar(500)");

                entity.Property(e => e.Nickname).HasColumnType("varchar(500)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnType("varchar(500)");
            });
        }
    }
}