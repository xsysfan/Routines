﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using DashboardCode.AdminkaV1.DataAccessEfCore;

namespace DashboardCode.AdminkaV1.Injected.NETStandard.EfCoreMigrationApp.Migrations
{
    [DbContext(typeof(AdminkaDbContext))]
    partial class AdminkaDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupAdName")
                        .IsRequired()
                        .HasMaxLength(126);

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("GroupId");

                    b.HasIndex("GroupAdName")
                        .IsUnique()
                        .HasName("IX_scr_Groups_GroupAdName");

                    b.HasIndex("GroupName")
                        .IsUnique()
                        .HasName("IX_scr_Groups_GroupName");

                    b.ToTable("Groups","scr");

                    b.HasAnnotation("Constraints", new  DashboardCode.AdminkaV1.DataAccessEfCore.Constraint[]{new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Groups_GroupName", Body=@"CHECK (GroupName NOT LIKE '%[^a-z0-9 ]%')", Message=@"Only letters, numbers and space", Fields=new[] {"GroupName"}},new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Groups_GroupAdName", Body=@"CHECK (GroupAdName NOT LIKE '%[^a-z0-9!.!-!_!\!@]%' ESCAPE '!')", Message=@"Only letters, numbers and .-_@", Fields=new[] {"GroupAdName"}},});
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.GroupPrivilege", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("GroupId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("GroupPrivilegeMap","scr");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.GroupRole", b =>
                {
                    b.Property<int>("GroupId");

                    b.Property<int>("RoleId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("GroupId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("GroupRoleMap","scr");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.Privilege", b =>
                {
                    b.Property<string>("PrivilegeId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<string>("PrivilegeName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("PrivilegeId");

                    b.HasIndex("PrivilegeName")
                        .IsUnique()
                        .HasName("IX_scr_Privileges_PrivilegeName");

                    b.ToTable("Privileges","scr");

                    b.HasAnnotation("Constraints", new  DashboardCode.AdminkaV1.DataAccessEfCore.Constraint[]{new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Privileges_PrivilegeId", Body=@"CHECK (PrivilegeId NOT LIKE '%[^a-z0-9]%')", Message=@"Only letters and numbers", Fields=new[] {"PrivilegeId"}},});
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.Role", b =>
                {
                    b.Property<int>("RoleId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(64);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("RoleId");

                    b.HasIndex("RoleName")
                        .IsUnique()
                        .HasName("IX_scr_Roles_RoleName");

                    b.ToTable("Roles","scr");

                    b.HasAnnotation("Constraints", new  DashboardCode.AdminkaV1.DataAccessEfCore.Constraint[]{new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Roles_RoleName", Body=@"CHECK(RoleName NOT LIKE '%[^a-z0-9 ]%')", Message=@"[^a-z0-9 ]", Fields=new[] {"RoleName"}},});
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.RolePrivilege", b =>
                {
                    b.Property<int>("RoleId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("RoleId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("RolePrivilegeMap","scr");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FirstName")
                        .HasMaxLength(64);

                    b.Property<string>("LoginName")
                        .IsRequired()
                        .HasMaxLength(126);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.Property<string>("SecondName")
                        .HasMaxLength(32);

                    b.HasKey("UserId");

                    b.HasIndex("LoginName")
                        .IsUnique()
                        .HasName("IX_scr_Users_LoginName");

                    b.ToTable("Users","scr");

                    b.HasAnnotation("Constraints", new  DashboardCode.AdminkaV1.DataAccessEfCore.Constraint[]{new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Users_LoginName", Body=@"CHECK (LoginName NOT LIKE '%[^a-z0-9!.!-!_!\!@]%' ESCAPE '!')", Message=@"Only letters, numbers and .-_@", Fields=new[] {"LoginName"}},new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Users_SecondName", Body=@"CHECK (SecondName NOT LIKE '%[^a-z '']%')", Message=@"Only letters, space and apostrophe", Fields=new[] {"SecondName"}},new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_scr_Users_FirstName", Body=@"CHECK (FirstName NOT LIKE '%[^a-z ]%')", Message=@"Only letters and space", Fields=new[] {"FirstName"}},});
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.UserGroup", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("GroupId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("UserId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("UserGroupMap","scr");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.UserPrivilege", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<string>("PrivilegeId")
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("UserId", "PrivilegeId");

                    b.HasIndex("PrivilegeId");

                    b.ToTable("UserPrivilegeMap","scr");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.UserRole", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("RoleId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("UserRoleMap","scr");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.LoggingDom.ActivityRecord", b =>
                {
                    b.Property<int>("ActivityRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ActivityRecordLoggedAt");

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<Guid>("CorrelationToken");

                    b.Property<long>("DurationTicks");

                    b.Property<string>("FullActionName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<bool>("Successed");

                    b.HasKey("ActivityRecordId");

                    b.ToTable("ActivityRecords","log");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.LoggingDom.VerboseRecord", b =>
                {
                    b.Property<int>("ActivityRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Application")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.Property<Guid>("CorrelationToken");

                    b.Property<string>("FullActionName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<DateTime>("VerboseRecordLoggedAt");

                    b.Property<string>("VerboseRecordMessage");

                    b.Property<string>("VerboseRecordTypeId")
                        .IsRequired()
                        .HasMaxLength(4);

                    b.HasKey("ActivityRecordId");

                    b.ToTable("VerboseRecords","log");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.ChildRecord", b =>
                {
                    b.Property<int>("ParentRecordId")
                        .HasMaxLength(4);

                    b.Property<string>("TypeRecordId")
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.Property<string>("XmlField1")
                        .HasColumnType("xml");

                    b.Property<string>("XmlField2")
                        .HasColumnType("xml");

                    b.HasKey("ParentRecordId", "TypeRecordId");

                    b.HasIndex("TypeRecordId");

                    b.ToTable("ChildRecords","tst");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.HierarchyRecord", b =>
                {
                    b.Property<int>("HierarchyRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HierarchyRecordTitle")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<byte[]>("ParentHierarchyRecordId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("HierarchyRecordId");

                    b.ToTable("HierarchyRecords","tst");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.ParentRecord", b =>
                {
                    b.Property<int>("ParentRecordId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FieldA")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldB1")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldB2")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldCA")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldCB1")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<string>("FieldCB2")
                        .IsRequired()
                        .HasMaxLength(16);

                    b.Property<int>("FieldNotNull");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("ParentRecordId");

                    b.HasAlternateKey("FieldCA");


                    b.HasAlternateKey("FieldCB1", "FieldCB2");

                    b.HasIndex("FieldA")
                        .IsUnique();

                    b.HasIndex("FieldB1", "FieldB2")
                        .IsUnique();

                    b.ToTable("ParentRecords","tst");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.ParentRecordHierarchyRecord", b =>
                {
                    b.Property<int>("ParentRecordId");

                    b.Property<int>("HierarchyRecordId");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.HasKey("ParentRecordId", "HierarchyRecordId");

                    b.HasIndex("HierarchyRecordId");

                    b.ToTable("ParentRecordHierarchyRecordMap","tst");
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.TypeRecord", b =>
                {
                    b.Property<string>("TestTypeRecordId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(4);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<DateTime>("RowVersionAt");

                    b.Property<string>("RowVersionBy")
                        .HasMaxLength(126);

                    b.Property<string>("TypeRecordName")
                        .IsRequired()
                        .HasMaxLength(32);

                    b.Property<string>("TypeRecordTestTypeRecordId");

                    b.HasKey("TestTypeRecordId");

                    b.HasIndex("TypeRecordName")
                        .IsUnique();

                    b.HasIndex("TypeRecordTestTypeRecordId");

                    b.ToTable("TypeRecords","tst");

                    b.HasAnnotation("Constraints", new  DashboardCode.AdminkaV1.DataAccessEfCore.Constraint[]{new DashboardCode.AdminkaV1.DataAccessEfCore.Constraint(){Name="CK_TypeRecords_TypeRecordName", Body=@"CHECK (TypeRecordName NOT LIKE '%[^a-z0-9 ]%')", Message=@"Only letters, numbers and space", Fields=new[] {"TypeRecordName"}},});
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.GroupPrivilege", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Group", "Group")
                        .WithMany("GroupPrivilegeMap")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Privilege", "Privilege")
                        .WithMany("GroupPrivilegeMap")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.GroupRole", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Group", "Group")
                        .WithMany("GroupRoleMap")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Role", "Role")
                        .WithMany("GroupRoleMap")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.RolePrivilege", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Privilege", "Privilege")
                        .WithMany("RolePrivilegeMap")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Role", "Role")
                        .WithMany("RolePrivilegeMap")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.UserGroup", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Group", "Group")
                        .WithMany("UserGroupMap")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.User", "User")
                        .WithMany("UserGroupMap")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.UserPrivilege", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Privilege", "Privilege")
                        .WithMany("UserPrivilegeMap")
                        .HasForeignKey("PrivilegeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.User", "User")
                        .WithMany("UserPrivilegeMap")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.AuthenticationDom.UserRole", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.Role", "Role")
                        .WithMany("UserRoleMap")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.AuthenticationDom.User", "User")
                        .WithMany("UserRoleMap")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.ChildRecord", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.TestDom.ParentRecord", "ParentRecord")
                        .WithMany("ChildRecords")
                        .HasForeignKey("ParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.TestDom.TypeRecord", "TypeRecord")
                        .WithMany("ChildRecords")
                        .HasForeignKey("TypeRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.ParentRecordHierarchyRecord", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.TestDom.HierarchyRecord", "HierarchyRecord")
                        .WithMany("ParentRecordHierarchyRecordMap")
                        .HasForeignKey("HierarchyRecordId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DashboardCode.AdminkaV1.TestDom.ParentRecord", "ParentRecord")
                        .WithMany("ParentRecordHierarchyRecordMap")
                        .HasForeignKey("ParentRecordId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DashboardCode.AdminkaV1.TestDom.TypeRecord", b =>
                {
                    b.HasOne("DashboardCode.AdminkaV1.TestDom.TypeRecord")
                        .WithMany("TypeRecords")
                        .HasForeignKey("TypeRecordTestTypeRecordId");
                });
#pragma warning restore 612, 618
        }
    }
}