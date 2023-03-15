namespace EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

public record PersonFilter(int? Id, string? Name, int? ParentId, string? ParentName, int? ChildrenAmount, string? ChildName);