﻿using Microsoft.EntityFrameworkCore;
using Pds.Core;
using Pds.Core.Enums;
using Pds.Data.Entities;
namespace Pds.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<Content> Contents { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<Cost> Costs { get; set; }
    public DbSet<Client> Clients { get; set; }

    public DbSet<Brand> Brands { get; set; }

    public DbSet<Gift> Gifts { get; set; }

    public DbSet<Gift> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        UpdateStructure(builder);
        SeedDate(builder);
        base.OnModelCreating(builder);
    }

    private void UpdateStructure(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .Property(b => b.Status)
            .HasDefaultValue(PersonStatus.Active);

        modelBuilder.Entity<Content>()
            .Property(b => b.SocialMediaType);

        modelBuilder.Entity<Content>()
            .HasOne(a => a.Bill)
            .WithOne(a => a.Content)
            .HasForeignKey<Bill>(c => c.ContentId);
    }

    private void SeedDate(ModelBuilder builder)
    {
        var brands = new List<Brand>
        {
            new()
            {
                Id = Guid.Parse("5AA23FA2-4B73-4A3F-C3D4-08D8D2705C5F"),
                Name = "АйТиБорода",
                Url = "https://youtube.com/itbeard"
            },
            new()
            {
                Id = Guid.Parse("6BB23FA2-4B73-4A3F-C3D4-08D8D2705C5F"),
                Name = "Тёмный Лес",
                Url = "https://youtube.com/thedarkless"
            }
        };

        var settins = new List<Setting>
        {
            new()
            {
                Id = Guid.Parse("0BB23FA2-4B73-4A3F-C3D4-08D8D2705C5F"),
                Key = SettingsKeys.ExternalLink1Title,
                Value = "Link #1"
            },
            new()
            {
                Id = Guid.Parse("1BB23FA2-4B73-4A3F-C3D4-08D8D2705C5F"),
                Key = SettingsKeys.ExternalLink1Url,
                Value = "https://google.com"
            },
            new()
            {
                Id = Guid.Parse("2BB23FA2-4B73-4A3F-C3D4-08D8D2705C5F"),
                Key = SettingsKeys.ExternalLink2Title,
                Value = "Link #2"
            },
            new()
            {
                Id = Guid.Parse("3BB23FA2-4B73-4A3F-C3D4-08D8D2705C5F"),
                Key = SettingsKeys.ExternalLink2Url,
                Value = "https://youtube.com"
            }
        };

        builder.Entity<Brand>().HasData(brands.ToArray());
        builder.Entity<Setting>().HasData(settins.ToArray());
    }
}