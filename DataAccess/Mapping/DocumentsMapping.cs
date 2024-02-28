using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NodaTime;
using Core;

namespace DataAccess.Mapping;

public class DocumentsMapping : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        var instantConverter =
        new ValueConverter<Instant, DateTime>(v =>
         v.ToDateTimeUtc(),
         v => Instant.FromDateTimeUtc(v));

        //builder.Property(e => e.DateCreateDocument)
        //    .HasConversion(instantConverter);
        //builder.Property(e => e.DateDocument)
        //    .HasConversion(instantConverter);
    }
}
