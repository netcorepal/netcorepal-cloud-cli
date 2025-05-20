using Microsoft.Extensions.Logging;
using Moq;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Test;

public class DomainEventHandlerGeneratorTests
{
    private readonly Mock<ICodeGenerationHelper> _mockCodeGenerationHelper = new();
    private readonly Mock<ILogger<DomainEventHandlerGenerator>> _mockLogger = new();

    [Fact]
    public void Generate_CreatesDomainEventHandlerFile_WithExpectedContent()
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string className = "OrderCreatedHandler";
        const string domainEventType = "OrderCreatedDomainEvent";
        var parameters = new GenerationCommonParameters(className, tempDir);

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("TestNamespace");

        var generator = new DomainEventHandlerGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters, domainEventType);

            // Assert
            var filePath = Path.Combine(tempDir, className + "DomainEventHandler.cs");
            Assert.True(File.Exists(filePath), "生成的文件不存在");

            var content = File.ReadAllText(filePath);
            Assert.Contains("using MediatR;", content);
            Assert.Contains("using NetCorePal.Extensions.Domain;", content);
            Assert.Contains($"public class {className}(IMediator mediator)", content);
            Assert.Contains($"IDomainEventHandler<{domainEventType}>", content);
            Assert.Contains($"Task Handle({domainEventType} notification", content);
            Assert.Contains("throw new NotImplementedException();", content);
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
        const string className = "ExistingHandler";
        var parameters = new GenerationCommonParameters(className, tempDir);
        var filePath = Path.Combine(tempDir, className + "DomainEventHandler.cs");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, "// pre-existing content");

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("FakeNamespace");

        var generator = new DomainEventHandlerGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters, "DummyEvent");

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