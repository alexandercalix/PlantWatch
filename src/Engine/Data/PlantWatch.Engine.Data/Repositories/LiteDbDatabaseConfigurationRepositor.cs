using System;
using LiteDB;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;
namespace PlantWatch.Engine.Data.Repositories;

public class LiteDbDatabaseConfigurationRepository : IDatabaseConfigurationRepository
{
    private readonly string _dbPath;
    private readonly string _password;
    private readonly string _collectionName = "db_configs";

    public LiteDbDatabaseConfigurationRepository(string filePath, string password)
    {
        _dbPath = filePath;
        _password = password;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        if (!File.Exists(_dbPath))
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<DatabaseConfigDocument>(_collectionName);
            col.EnsureIndex(x => x.Id, true);
            col.EnsureIndex(x => x.Name, true);
        }
    }

    private string BuildConnectionString() => $"Filename={_dbPath};Password={_password};";

    public async Task<IEnumerable<DatabaseConfig>> LoadAllDatabaseConfigurationsAsync()
    {
        return await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<DatabaseConfigDocument>(_collectionName);
            return col.FindAll().Select(ToModel).ToList();
        });
    }

    public async Task<DatabaseConfig?> GetDatabaseConfigurationAsync(Guid id)
    {
        return await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<DatabaseConfigDocument>(_collectionName);
            var doc = col.FindOne(x => x.Id == id);
            return doc == null ? null : ToModel(doc);
        });
    }

    public async Task SaveDatabaseConfigurationAsync(DatabaseConfig config)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<DatabaseConfigDocument>(_collectionName);
            var doc = ToDocument(config);
            col.Upsert(doc);
        });
    }

    public async Task DeleteDatabaseConfigurationAsync(Guid id)
    {
        await Task.Run(() =>
        {
            using var db = new LiteDatabase(BuildConnectionString());
            var col = db.GetCollection<DatabaseConfigDocument>(_collectionName);
            col.DeleteMany(x => x.Id == id);
        });
    }

    // --- Mapping ---
    private static DatabaseConfig ToModel(DatabaseConfigDocument doc) => new()
    {
        Id = doc.Id,
        Name = doc.Name,
        DriverType = doc.DriverType,
        Parameters = doc.Parameters
    };

    private static DatabaseConfigDocument ToDocument(DatabaseConfig model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        DriverType = model.DriverType,
        Parameters = model.Parameters
    };
}
