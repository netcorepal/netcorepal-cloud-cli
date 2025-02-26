using System.ComponentModel.DataAnnotations;
using Cocona;
using NetCorePal.Cloud.CLI.Toolkit.Validator;

namespace NetCorePal.Cloud.CLI.Toolkit.CommonParameters;

// ReSharper disable once ClassNeverInstantiated.Global
public record GenerationCommonParameters(
    [Option('n', Description = "名称")]
    [Required(ErrorMessage = "名称不能为空")]
    string Name,
    [Option('d', Description = "输出目录（可选，默认为当前目录）")]
    [DirectoryExists]
    string? OutputDirectory = null) : ICommandParameterSet;