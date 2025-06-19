using System;
using LiteDB;
using PlantWatch.Core.Interfaces;
using PlantWatch.Core.Models.Definitions;
using PlantWatch.DriverRuntime.Models.Database;


namespace PlantWatch.DriverRuntime.Repositories;

public class LiteDbConfigurationRepository : IConfigurationRepository
{
    private readonly string _dbPath;
    private readonly string _password;
    private readonly string _collectionName = "plc_drivers";

    public LiteDbConfigurationRepository(string filePath, string password)
    {
        _dbPath = filePath;
        _password = password;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (!File.Exists(_dbPath))
        {
            Console.WriteLine($"[LiteDB] Creating new database at {_dbPath}");
            using var db = new LiteDatabase(BuildConnectionString());
            var collection = db.GetCollection<PlcConnectionDocument>(_collectionName);
            collection.EnsureIndex(x => x.Id, true);
            collection.EnsureIndex(x => x.Name, true);
        }
    }

    private string BuildConnectionString() => $"Filename={_dbPath};Password={_password};";

    // -------------- PLC LEVEL --------------

    public async Task<IEnumerable<PlcConnectionDefinition>> LoadAllPlcConfigurationsAsync()
    {
        return await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            return col.FindAll().Select(ToModel).ToList();
        });
    }

    public async Task<PlcConnectionDefinition> GetPlcConfigurationAsync(Guid plcId)
    {
        return await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = col.FindOne(x => x.Id == plcId);
            return doc == null ? null : ToModel(doc);
        });
    }

    public async Task SavePlcConfigurationAsync(PlcConnectionDefinition config)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = ToDocument(config);
            col.Upsert(doc);
        });
    }

    public async Task DeletePlcConfigurationAsync(Guid plcId)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            col.DeleteMany(x => x.Id == plcId);
        });
    }

    // -------------- TAG LEVEL --------------

    public async Task<IEnumerable<PlcTagDefinition>> LoadTagsAsync(Guid plcId)
    {
        var plc = await GetPlcConfigurationAsync(plcId);
        return plc?.Tags ?? Enumerable.Empty<PlcTagDefinition>();
    }

    public async Task<PlcTagDefinition> GetTagAsync(Guid plcId, Guid tagId)
    {
        var tags = await LoadTagsAsync(plcId);
        return tags.FirstOrDefault(t => t.Id == tagId);
    }

    public async Task AddOrUpdateTagAsync(Guid plcId, PlcTagDefinition tag)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = col.FindOne(x => x.Id == plcId);
            if (doc == null)
                throw new InvalidOperationException($"PLC ID '{plcId}' not found.");

            doc.Tags.RemoveAll(t => t.Id == tag.Id);
            doc.Tags.Add(new PlcTagDocument
            {
                Id = tag.Id,
                Name = tag.Name,
                Datatype = tag.Datatype,
                Address = tag.Address
            });

            col.Update(doc);
        });
    }

    public async Task DeleteTagAsync(Guid plcId, Guid tagId)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = col.FindOne(x => x.Id == plcId);
            if (doc == null)
                return;

            doc.Tags.RemoveAll(t => t.Id == tagId);
            col.Update(doc);
        });
    }

    // -------------- MAPPING --------------

    private PlcConnectionDocument ToDocument(PlcConnectionDefinition model)
    {
        return new PlcConnectionDocument
        {
            Id = model.Id,  // <-- AQUI AHORA GUARDAMOS EL ID
            Name = model.Name,
            DriverType = model.DriverType,
            IpAddress = model.IpAddress,
            Rack = model.Rack,
            Slot = model.Slot,
            Tags = model.Tags.Select(t => new PlcTagDocument
            {
                Id = t.Id,
                Name = t.Name,
                Datatype = t.Datatype,
                Address = t.Address
            }).ToList()
        };
    }

    private PlcConnectionDefinition ToModel(PlcConnectionDocument doc)
    {
        return new PlcConnectionDefinition
        {
            Id = doc.Id,
            Name = doc.Name,
            DriverType = doc.DriverType,
            IpAddress = doc.IpAddress,
            Rack = doc.Rack,
            Slot = doc.Slot,
            Tags = doc.Tags.Select(t => new PlcTagDefinition
            {
                Id = t.Id,
                Name = t.Name,
                Datatype = t.Datatype,
                Address = t.Address
            }).ToList()
        };
    }
}