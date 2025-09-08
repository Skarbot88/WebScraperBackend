using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

/// <summary>
/// Entity Framework DbContext for SEO Tracker
/// </summary>
public class WebScraperContext : DbContext
{
    public WebScraperContext(DbContextOptions<WebScraperContext> options) : base(options)
    {
    }

    public DbSet<SearchResult> SearchResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure SearchResult entity
        modelBuilder.Entity<SearchResult>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
            entity.Property(e => e.SearchTerm)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.TargetUrl)
                .IsRequired()
                .HasMaxLength(1000);
                
            entity.Property(e => e.Positions)
                .HasColumnType("text");
                
            entity.Property(e => e.SearchDate)
                .IsRequired()
                .HasColumnType("timestamp with time zone");
                
            entity.Property(e => e.TotalResults);

            // Indexes for better query performance
            entity.HasIndex(e => e.SearchTerm)
                .HasDatabaseName("IX_SearchResults_SearchTerm");
                
            entity.HasIndex(e => e.SearchDate)
                .HasDatabaseName("IX_SearchResults_SearchDate");
                
            entity.HasIndex(e => new { e.SearchTerm, e.SearchDate })
                .HasDatabaseName("IX_SearchResults_SearchTerm_SearchDate");
        });
    }
}