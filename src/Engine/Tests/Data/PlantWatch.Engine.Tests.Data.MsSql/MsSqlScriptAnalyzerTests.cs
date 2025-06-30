
using Moq;
using PlantWatch.Engine.Core.Data.Interfaces;
using PlantWatch.Engine.Core.Data.Models;

namespace PlantWatch.Engine.Tests.Data.MsSql;

public class MsSqlScriptAnalyzerTests
{
    [Fact]
    public async Task Returns_Empty_When_Query_Is_Empty()
    {
        var mock = new Mock<IScriptAnalyzer>();
        mock.Setup(a => a.AnalyzeSelectQueryAsync("")).ReturnsAsync(new List<SqlColumnInfo>());

        var result = await mock.Object.AnalyzeSelectQueryAsync("");

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Returns_Columns_For_Simple_Select()
    {
        var mock = new Mock<IScriptAnalyzer>();
        mock.Setup(a => a.AnalyzeSelectQueryAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SqlColumnInfo>
            {
                    new SqlColumnInfo { Column = "Id", DataType = "int", IsNullable = false, Table = "Users" },
                    new SqlColumnInfo { Column = "Name", DataType = "nvarchar", IsNullable = true, Table = "Users" }
            });

        var result = await mock.Object.AnalyzeSelectQueryAsync("SELECT Id, Name FROM Users");

        Assert.Equal(2, result.Count);
        Assert.Equal("Id", result[0].Column);
        Assert.Equal("nvarchar", result[1].DataType);
    }

    [Fact]
    public async Task Handles_Complex_Join_Query()
    {
        var mock = new Mock<IScriptAnalyzer>();
        mock.Setup(a => a.AnalyzeSelectQueryAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SqlColumnInfo>
            {
                    new SqlColumnInfo { Column = "Id", DataType = "int", IsNullable = false, Table = "Users" },
                    new SqlColumnInfo { Column = "OrderId", DataType = "int", IsNullable = false, Table = "Orders" }
            });

        var result = await mock.Object.AnalyzeSelectQueryAsync("SELECT u.Id, o.OrderId FROM Users u JOIN Orders o ON u.Id = o.UserId");

        Assert.Equal(2, result.Count);
        Assert.Contains(result, c => c.Column == "OrderId");
    }

    [Fact]
    public async Task Returns_Unknown_If_Metadata_Is_Missing()
    {
        var mock = new Mock<IScriptAnalyzer>();
        mock.Setup(a => a.AnalyzeSelectQueryAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<SqlColumnInfo>
            {
                    new SqlColumnInfo { Column = "Dummy", DataType = "unknown", IsNullable = false, Table = "unknown" }
            });

        var result = await mock.Object.AnalyzeSelectQueryAsync("SELECT 1 AS Dummy");

        Assert.Single(result);
        Assert.Equal("Dummy", result[0].Column);
        Assert.Equal("unknown", result[0].DataType);
    }

}
