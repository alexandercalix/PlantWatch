namespace PlantWatch.Engine.Core.Data.Interfaces;

public interface ICommandValidator
{
    bool Validate(string command);
}
