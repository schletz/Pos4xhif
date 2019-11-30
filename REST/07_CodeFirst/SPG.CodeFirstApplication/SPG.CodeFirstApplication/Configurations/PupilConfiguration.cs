using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPG.CodeFirstApplication.Entities;
using System;

namespace SPG.CodeFirstApplication.Configurations
{
    public class PupilConfiguration : IEntityTypeConfiguration<Pupil>
    {
        public void Configure(EntityTypeBuilder<Pupil> builder)
        {
            builder.ToTable("Pupils");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id).IsRequired();
            builder.Property(c => c.FirstName).HasMaxLength(250).IsRequired();
            builder.Property(c => c.LastName).HasMaxLength(250).IsRequired();

            builder.HasData(
                new Pupil() { Id = new Guid("26a76d85-7577-4b53-abd1-4aca501a3f68"), FirstName = "Vorname 1", LastName = "Nachname 1", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("5699f9fe-4f2d-4c00-b226-007e0ff42ca7"), FirstName = "Vorname 2", LastName = "Nachname 2", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("3404ce31-e751-44cb-b84a-3b318a017176"), FirstName = "Vorname 3", LastName = "Nachname 3", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("1b71952b-4695-4741-92a0-b2fb9dfa6851"), FirstName = "Vorname 4", LastName = "Nachname 4", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("7b66c7c6-2898-456e-95bf-37af3f97e799"), FirstName = "Vorname 5", LastName = "Nachname 5", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("cf3bd38f-3e4f-4202-a6ee-102e00c03a2a"), FirstName = "Vorname 6", LastName = "Nachname 6", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("65549b37-0de2-4549-9a0c-90311bad52f1"), FirstName = "Vorname 7", LastName = "Nachname 7", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("d69d01ba-afed-40ea-b54e-63c0fbd25abd"), FirstName = "Vorname 8", LastName = "Nachname 8", Gender = "M", SchoolClassId = new Guid("75d42b58-c4c6-4380-9f8b-bacdcf8e03ee") },
                new Pupil() { Id = new Guid("a16970d2-2a84-4a76-8991-0ecec1eeb1c8"), FirstName = "Vorname 9", LastName = "Nachname 9", Gender = "M", SchoolClassId = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b") },
                new Pupil() { Id = new Guid("38cf31d8-a3a9-4e4d-86d9-aacb52c52c1b"), FirstName = "Vorname 10", LastName = "Nachname 10", Gender = "M", SchoolClassId = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b") },
                new Pupil() { Id = new Guid("ef2e0d3d-91b8-44c1-b335-183a82f517b2"), FirstName = "Vorname 11", LastName = "Nachname 11", Gender = "M", SchoolClassId = new Guid("ac87ce7b-89bd-434f-a800-b2979d745c1b") },
                new Pupil() { Id = new Guid("ad4ab73a-8cba-4ad6-9079-3f546c0f8589"), FirstName = "Vorname 12", LastName = "Nachname 12", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") },
                new Pupil() { Id = new Guid("fcd40d7b-b676-43a9-bec5-1d2f9301a450"), FirstName = "Vorname 13", LastName = "Nachname 13", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") },
                new Pupil() { Id = new Guid("7893a991-cb8c-457b-84b6-87329f70d9b6"), FirstName = "Vorname 14", LastName = "Nachname 14", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") },
                new Pupil() { Id = new Guid("65dc791d-8109-4f63-9bf2-fab745af866d"), FirstName = "Vorname 15", LastName = "Nachname 15", Gender = "M", SchoolClassId = new Guid("1712daf8-bf01-4f88-905b-74ec9498d077") }
                );
        }
    }
}
