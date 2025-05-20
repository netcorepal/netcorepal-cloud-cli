using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;

public class DomainEventGenerator(
    ICodeGenerationHelper codeGenerator,
    ILogger<DomainEventGenerator> logger)
    : GenerationBase(codeGenerator, logger)
{
    public void Generate(GenerationCommonParameters parameters)
    {
        GenerateCore(parameters, namespaceValue => $"""
                                                    using NetCorePal.Extensions.Domain;
                                                    using NetCorePal.Extensions.Primitives;

                                                    namespace {namespaceValue};

                                                    public record {parameters.Name}DomainEvent() : IDomainEvent;
                                                    """, "DomainEvent");
    }
}