using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Persistence
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<ApprovalRule> ApprovalRules { get; set; }
        public DbSet<ApprovalStatus> ApprovalStatuses { get; set; }
        public DbSet<ApproverRole> ApproverRoles { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<ProjectApprovalStep> ProjectApprovalSteps { get; set; }
        public DbSet<ProjectProposal> ProjectProposals { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovalRule>(entity =>
            {
                entity.ToTable("ApprovalRule");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(x => x.MinAmount).HasPrecision(18, 4);
                entity.Property(x => x.MaxAmount).HasPrecision(18, 4);

                entity.HasOne(x => x.ProjectType).WithMany(a => a.ApprovalRules).HasForeignKey(x => x.Type).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.Areas).WithMany(a => a.ApprovalRules).HasForeignKey(x => x.Area).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ApproverRole).WithMany(a=> a.ApprovalRules).HasForeignKey(x=>x.ApproverRoleId).OnDelete(DeleteBehavior.Restrict);


                entity.HasData(
                    new ApprovalRule { Id = 1, MinAmount = 0, MaxAmount = 100000, Area = null, Type = null, StepOrder = 1, ApproverRoleId = 1 },
                    new ApprovalRule { Id = 2, MinAmount = 5000, MaxAmount = 20000, Area = null, Type = null, StepOrder = 2, ApproverRoleId = 2 },
                    new ApprovalRule { Id = 3, MinAmount = 0, MaxAmount = 20000, Area = 2, Type = 2, StepOrder = 1, ApproverRoleId = 2 },
                    new ApprovalRule { Id = 4, MinAmount = 20000, MaxAmount = 0, Area = null, Type = null, StepOrder = 3, ApproverRoleId = 3 },
                    new ApprovalRule { Id = 5, MinAmount = 5000, MaxAmount = 0, Area = 1, Type = 1, StepOrder = 2, ApproverRoleId = 2 },
                    new ApprovalRule { Id = 6, MinAmount = 0, MaxAmount = 10000, Area = null, Type = 2, StepOrder = 1, ApproverRoleId = 1 },
                    new ApprovalRule { Id = 7, MinAmount = 0, MaxAmount = 10000, Area = 2, Type = 1, StepOrder = 1, ApproverRoleId = 4 },
                    new ApprovalRule { Id = 8, MinAmount = 10000, MaxAmount = 30000, Area = 2, Type = null, StepOrder = 2, ApproverRoleId = 2 },
                    new ApprovalRule { Id = 9, MinAmount = 30000, MaxAmount = 0, Area = 3, Type = null, StepOrder = 2, ApproverRoleId = 3 },
                    new ApprovalRule { Id = 10, MinAmount = 0, MaxAmount = 50000, Area = null, Type = 4, StepOrder = 1, ApproverRoleId = 4 }
                    );

            });
            modelBuilder.Entity<ApprovalStatus>(entity =>
            {
                entity.ToTable("ApprovalStatus");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.Name)
                .HasColumnType("varchar(25)");

                entity.HasData(
                    new ApprovalStatus { Id = 1, Name = "Pending" },
                    new ApprovalStatus { Id = 2, Name = "Approved" },
                    new ApprovalStatus { Id = 3, Name = "Rejected" },
                    new ApprovalStatus { Id = 4, Name = "Observed" }
                    );

            });
            modelBuilder.Entity<ApproverRole>(entity =>
            {
                entity.ToTable("ApproverRole");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.Name)
                .HasColumnType("varchar(25)");

                entity.HasData(
                    new ApproverRole { Id = 1, Name = "Líder de Área" },
                    new ApproverRole { Id = 2, Name = "Gerente" },
                    new ApproverRole { Id = 3, Name = "Director" },
                    new ApproverRole { Id = 4, Name = "Comité Técnico" }
                    );

            });
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("Area");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.Name)
                .HasColumnType("varchar(25)");

                entity.HasData(
                    new Area { Id = 1, Name = "Finanzas" },
                    new Area { Id = 2, Name = "Tecnología" },
                    new Area { Id = 3, Name = "Recursos Humanos" },
                    new Area { Id = 4, Name = "Operaciones" }
                    );

            });
            modelBuilder.Entity<ProjectApprovalStep>(entity =>
            {
                entity.ToTable("ProjectApprovalStep");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.DecisionDate)
                .HasColumnType("datetime");

                entity.Property(t => t.Observations)
                .HasColumnType("varchar(MAX)");

                entity.HasOne(x => x.ProjectProposal).WithMany(a => a.ProjectApprovalSteps).HasForeignKey(x => x.ProjectProposalId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.User).WithMany(a => a.ProjectApprovalSteps).HasForeignKey(x => x.ApproverUserId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ApproverRole).WithMany(a => a.ProjectApprovalSteps).HasForeignKey(x => x.ApproverRoleId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ApprovalStatus).WithMany(a => a.ProjectApprovalSteps).HasForeignKey(x => x.Status).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<ProjectProposal>(entity =>
            {
                entity.ToTable("ProjectProposal");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.Title)
                .HasColumnType("varchar(255)");

                entity.Property(t => t.CreateAt)
                .HasColumnType("datetime");

                entity.Property(x => x.EstimatedAmount).HasPrecision(18, 4);

                entity.HasOne(x => x.Areas).WithMany(a => a.ProjectProposals).HasForeignKey(x => x.Area).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ProjectType).WithMany(a => a.ProjectProposals).HasForeignKey(x => x.Type).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.User).WithMany(a => a.ProjectProposals).HasForeignKey(x => x.CreateBy).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(x => x.ApprovalStatus).WithMany(a => a.ProjectProposals).HasForeignKey(x => x.Status).OnDelete(DeleteBehavior.Restrict);

            });
            modelBuilder.Entity<ProjectType>(entity =>
            {
                entity.ToTable("ProjectType");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.Name)
                .HasColumnType("varchar(25)");

                entity.HasData(
                    new ProjectType { Id = 1, Name = "Mejora de Procesos" },
                    new ProjectType { Id = 2, Name = "Innovación y Desarrollo" },
                    new ProjectType { Id = 3, Name = "Infraestructura" },
                    new ProjectType { Id = 4, Name = "Capacitación Interna" }
                    );

            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");
                entity.HasKey(x => x.Id);
                entity.Property(t => t.Id).ValueGeneratedOnAdd();

                entity.Property(t => t.Name)
                .HasColumnType("varchar(25)");

                entity.Property(t => t.Email)
                .HasColumnType("varchar(100)");

                entity.HasOne(x => x.ApproverRole).WithMany(a => a.Users).HasForeignKey(x => x.Role).OnDelete(DeleteBehavior.Restrict);

                entity.HasData(
                    new User { Id = 1, Name = "José Ferreyra", Email = "jferreyra@unaj.com", Role = 2 },
                    new User { Id = 2, Name = "Ana Lucero", Email = "alucero@unaj.com", Role = 1 },
                    new User { Id = 3, Name = "Gonzalo Molinas", Email = "gmolinas@unaj.com", Role = 2 },
                    new User { Id = 4, Name = "Lucas Olivera", Email = "lolivera@unaj.com", Role = 3 },
                    new User { Id = 5, Name = "Danilo Fagundez", Email = "dfagundez@unaj.com", Role = 4 },
                    new User { Id = 6, Name = "Gabriel Galli", Email = "ggalli@unaj.com", Role = 4 }
                    );
            });
        }
    }
}
