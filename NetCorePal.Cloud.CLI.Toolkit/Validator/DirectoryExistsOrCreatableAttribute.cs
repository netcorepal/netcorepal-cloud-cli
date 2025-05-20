using System.ComponentModel.DataAnnotations;

namespace NetCorePal.Cloud.CLI.Toolkit.Validator;

public class DirectoryExistsAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null) return ValidationResult.Success;

        var path = Path.GetFullPath((string)value);
        try
        {
            if (!IsValidPathFormat(path)) return new ValidationResult($"路径格式无效: {path}");

            return Directory.Exists(path)
                ? ValidationResult.Success
                : new ValidationResult($"目录 '{path}' 不存在或不可访问");
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException)
        {
            return new ValidationResult($"没有权限访问目录: {path}");
        }
        catch (Exception ex) when (ex is IOException or ArgumentException)
        {
            return new ValidationResult($"路径解析失败: {path}");
        }
    }

    private static bool IsValidPathFormat(string path)
    {
        return !string.IsNullOrWhiteSpace(path) &&
               path.IndexOfAny(Path.GetInvalidPathChars()) == -1 &&
               !Path.HasExtension(path);
    }
}