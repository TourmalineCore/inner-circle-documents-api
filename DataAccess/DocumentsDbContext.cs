using System.Collections.Generic;
using System.Reflection.Emit;
using Core;
using Microsoft.EntityFrameworkCore;

namespace DataAccess;

//Use next command in Package Manager Console to update Dev env DB
//PM> $env:ASPNETCORE_ENVIRONMENT = 'Debug'; Update-Database
public class DocumentsDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }

    public DocumentsDbContext(DbContextOptions<DocumentsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
