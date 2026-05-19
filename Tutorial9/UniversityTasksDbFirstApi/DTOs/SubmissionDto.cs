namespace UniversityTasksDbFirstApi.DTOs;

public class SubmissionDto
{
    public int SubmissionId { get; set; }
    public string StudentFullName { get; set; } = string.Empty;
    public string StudentIndexNumber { get; set; } = string.Empty;
    public string AssignmentTitle { get; set; } = string.Empty;
    public string RepositoryUrl { get; set; } = string.Empty;
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? Score { get; set; }
    public string? Feedback { get; set; }
}

public class CreateSubmissionDto
{
    public int AssignmentId { get; set; }
    public int StudentId { get; set; }
    public string RepositoryUrl { get; set; } = string.Empty;
}

public class GradeSubmissionDto
{
    public int Score { get; set; }
    public string? Feedback { get; set; }
}
