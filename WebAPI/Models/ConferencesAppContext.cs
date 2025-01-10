using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{

    public class SqlServerApplicationDbContext : ConferencesAppContext
    {
        public SqlServerApplicationDbContext(DbContextOptions<SqlServerApplicationDbContext> options)   //mozda fali configuration
            : base(options)
        { }
    }

    public abstract class ConferencesAppContext : DbContext
    {
        public ConferencesAppContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<Conference> Conferences { get; set; }

        public virtual DbSet<Hotel> Hotels { get; set; }

        public virtual DbSet<Paper> Papers { get; set; }

        public virtual DbSet<Registration> Registrations { get; set; }

        public virtual DbSet<Review> Reviews { get; set; }

        public virtual DbSet<UserConf> UserConfs { get; set; }

        public virtual DbSet<ProgramCommittee> ProgramCommittees { get; set; }
        public virtual DbSet<GuestLecturer> GuestLecturers { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            /* Unique keys */
            //modelBuilder.Entity<UserConf>()
            //            .HasIndex(b => b.Email)
            //            .IsUnique();

            //modelBuilder.Entity<ProgramCommittee>()
            //            .HasIndex(pc => new { pc.IdUser, pc.IdConference })
            //            .IsUnique();

            //modelBuilder.Entity<GuestLecturer>()
            //            .HasIndex(pc => new { pc.IdUser, pc.IdConference })
            //            .IsUnique();


            /* Relationships */
            modelBuilder.Entity<Conference>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<Conference>()
                        .HasMany(i => i.GuestLecturers)
                        .WithOne(t => t.Conference)
                        .HasForeignKey(i => i.IdConference)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<Conference>()
                        .HasMany(i => i.ProgramCommittee)
                        .WithOne(t => t.Conference)
                        .HasForeignKey(i => i.IdConference)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<Conference>()
                        .HasMany(i => i.Registrations)
                        .WithOne(t => t.Conference)
                        .HasForeignKey(i => i.IdConference)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<Hotel>()
                        .HasKey(i => new { i.IdRoom });

            modelBuilder.Entity<Paper>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<Paper>()
                        .HasOne(i => i.Registration)
                        .WithOne(j => j.Paper)
                        .HasForeignKey<Registration>(j => j.IdPaper)
                        .IsRequired(false);

            modelBuilder.Entity<Paper>()
                        .HasMany(i => i.Reviews)
                        .WithOne(t => t.Paper)
                        .HasForeignKey(i => i.IdPaper)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<Registration>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<Registration>()
                        .HasOne(i => i.Conference)
                        .WithMany(j => j.Registrations)
                        .HasForeignKey(j => j.IdConference)
                        .IsRequired(true);

            modelBuilder.Entity<Registration>()
                        .HasOne(i => i.User)
                        .WithMany(j => j.Registrations)
                        .HasForeignKey(j => j.IdUser)
                        .IsRequired(true);

            modelBuilder.Entity<Registration>()
                        .HasOne(i => i.Room)
                        .WithOne(j => j.Registration)
                        .HasForeignKey<Hotel>(i => i.IdRegistration)
                        .IsRequired(false);

            modelBuilder.Entity<Registration>()
                        .HasOne(i => i.Paper)
                        .WithOne(j => j.Registration)
                        .HasForeignKey<Paper>(i => i.IdRegistration)
                        .IsRequired(false);

            modelBuilder.Entity<Review>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<Review>()
                        .HasOne(i => i.Paper)
                        .WithMany(j => j.Reviews)
                        .HasForeignKey(j => j.IdPaper)
                        .IsRequired(true);

            modelBuilder.Entity<Review>()
                        .HasOne(i => i.Reviewer)
                        .WithMany(j => j.Reviews)
                        .HasForeignKey(j => j.IdReviewer)
                        .IsRequired(true);

            modelBuilder.Entity<UserConf>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<UserConf>()
                        .HasMany(i => i.Registrations)
                        .WithOne(t => t.User)
                        .HasForeignKey(i => i.IdUser)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<UserConf>()
                        .HasMany(i => i.ProgramCommittees)
                        .WithOne(t => t.User)
                        .HasForeignKey(i => i.IdUser)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<UserConf>()
                        .HasMany(i => i.GuestLecturers)
                        .WithOne(t => t.User)
                        .HasForeignKey(i => i.IdUser)
                        .HasPrincipalKey(t => t.Id);

            modelBuilder.Entity<GuestLecturer>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<GuestLecturer>()
                        .HasOne(i => i.User)
                        .WithMany(j => j.GuestLecturers)
                        .HasForeignKey(i => i.IdUser)
                        .IsRequired(true);

            modelBuilder.Entity<GuestLecturer>()
                        .HasOne(i => i.Conference)
                        .WithMany(j => j.GuestLecturers)
                        .HasForeignKey(i => i.IdConference)
                        .IsRequired(true);

            modelBuilder.Entity<ProgramCommittee>()
                        .HasKey(i => new { i.Id });

            modelBuilder.Entity<ProgramCommittee>()
                        .HasOne(i => i.User)
                        .WithMany(j => j.ProgramCommittees)
                        .HasForeignKey(i => i.IdUser)
                        .IsRequired(true);

            modelBuilder.Entity<ProgramCommittee>()
                        .HasOne(i => i.Conference)
                        .WithMany(j => j.ProgramCommittee)
                        .HasForeignKey(i => i.IdConference)
                        .IsRequired(true);

            modelBuilder.Entity<ProgramCommittee>()
                        .HasMany(i => i.Reviews)
                        .WithOne(t => t.Reviewer)
                        .HasForeignKey(i => i.IdReviewer)
                        .HasPrincipalKey(t => t.Id);
        }
    }
}