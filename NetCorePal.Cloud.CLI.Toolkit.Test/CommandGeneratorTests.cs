using Microsoft.Extensions.Logging;
using Moq;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Test;

public class CommandGeneratorTests
{
    private readonly Mock<ICodeGenerationHelper> _mockCodeGenerationHelper = new();
    private readonly Mock<ILogger<CommandGenerator>> _mockLogger = new();

    [Theory]
    [InlineData(null)]
    [InlineData("string")]
    public void Generate_CreatesCommandFile_WithExpectedContent(string? returnType)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string className = "Test";
        var parameters = new GenerationCommonParameters(className, tempDir);

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("TestNamespace");

        var generator = new CommandGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters, returnType);

            // Assert
            var fileName = Path.Combine(tempDir, className + "Command.cs");
            Assert.True(File.Exists(fileName), "生成的文件不存在");

            var fileContent = File.ReadAllText(fileName);
            Assert.Contains("namespace TestNamespace;", fileContent);
            Assert.Contains($"public record {className}Command()", fileContent);
            Assert.Contains($"public class {className}CommandValidator : AbstractValidator<{className}Command>",
                fileContent);
            Assert.Contains("CommandHandler", fileContent);

            if (returnType is not null)
            {
                Assert.Contains($"ICommand<{returnType}>", fileContent);
                Assert.Contains($"Task<{returnType}> Handle", fileContent);
                Assert.Contains($"ICommandHandler<{className}Command,<{returnType}>>",
                    fileContent.Replace(" ", "").Replace("\r", "").Replace("\n", ""));
            }
            else
            {
                Assert.Contains("ICommand", fileContent);
                Assert.Contains("Task Handle", fileContent);
            }
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
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var parameters = new GenerationCommonParameters("Exists", tempDir);
        var filePath = Path.Combine(tempDir, "ExistsCommand.cs");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, "// existing");

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("FakeNS");

        var generator = new CommandGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters);

            // Assert
            var content = File.ReadAllText(filePath);
            Assert.Equal("// existing", content);

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