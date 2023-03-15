using System.ComponentModel.DataAnnotations.Schema;
using EntityFrameworkGenericRepository.Attributes;
using EntityFrameworkGenericRepository.Entities;

namespace EntityFrameworkGenericRepositoryImplementation.DAL.Entities;

public sealed class Person : BaseEntity<int>
{
    public override int Id { get; set; }
    public string Name { get; set; }

    public int? ParentId { get; set; }

    [Include]
    [ForeignKey("ParentId")]
    public Person Parent { get; set; }

    [Include]
    public List<Person> Children { get; set; }

    public Person(int id, string name, int? parentId = null)
    {
        Id = id;
        Name = name;
        ParentId = parentId;
    }

    public Person(string name, int? parentId = null)
    {
        Name = name;
        ParentId = parentId;
    }
}