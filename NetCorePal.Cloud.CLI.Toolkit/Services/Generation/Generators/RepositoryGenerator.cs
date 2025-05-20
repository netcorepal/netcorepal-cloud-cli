using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;

public class RepositoryGenerator(
    ICodeGenerationHelper codeGenerator,
    ILogger<RepositoryGenerator> logger)
    : GenerationBase(codeGenerator, logger)
{
    public void Generate(
        GenerationCommonParameters parameters,
        string dbContextType = "ApplicationDbContext")
    {
        GenerateCore(parameters, namespaceValue =>
        {
            var idName = $"{parameters.Name}Id";
            return $$"""
                     using NetCorePal.Extensions.Repository;
                     using NetCorePal.Extensions.Repository.EntityFrameworkCore;

                     namespace {{namespaceValue}};

                     public interface I{{parameters.Name}}Repository : IRepository<{{parameters.Name}}, {{idName}}>;

                     public class {{parameters.Name}}Repository({{dbContextType}} context) 
                         : RepositoryBase<{{parameters.Name}}, {{idName}}, {{dbContextType}}>(context), 
                           I{{parameters.Name}}Repository
                     {
                     }
                     """;
        }, "Repository");
    }
}