using System.Text;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Utils;

/// <summary>
///     代码生成工具类
/// </summary>
public class CodeGenerationHelper(IProjectFinder projectFinder)
{
    private readonly IProjectFinder _projectFinder =
        projectFinder ?? throw new ArgumentNullException(nameof(projectFinder));

    /// <summary>
    ///     确定命名空间
    /// </summary>
    public string DetermineNamespace(string outputDirectory)
    {
        var projectDirectory = _projectFinder.FindProjectDirectory(outputDirectory);
        if (projectDirectory == null) throw new FileNotFoundException("未找到 .csproj 文件");

        var projectFile = _projectFinder.GetProjectFileName(projectDirectory);
        if (projectFile == null) throw new FileNotFoundException("未找到 .csproj 文件");

        var rootNamespace = Path.GetFileNameWithoutExtension(projectFile);
        var relativePath = Path.GetRelativePath(projectDirectory, outputDirectory);

        if (relativePath == ".")
            return SanitizeNamespaceSegment(rootNamespace);

        var pathSegments = relativePath
            .Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries)
            .Select(SanitizeNamespaceSegment);

        return string.Join(".", new[] { rootNamespace }.Concat(pathSegments));
    }

    /// <summary>
    ///     清理命名空间段
    /// </summary>
    private static string SanitizeNamespaceSegment(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("命名空间段为空");

        var cleaned = new StringBuilder(input.Length);

        foreach (var c in input)
            if (char.IsLetterOrDigit(c))
                cleaned.Append(c);
            else if (c == '-') cleaned.Append('_');

        if (cleaned.Length == 0)
            throw new ArgumentException("无效命名空间段：", input);

        if (char.IsDigit(cleaned[0])) cleaned.Insert(0, '_');

        return cleaned.ToString();
    }
}