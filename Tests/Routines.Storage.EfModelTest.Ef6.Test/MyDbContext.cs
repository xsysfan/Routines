﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage.EfModelTest.Ef6.Test
{
    public class MyDbContext : DbContext
    {
        internal MyDbContext(string connectionStringName, Action<string> verbose)
            : base(connectionStringName)
        {
            Database.SetInitializer<MyDbContext>(new MyCreateDatabaseIfNotExists());

            if (verbose != null)
                this.Database.Log += message =>
                    verbose(message);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ParentRecord>().Property(t => t.FieldA).IsRequired().HasColumnAnnotation(IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new[]
                    {
                        new IndexAttribute("ParentRecord_UX_FieldA", 1) {IsUnique=true}
                    }
                    ));
            modelBuilder.Entity<ParentRecord>().HasKey(e => e.ParentRecordId)
                .ToTable("ParentRecords", "tst");
            modelBuilder.Entity<HierarchyRecord>().HasKey(e => e.HierarchyRecordId)
                .ToTable("HierarchyRecords", "tst");
            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasKey(e => new { e.ParentRecordId, e.HierarchyRecordId })
                .ToTable("ParentRecordHierarchyRecordMap","tst");

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasRequired(e => e.ParentRecord)
                .WithMany(e => e.ParentRecordHierarchyRecordMap)
                .HasForeignKey(e => e.ParentRecordId);

            modelBuilder.Entity<ParentRecordHierarchyRecord>()
                .HasRequired(e => e.HierarchyRecord)
                .WithMany(e => e.ParentRecordHierarchyRecordMap)
                .HasForeignKey(e => e.HierarchyRecordId);

            modelBuilder.Entity<ChildRecord>().HasKey(e => e.ParentRecordId)
                .ToTable("ChildRecords", "tst");
            modelBuilder.Entity<TypeRecord>().HasKey(e => e.TestTypeRecordId)
                .ToTable("TypeRecords", "tst");

        }

        public DbSet<ParentRecord> ParentRecords { get; set; }
        public DbSet<ChildRecord> ChildRecords { get; set; }
        public DbSet<TypeRecord> TypeRecords { get; set; }
        public DbSet<HierarchyRecord> HierarchyRecords { get; set; }
        public DbSet<ParentRecordHierarchyRecord> ParentRecordHierarchyRecords { get; set; }
    }

    public class MyCreateDatabaseIfNotExists : CreateDatabaseIfNotExists<MyDbContext>
    {
        protected override void Seed(MyDbContext context)
        {
            base.Seed(context);
        }
    }
}
