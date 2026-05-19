namespace UniversityTasksDbFirstApi.DTOs;

public class StudentDashboardDto
{
    public int StudentId { get; set; }
    public string IndexNumber { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<EnrollmentInfoDto> Enrollments { get; set; } = new();
    public List<SubmissionInfoDto> Submissions { get; set; } = new();
}

public class EnrollmentInfoDto
{
    public int EnrollmentId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string CourseCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateOnly EnrolledAt { get; set; }
}

public class SubmissionInfoDto
{
    public int SubmissionId { get; set; }
    public string AssignmentTitle { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Score { get; set; }
}
