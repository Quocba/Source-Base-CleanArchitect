using Infrastructure.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQContract.Generic;
using System;
using System.Text.Json;

public class DbActionConsumer : IConsumer<DbActionMessage>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public DbActionConsumer(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task Consume(ConsumeContext<DbActionMessage> context)
    {
        var msg = context.Message;
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DBContext>();

        var type = Type.GetType(msg.EntityType);
        if (type == null) return;

        var entity = JsonSerializer.Deserialize(msg.Payload, type);
        if (entity == null) return;

        db.ChangeTracker.Clear();

        switch (msg.Action)
        {
            case "Add": db.Entry(entity).State = EntityState.Added; break;
            case "Update": db.Entry(entity).State = EntityState.Modified; break;
            case "Delete": db.Entry(entity).State = EntityState.Deleted; break;
        }

        await db.SaveChangesAsync();
        Console.WriteLine($"✅ [DB Queue] {msg.Action} {msg.EntityType}");
    }

}
