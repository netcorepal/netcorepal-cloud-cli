using Microsoft.Extensions.Logging;
using Moq;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Test;

public class RepositoryGeneratorTests
{
    private readonly Mock<ICodeGenerationHelper> _mockCodeGenerationHelper = new();
    private readonly Mock<ILogger<RepositoryGenerator>> _mockLogger = new();

    [Theory]
    [InlineData("ApplicationDbContext")]
    [InlineData("CustomDbContext")]
    public void Generate_CreatesRepositoryFile_WithExpectedContent(string dbContextType)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string entityName = "Product";
        var parameters = new GenerationCommonParameters(entityName, tempDir);
        var idName = $"{entityName}Id";

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("TestNamespace");

        var generator = new RepositoryGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters, dbContextType);

            // Assert
            var filePath = Path.Combine(tempDir, entityName + "Repository.cs");
            Assert.True(File.Exists(filePath), "生成的文件不存在");

            var content = File.ReadAllText(filePath);
            Assert.Contains("using NetCorePal.Extensions.Repository;", content);
            Assert.Contains("using NetCorePal.Extensions.Repository.EntityFrameworkCore;", content);
            Assert.Contains($"public interface I{entityName}Repository : IRepository<{entityName}, {idName}>;",
                content);
            Assert.Contains($"public class {entityName}Repository({dbContextType} context)", content);
            Assert.Contains($"RepositoryBase<{entityName}, {idName}, {dbContextType}>", content);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }

    [Fact]
    public void Generate_DoesNotOverwriteExistingFile()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string entityName = "Order";
        var parameters = new GenerationCommonParameters(entityName, tempDir);
        var filePath = Path.Combine(tempDir, entityName + "Repository.cs");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, "// pre-existing content");

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("FakeNamespace");

        var generator = new RepositoryGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters);

            // Assert
            var content = File.ReadAllText(filePath);
            Assert.Equal("// pre-existing content", content);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("已存在")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
        finally
        {
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, true);
        }
    }
}