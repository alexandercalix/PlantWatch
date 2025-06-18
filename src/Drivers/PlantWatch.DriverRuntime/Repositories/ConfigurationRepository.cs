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
            collection.EnsureIndex(x => x.Name, true);
        }
    }

    private string BuildConnectionString() => $"Filename={_dbPath};Password=your-strong-password;";

    // ----------- PLC Level --------------

    public async Task<IEnumerable<PlcConnectionDefinition>> LoadAllPlcConfigurationsAsync()
    {
        return await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            return col.FindAll().Select(ToModel).ToList();
        });
    }

    public async Task<PlcConnectionDefinition> GetPlcConfigurationAsync(string plcName)
    {
        return await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = col.FindOne(x => x.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
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

    public async Task DeletePlcConfigurationAsync(string plcName)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            col.DeleteMany(x => x.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
        });
    }

    // ----------- TAG Level --------------

    public async Task<IEnumerable<PlcTagDefinition>> LoadTagsAsync(string plcName)
    {
        var plc = await GetPlcConfigurationAsync(plcName);
        return plc?.Tags ?? Enumerable.Empty<PlcTagDefinition>();
    }

    public async Task<PlcTagDefinition> GetTagAsync(string plcName, Guid tagId)
    {
        var tags = await LoadTagsAsync(plcName);
        return tags.FirstOrDefault(t => t.Id == tagId);
    }

    public async Task AddOrUpdateTagAsync(string plcName, PlcTagDefinition tag)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = col.FindOne(x => x.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
            if (doc == null)
                throw new InvalidOperationException($"PLC '{plcName}' not found.");

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

    public async Task DeleteTagAsync(string plcName, Guid tagId)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<PlcConnectionDocument>(_collectionName);
            var doc = col.FindOne(x => x.Name.Equals(plcName, StringComparison.OrdinalIgnoreCase));
            if (doc == null)
                return;

            doc.Tags.RemoveAll(t => t.Id == tagId);
            col.Update(doc);
        });
    }

    // ----------- Mapping -----------

    private PlcConnectionDocument ToDocument(PlcConnectionDefinition model)
    {
        return new PlcConnectionDocument
        {
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