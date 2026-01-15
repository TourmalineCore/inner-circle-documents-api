using Core;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

public class DocumentsDbContext : DbContext
{
  public required DbSet<Document> Documents { get; set; }

  public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : base(options)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    base.OnModelCreating(modelBuilder);
  }
}
