using Microsoft.Extensions.Logging;
using Moq;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Test;

public class DomainEventGeneratorTests
{
    private readonly Mock<ICodeGenerationHelper> _mockCodeGenerationHelper = new();
    private readonly Mock<ILogger<DomainEventGenerator>> _mockLogger = new();

    [Fact]
    public void Generate_CreatesDomainEventFile_WithExpectedContent()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string className = "OrderCreated";
        var parameters = new GenerationCommonParameters(className, tempDir);

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("TestNamespace");

        var generator = new DomainEventGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters);

            // Assert
            var fileName = Path.Combine(tempDir, className + "DomainEvent.cs");
            Assert.True(File.Exists(fileName), "生成的 DomainEvent 文件不存在");

            var content = File.ReadAllText(fileName);
            Assert.Contains("namespace TestNamespace;", content);
            Assert.Contains($"public record {className}DomainEvent() : IDomainEvent;", content);
            Assert.Contains("using NetCorePal.Extensions.Domain;", content);
            Assert.Contains("using NetCorePal.Extensions.Primitives;", content);
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
        const string className = "Existing";
        var parameters = new GenerationCommonParameters(className, tempDir);
        var filePath = Path.Combine(tempDir, className + "DomainEvent.cs");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, "// existing content");

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("SomeNamespace");

        var generator = new DomainEventGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters);

            // Assert: 文件未被覆盖
            var content = File.ReadAllText(filePath);
            Assert.Equal("// existing content", content);

            // Assert: 错误日志已记录
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