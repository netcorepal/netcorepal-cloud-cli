using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Utils;

namespace NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;

public class DomainEventHandlerGenerator(
    CodeGenerationHelper codeGenerator,
    ILogger<DomainEventHandlerGenerator> logger)
    : GenerationBase(codeGenerator, logger)
{
    public void Generate(
        GenerationCommonParameters parameters,
        string domainEventType)
    {
        GenerateCore(parameters, namespaceValue => $$"""
                                                     using MediatR;
                                                     using NetCorePal.Extensions.Domain;
                                                     using NetCorePal.Extensions.Primitives;

                                                     namespace {{namespaceValue}};

                                                     public class {{parameters.Name}}(IMediator mediator) 
                                                         : IDomainEventHandler<{{domainEventType}}>
                                                     {
                                                         public async Task Handle({{domainEventType}} notification, 
                                                             CancellationToken cancellationToken)
                                                         {
                                                             // 实现业务逻辑
                                                             throw new NotImplementedException();
                                                         }
                                                     }
                                                     """, "DomainEventHandler");
    }
}