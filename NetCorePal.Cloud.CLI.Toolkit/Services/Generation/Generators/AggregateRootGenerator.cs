using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Const;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;

public class AggregateRootGenerator(
    ICodeGenerationHelper codeGenerator,
    ILogger<AggregateRootGenerator> logger)
    : GenerationBase(codeGenerator, logger)
{
    public void Generate(
        GenerationCommonParameters parameters,
        StrongIdType idType = StrongIdType.Int64)
    {
        GenerateCore(parameters, namespaceValue =>
        {
            var idName = $"{parameters.Name}Id";
            return $$"""
                     using NetCorePal.Extensions.Domain;
                     using NetCorePal.Extensions.Primitives;

                     namespace {{namespaceValue}};

                     public partial record {{idName}} : I{{idType}}StronglyTypedId;

                     public class {{parameters.Name}} : Entity<{{idName}}>, IAggregateRoot
                     {
                         protected {{parameters.Name}}() { }
                     }
                     """;
        });
    }
}