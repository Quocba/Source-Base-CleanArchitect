using Domain.Entities.Enum;
using Infrastructure.Context;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQContract.Generic.Request;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQContract.Generic
{
    public class GenericQueueConsumer : IConsumer<GenericQueueMessage>
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<GenericQueueConsumer> _logger;

        public GenericQueueConsumer(IServiceScopeFactory scopeFactory, ILogger<GenericQueueConsumer> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GenericQueueMessage> context)
        {
            var msg = context.Message;

            var entityType = ResolveType(msg.EntityType);
            if (entityType == null)
            {
                _logger.LogError($"❌ Không tìm thấy EntityType: {msg.EntityType}");
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DBContext>();

            db.ChangeTracker.Clear();

            var token = JToken.Parse(msg.PayloadJson);
            var jsonArray = token.Type == JTokenType.Array ? (JArray)token : new JArray(token);

            switch (msg.ActionType)
            {
                case QueueActionType.Add:
                    foreach (var j in jsonArray)
                    {
                        var entity = j.ToObject(entityType, JsonSerializer.Create());
                        if (entity != null)
                            await db.AddAsync(entity);
                    }
                    break;

                case QueueActionType.Update:
                    foreach (var j in jsonArray)
                    {
                        var entity = j.ToObject(entityType, JsonSerializer.Create());
                        if (entity != null)
                            db.Update(entity);
                    }
                    break;

                case QueueActionType.Delete:
                    var keyProp = db.Model.FindEntityType(entityType)?.FindPrimaryKey()?.Properties?.FirstOrDefault();
                    if (keyProp == null)
                    {
                        _logger.LogError("❌ Không tìm thấy khóa chính cho entity " + entityType.Name);
                        return;
                    }

                    foreach (var j in jsonArray)
                    {
                        var idToken = j[keyProp.Name] ?? j["Id"] ?? j["ID"] ?? j["id"];
                        if (idToken == null) continue;

                        _logger.LogInformation($"DEBUG: idToken Type: {idToken.Type}, Value: '{idToken}', ToString: '{idToken.ToString()}', ClrType: {keyProp.ClrType}");
                        var idVal = Convert.ChangeType(idToken.ToString(), keyProp.ClrType);
                        var entity = await db.FindAsync(entityType, idVal);

                        if (entity != null)
                        {
                            db.Remove(entity);
                            _logger.LogInformation($"🗑 Xóa {entityType.Name} (PK={idVal})");
                        }
                        else
                        {
                            _logger.LogWarning($"⚠️ Không tìm thấy {entityType.Name} (PK={idVal}) để xóa");
                        }
                    }
                    break;
            }

            try
            {
                var affected = await db.SaveChangesAsync();
                _logger.LogInformation($"✅ Queue {msg.ActionType} => {entityType.Name} (Affected: {affected})");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError($"💥 Lỗi khi SaveChanges cho {entityType.Name}: {dbEx.InnerException?.Message ?? dbEx.Message}");
                // Log more details
                foreach (var entry in db.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged))
                {
                    var pk = entry.Metadata.FindPrimaryKey();
                    var keyString = pk != null
                        ? string.Join(", ", pk.Properties.Select(p => $"{p.Name}={entry.Property(p.Name).CurrentValue}"))
                        : "N/A";
                    _logger.LogError($"Entity: {entry.Entity.GetType().Name}, State: {entry.State}, Key: {keyString}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"💥 Lỗi không xác định cho {entityType.Name}: {ex.Message}");
            }
        }

        // Resolve type an toàn (kể cả khi khác assembly)
        private static Type? ResolveType(string typeName)
        {
            var t = Type.GetType(typeName);
            if (t != null) return t;

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    t = asm.GetType(typeName, throwOnError: false, ignoreCase: false);
                    if (t != null) return t;
                }
                catch { }
            }

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    t = asm.GetTypes().FirstOrDefault(x =>
                        x.FullName == typeName ||
                        x.Name == typeName ||
                        x.AssemblyQualifiedName == typeName);
                    if (t != null) return t;
                }
                catch { }
            }

            return null;
        }
    }
}