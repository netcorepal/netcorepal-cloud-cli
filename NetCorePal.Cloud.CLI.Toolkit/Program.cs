using Cocona;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.Commands;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;
using NetCorePal.Cloud.CLI.Toolkit.Utils;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

var builder = CoconaApp.CreateBuilder(
    configureOptions: options =>
    {
        options.EnableShellCompletionSupport = true;
        options.TreatPublicMethodsAsCommands = false;
    });

builder.Services
    .AddLogging(logging => logging.AddConsole())
    .AddSingleton<IProjectFinder, ProjectFinder>()
    .AddSingleton<ICodeGenerationHelper, CodeGenerationHelper>()
    .AddScoped<AggregateRootGenerator>()
    .AddScoped<CommandGenerator>()
    .AddScoped<RepositoryGenerator>()
    .AddScoped<DomainEventGenerator>()
    .AddScoped<DomainEventHandlerGenerator>();

var app = builder.Build();

app.AddSubCommand("new", x => x.AddCommands<GenerationCommands>())
    .WithDescription("代码模板生成");
;

app.Run();