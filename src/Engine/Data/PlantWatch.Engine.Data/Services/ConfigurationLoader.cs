using System;
using System.Text.Json;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Data.Services;

public static class ConfigurationLoader
{
    public static AppConfig LoadFromJson(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException("Config file not found", path);

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<AppConfig>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new AppConfig();
    }
}