using MassTransit;
using Microsoft.EntityFrameworkCore;
using RabbitMQContract.Generic;
using RabbitMQContract.Payload.Request;
using System.Text.Json;

public class QueueDbContext : DbContext
{
    private readonly IPublishEndpoint _publishEndpoint;

    public QueueDbContext(DbContextOptions options, IPublishEndpoint publishEndpoint)
        : base(options)
    {
        _publishEndpoint = publishEndpoint;
    }

    public override int SaveChanges()
    {
        PublishChangesAsync().GetAwaiter().GetResult();
        return 0; 
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .ToList();

        int result = await base.SaveChangesAsync(cancellationToken);

        foreach (var entry in entries)
        {
            string action = entry.State switch
            {
                EntityState.Added => "Add",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            };

            var message = new DbActionMessage
            {
                Action = action,
                EntityType = entry.Entity.GetType().AssemblyQualifiedName!, 
                Payload = JsonSerializer.Serialize(entry.Entity)
            };

            await _publishEndpoint.Publish(message);
        }

        Console.WriteLine($"[QueueDbContext] {entries.Count} thay đổi đã được lưu và gửi ra queue");
        return result;
    }

    private async Task PublishChangesAsync()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added ||
                        e.State == EntityState.Modified ||
                        e.State == EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            string action = entry.State switch
            {
                EntityState.Added => "Add",
                EntityState.Modified => "Update",
                EntityState.Deleted => "Delete",
                _ => "Unknown"
            };

            var message = new DbActionMessage
            {
                Action = action,
                EntityType = entry.Entity.GetType().FullName!,
                Payload = JsonSerializer.Serialize(entry.Entity)
            };

            await _publishEndpoint.Publish(message);
        }

        Console.WriteLine($"[QueueDbContext] {entries.Count} changes sent to queue");
    }
}
