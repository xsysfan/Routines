﻿using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;

using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.AdminkaV1.LoggingDom;
using DashboardCode.AdminkaV1.TestDom;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AdminkaDbContext : DbContext
    {
        public readonly ILoggerFactory loggerFactory;
        public readonly Action<ILoggerFactory> returnLoggerFactory;
        static AdminkaDbContext()
        {
            var loadit = new[] 
            {
                typeof(Remotion.Linq.DefaultQueryProvider),
                typeof(System.Collections.Generic.AsyncEnumerator),
                typeof(Remotion.Linq.DefaultQueryProvider)
            };
        }

        public AdminkaDbContext(IAdminkaOptionsFactory optionsFactory)
            : base(optionsFactory
                  .BuildOptions(
                        new DbContextOptionsBuilder<AdminkaDbContext>()
                    )
              )
        {
        }

        public AdminkaDbContext(
            IAdminkaOptionsFactory optionsFactory, 
            Func<StatefullLoggerFactory>   getLoggerFactory,
            Action<StatefullLoggerFactory> returnLoggerFactory
            )
            : this(optionsFactory)
        {
        }

        private AdminkaDbContext(IAdminkaOptionsFactory optionsFactory,
            ILoggerFactory loggerFactory,
            Action<ILoggerFactory> returnLoggerFactory
            )
            : base(optionsFactory
                  .BuildOptions(
                        new DbContextOptionsBuilder<AdminkaDbContext>()
                            .UseLoggerFactory(loggerFactory)
                    )
              )
        {
            this.loggerFactory = loggerFactory;
            this.returnLoggerFactory = returnLoggerFactory;
        }

        #region DbSets
        public DbSet<Privilege> Privileges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<VerboseRecord> VerboseRecords { get; set; }
        public DbSet<ActivityRecord> ActivityRecords { get; set; }
        public DbSet<ParentRecord> ParentRecords { get; set; }
        public DbSet<ChildRecord> ChildRecords { get; set; }
        public DbSet<HierarchyRecord> TestRecords { get; set; }

        private static string GetEntityTableName(string value)
        {
            return value + "s";
        }
        private static string GetMapTableName(string value)
        {
            return value + "Map";
        }
        #endregion

        private void SetupVersioned<T>(EntityTypeBuilder<T> builder) where T : class, IVersioned
        {
            builder.Property(e => e.RowVersionBy).HasMaxLength(LengthConstants.AdName);
            builder.Property(e => e.RowVersion).IsRowVersion();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupVersioned(modelBuilder.Entity<TypeRecord>());
            SetupVersioned(modelBuilder.Entity<ChildRecord>());
            SetupVersioned(modelBuilder.Entity<HierarchyRecord>());
            SetupVersioned(modelBuilder.Entity<ParentRecordHierarchyRecord>());
            SetupVersioned(modelBuilder.Entity<ParentRecord>());
            #region Test Island
            string testIslandSchema = "tst";
            modelBuilder.Entity<ParentRecord>()
                .ToTable(GetEntityTableName(nameof(ParentRecord)), schema: testIslandSchema)
                .HasKey(e => e.ParentRecordId);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldA).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldB1).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldB2).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCA).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCB1).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            modelBuilder.Entity<ParentRecord>().Property(e => e.FieldCB2).IsRequired().HasMaxLength(LengthConstants.GoodForFormLabel);
            // unique indexes
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => e.FieldA).IsUnique();
            modelBuilder.Entity<ParentRecord>()
                .HasIndex(e => new { e.FieldB1, e.FieldB2 }).IsUnique();
            // unique constraints
            modelBuilder.Entity<ParentRecord>()
               .HasAlternateKey(e => e.FieldCA);
            modelBuilder.Entity<ParentRecord>()
                .HasAlternateKey(e => new { e.FieldCB1, e.FieldCB2 });

            modelBuilder.Entity<ChildRecord>().Property(e => e.ParentRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ChildRecord>().Property(e => e.TypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField1).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>().Property(e => e.XmlField2).HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>()
                .ToTable(GetEntityTableName(nameof(ChildRecord)), schema: testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.TypeRecordId });

            modelBuilder.Entity<ChildRecord>().HasOne(e => e.ParentRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.ParentRecordId);
            //HasColumnType("xml");
            modelBuilder.Entity<ChildRecord>()
                .HasOne(e => e.TypeRecord)
                .WithMany(e => e.ChildRecords)
                .HasForeignKey(e => e.TypeRecordId);

            modelBuilder.Entity<TypeRecord>()
                .ToTable(GetEntityTableName(nameof(TypeRecord)), schema: testIslandSchema)
                .HasKey(e => e.TestTypeRecordId);
            modelBuilder.Entity<TypeRecord>()
                .Property(e => e.TestTypeRecordId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<TypeRecord>().Property(e => e.TypeRecordName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<TypeRecord>()
                .HasIndex(e => e.TypeRecordName).IsUnique();

            modelBuilder.Entity<HierarchyRecord>().Property(e => e.HierarchyRecordTitle).IsRequired().HasMaxLength(LengthConstants.GoodForLongTitle);
            modelBuilder.Entity<HierarchyRecord>()
               .ToTable(GetEntityTableName(nameof(HierarchyRecord)), schema: testIslandSchema)
               .HasKey(e => e.HierarchyRecordId);

            #region ParentRecordHierarchyRecord


            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .ToTable(GetMapTableName(nameof(ParentRecordHierarchyRecord)), schema: testIslandSchema)
                .HasKey(e => new { e.ParentRecordId, e.HierarchyRecordId });

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasOne(r => r.ParentRecord)
                .WithMany(pr => pr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.ParentRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasOne(r => r.HierarchyRecord)
                .WithMany(hr => hr.ParentRecordHierarchyRecordMap)
                .HasForeignKey(r => r.HierarchyRecordId);
            #endregion
            #endregion

            modelBuilder.Entity<ActivityRecord>().HasKey(e=> e.ActivityRecordId);
            modelBuilder.Entity<ActivityRecord>().Property(e => e.CorrelationToken).IsRequired();
            modelBuilder.Entity<ActivityRecord>().Property(e => e.Application).IsRequired().HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<ActivityRecord>().Property(e => e.FullActionName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<VerboseRecord>().HasKey(e => e.ActivityRecordId);
            modelBuilder.Entity<VerboseRecord>().Property(e => e.CorrelationToken).IsRequired();
            modelBuilder.Entity<VerboseRecord>().Property(e => e.Application).IsRequired().HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<VerboseRecord>().Property(e => e.FullActionName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<VerboseRecord>().Property(e => e.VerboseRecordTypeId).IsRequired().HasMaxLength(LengthConstants.GoodForKey);

            modelBuilder.Entity<Privilege>().HasKey(e => e.PrivilegeId);
            modelBuilder.Entity<Privilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<Privilege>().Property(e => e.PrivilegeName).IsRequired().HasMaxLength(LengthConstants.GoodForTitle);
            modelBuilder.Entity<User>().HasKey(e => e.UserId);
            modelBuilder.Entity<User>().Property(e => e.LoginName).IsRequired().HasMaxLength(LengthConstants.AdName);
            modelBuilder.Entity<User>().Property(e => e.FirstName).IsRequired().HasMaxLength(LengthConstants.GoodForTitle);
            modelBuilder.Entity<User>().Property(e => e.SecondName).IsRequired().HasMaxLength(LengthConstants.GoodForName);
            modelBuilder.Entity<Group>().HasKey(e=>e.GroupId);
            modelBuilder.Entity<Group>().Property(e => e.GroupName).IsRequired().HasMaxLength(LengthConstants.GoodForTitle);
            modelBuilder.Entity<Group>().Property(e => e.GroupAdName).IsRequired().HasMaxLength(LengthConstants.AdName);
            modelBuilder.Entity<Role>().HasKey(e => e.RoleId);
            modelBuilder.Entity<Role>().Property(e => e.RoleName).IsRequired().HasMaxLength(LengthConstants.GoodForTitle);

            #region UsersPrivileges
            modelBuilder.Entity<UserPrivilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<UserPrivilege>()
                .ToTable(GetMapTableName(nameof(UserPrivilege)))
                .HasKey(e => new { e.UserId, e.PrivilegeId });

            modelBuilder.Entity<UserPrivilege>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserPrivilegeMap)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserPrivilege>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.UserPrivilegeMap)
                .HasForeignKey(up => up.PrivilegeId);
            #endregion

            #region GroupsPrivileges
            modelBuilder.Entity<GroupPrivilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<GroupPrivilege>()
                .ToTable(GetMapTableName(nameof(GroupPrivilege)))
                .HasKey(e => new { e.GroupId, e.PrivilegeId });

            modelBuilder.Entity<GroupPrivilege>()
                .HasOne(gp => gp.Group)
                .WithMany(g => g.GroupPrivilegeMap)
                .HasForeignKey(gp => gp.GroupId);

            modelBuilder.Entity<GroupPrivilege>()
                .HasOne(gp => gp.Privilege)
                .WithMany(p => p.GroupPrivilegeMap)
                .HasForeignKey(gp => gp.PrivilegeId);
            #endregion

            #region UsersGroups
            modelBuilder.Entity<UserGroup>()
                .ToTable(GetMapTableName(nameof(UserGroup)))
                .HasKey(e => new { e.UserId, e.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroupMap)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.UserGroupMap)
                .HasForeignKey(ug => ug.GroupId);
            #endregion

            #region GroupsRoles
            modelBuilder.Entity<GroupRole>()
                .ToTable(GetMapTableName(nameof(GroupRole)))
                .HasKey(gr => new { gr.GroupId, gr.RoleId });

            modelBuilder.Entity<GroupRole>()
                .HasOne(gr => gr.Group)
                .WithMany(g => g.GroupRoleMap)
                .HasForeignKey(gr => gr.GroupId);

            modelBuilder.Entity<GroupRole>()
                .HasOne(gr => gr.Role)
                .WithMany(r => r.GroupRoleMap)
                .HasForeignKey(gr => gr.RoleId);
            #endregion

            #region RolesPrivileges
            modelBuilder.Entity<RolePrivilege>()
               .ToTable(GetMapTableName(nameof(RolePrivilege)))
               .HasKey(e => new { e.RoleId, e.PrivilegeId });
            modelBuilder.Entity<RolePrivilege>().Property(e => e.PrivilegeId).HasMaxLength(LengthConstants.GoodForKey);
            modelBuilder.Entity<RolePrivilege>()
                .HasOne(up => up.Role)
                .WithMany(u => u.RolePrivilegeMap)
                .HasForeignKey(up => up.RoleId);

            modelBuilder.Entity<RolePrivilege>()
                .HasOne(up => up.Privilege)
                .WithMany(p => p.RolePrivilegeMap)
                .HasForeignKey(up => up.PrivilegeId);
            #endregion

            #region UsersRoles
            modelBuilder.Entity<UserRole>()
                .ToTable(GetMapTableName(nameof(UserRole)))
                .HasKey(e => new { e.UserId, e.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(up => up.User)
                .WithMany(u => u.UserRoleMap)
                .HasForeignKey(up => up.UserId);

            modelBuilder.Entity<UserRole>()
                .HasOne(up => up.Role)
                .WithMany(p => p.UserRoleMap)
                .HasForeignKey(up => up.RoleId);
            #endregion
        }

        // NOTE: not threadsafe way of disposing
        bool isDisposed;
        public override void Dispose()
        {
            if (!isDisposed)
            {
                returnLoggerFactory(this.loggerFactory);
                isDisposed = true;
            }
            base.Dispose();
        }
    }
}
