using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Utils;

namespace NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;

public class CommandGenerator(
    CodeGenerationHelper codeGenerator,
    ILogger<CommandGenerator> logger)
    : GenerationBase(codeGenerator, logger)
{
    public void Generate(
        GenerationCommonParameters parameters,
        string? returnType = null)
    {
        GenerateCore(parameters, namespaceValue =>
        {
            var (genericPart1, genericPart2) = returnType is null
                ? (string.Empty, string.Empty)
                : ($"<{returnType}>", $",<{returnType}>");

            return $$"""
                     using FluentValidation;
                     using NetCorePal.Extensions.Primitives;

                     namespace {{namespaceValue}};

                     public record {{parameters.Name}}Command() : ICommand{{genericPart1}};

                     public class {{parameters.Name}}CommandValidator : AbstractValidator<{{parameters.Name}}Command>
                     {
                         public {{parameters.Name}}CommandValidator()
                         {
                             // 添加验证规则示例：
                             // RuleFor(x => x.Property).NotEmpty();
                         }
                     }

                     public class {{parameters.Name}}CommandHandler : ICommandHandler<{{parameters.Name}}Command{{genericPart2}}>
                     {
                         public async Task{{genericPart1}} Handle(
                             {{parameters.Name}}Command request, 
                             CancellationToken cancellationToken)
                         {
                             // 实现业务逻辑
                             throw new NotImplementedException();
                         }
                     }
                     """;
        }, "Command");
    }
}