using Microsoft.EntityFrameworkCore;
using UniversityTasksDbFirstApi.Models;

namespace UniversityTasksDbFirstApi.Data;

public partial class UniversityTasksDbContext : DbContext
{
    public UniversityTasksDbContext(DbContextOptions<UniversityTasksDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Assignment> Assignments { get; set; }
    public virtual DbSet<Course> Courses { get; set; }
    public virtual DbSet<Enrollment> Enrollments { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<Submission> Submissions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.AssignmentId);
            entity.Property(e => e.Title).HasMaxLength(160);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.DueDate).HasColumnType("datetime2");
            entity.HasCheckConstraint("CK_Assignments_MaxPoints", "[MaxPoints] > 0");

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignments_Courses");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId);
            entity.HasIndex(e => e.Code).IsUnique().HasDatabaseName("UQ_Courses_Code");
            entity.Property(e => e.Code).HasMaxLength(20);
            entity.Property(e => e.Name).HasMaxLength(160);
            entity.HasCheckConstraint("CK_Courses_Credits", "[Credits] BETWEEN 1 AND 10");
        });

        modelBuilder.Entity<Enrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId);
            entity.HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique().HasDatabaseName("UQ_Enrollments_Student_Course");
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.HasCheckConstraint("CK_Enrollments_Status", "[Status] IN (N'Active', N'Completed', N'Dropped')");

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Students");

            entity.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Enrollments_Courses");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId);
            entity.HasIndex(e => e.IndexNumber).IsUnique().HasDatabaseName("UQ_Students_IndexNumber");
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("UQ_Students_Email");
            entity.Property(e => e.IndexNumber).HasMaxLength(20);
            entity.Property(e => e.FirstName).HasMaxLength(80);
            entity.Property(e => e.LastName).HasMaxLength(80);
            entity.Property(e => e.Email).HasMaxLength(160);
            entity.Property(e => e.EnrollmentDate).HasColumnType("date");
        });

        modelBuilder.Entity<Submission>(entity =>
        {
            entity.HasKey(e => e.SubmissionId);
            entity.HasIndex(e => new { e.AssignmentId, e.StudentId })
                .IsUnique().HasDatabaseName("UQ_Submissions_Assignment_Student");
            entity.Property(e => e.RepositoryUrl).HasMaxLength(300);
            entity.Property(e => e.Feedback).HasMaxLength(1000);
            entity.Property(e => e.Status).HasMaxLength(30);
            entity.Property(e => e.SubmittedAt).HasColumnType("datetime2");
            entity.HasCheckConstraint("CK_Submissions_Status", "[Status] IN (N'Submitted', N'Late', N'Graded')");
            entity.HasCheckConstraint("CK_Submissions_Score", "[Score] IS NULL OR [Score] >= 0");

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submissions_Assignments");

            entity.HasOne(e => e.Student)
                .WithMany(s => s.Submissions)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Submissions_Students");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
