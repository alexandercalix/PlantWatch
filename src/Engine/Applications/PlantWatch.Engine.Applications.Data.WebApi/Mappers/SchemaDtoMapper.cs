using System;
using PlantWatch.Engine.Applications.Data.WebApi.DTOs;
using PlantWatch.Engine.Core.Data.Models.Schema;

namespace PlantWatch.Engine.Applications.Data.WebApi.Mappers;

public static class SchemaDtoMapper
{
    public static List<DataStructureDto> Map(DatabaseSchema schema)
    {
        return schema.Tables.Select(table => new DataStructureDto
        {
            EntityName = table.Name,
            EntityType = table.Type,
            Fields = table.Fields.Select(field => new FieldDescriptorDto
            {
                FieldName = field.Name,
                DataType = field.Type,
                IsNullable = field.IsNullable,
                MaxLength = field.MaxLength,
                Precision = field.Precision,
                Scale = field.Scale,
                IsPrimaryKey = field.IsPrimaryKey,
                IsTag = field.IsTag,
                IsIndexed = field.IsIndexed
            }).ToList(),
            References = table.References.Select(r => new ReferenceDescriptorDto
            {
                SourceField = $"{table.Name}.{r.Field}",
                TargetEntity = r.ReferencedTable,
                TargetField = $"{r.ReferencedTable}.{r.ReferencedField}"
            }).ToList()
        }).ToList();
    }
}
