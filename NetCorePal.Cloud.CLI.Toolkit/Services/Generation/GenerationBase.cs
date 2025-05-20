using System.Text;
using Microsoft.Extensions.Logging;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Utils;

namespace NetCorePal.Cloud.CLI.Toolkit.Services.Generation;

public abstract class GenerationBase(CodeGenerationHelper codeGenerationHelper, ILogger logger)
{
    protected void GenerateCore(
        GenerationCommonParameters parameters,
        Func<string, string> contentGenerator,
        string fileSuffix = "")
    {
        try
        {
            var outputDir = Path.GetFullPath(parameters.OutputDirectory ?? Directory.GetCurrentDirectory());

            var fileName = $"{parameters.Name}{fileSuffix}.cs";
            var fullPath = Path.Combine(outputDir, fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            if (File.Exists(fullPath))
            {
                logger.LogError("⚠️ 文件 {filePath} 已存在", fullPath);
                return;
            }

            var namespaceValue = codeGenerationHelper.DetermineNamespace(outputDir);
            var content = contentGenerator(namespaceValue);

            File.WriteAllText(fullPath, content, new UTF8Encoding(true));

            if (!File.Exists(fullPath)) throw new FileNotFoundException("文件生成失败，请检查写入权限");

            logger.LogInformation("📁 文件生成路径: {FullPath}", fullPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "❌ 生成 {ObjectType} 失败", GetType().Name.Replace("Generator", ""));
        }
    }
}