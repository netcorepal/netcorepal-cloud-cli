using Cocona;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Const;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;

namespace NetCorePal.Cloud.CLI.Toolkit.Commands;

public class GenerationCommands(
    AggregateRootGenerator aggregateRootGenerator,
    CommandGenerator commandGenerator,
    RepositoryGenerator repositoryGenerator,
    DomainEventGenerator domainEventGenerator,
    DomainEventHandlerGenerator domainEventHandlerGenerator)
{
    [Command("ar", Description = "生成聚合根`{name}.cs`文件")]
    public void GenerateAggregateRoot(
        GenerationCommonParameters parameters,
        [Option('i', Description = "强类型ID类型")] StrongIdType idType = StrongIdType.Int64)
    {
        aggregateRootGenerator.Generate(parameters, idType);
    }

    [Command("cmd", Description = "生成命令`{name}Command.cs`文件,包含验证器及处理器")]
    public void GenerateCommand(
        GenerationCommonParameters parameters,
        [Option('r', Description = "命令处理器返回值类型（可选）")]
        string? returnType = null)
    {
        commandGenerator.Generate(parameters, returnType);
    }

    [Command("repo", Description = "生成仓储`{name}Repository.cs`文件")]
    public void GenerateRepository(
        GenerationCommonParameters parameters,
        [Option('t', Description = "DbContext类型（可选，默认为`ApplicationDbContext`）")]
        string dbContextType = "ApplicationDbContext")
    {
        repositoryGenerator.Generate(parameters, dbContextType);
    }

    [Command("de", Description = "生成领域事件`{name}DomainEvent.cs`文件")]
    public void GenerateDomainEvent(GenerationCommonParameters parameters)
    {
        domainEventGenerator.Generate(parameters);
    }

    [Command("deh", Description = "生成领域事件处理器`{name}.cs`文件")]
    public void GenerateDomainEventHandler(
        GenerationCommonParameters parameters,
        [Option('t', Description = "领域事件类型")] string domainEventType)
    {
        domainEventHandlerGenerator.Generate(parameters, domainEventType);
    }
}