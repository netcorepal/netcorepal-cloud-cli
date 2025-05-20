using Microsoft.Extensions.Logging;
using Moq;
using NetCorePal.Cloud.CLI.Toolkit.CommonParameters;
using NetCorePal.Cloud.CLI.Toolkit.Const;
using NetCorePal.Cloud.CLI.Toolkit.Services.Generation.Generators;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Test;

public class AggregateRootGeneratorTests
{
    private readonly Mock<ICodeGenerationHelper> _mockCodeGenerationHelper = new();
    private readonly Mock<ILogger<AggregateRootGenerator>> _mockLogger = new();

    [Theory]
    [InlineData(StrongIdType.Int64)]
    [InlineData(StrongIdType.Guid)]
    [InlineData(StrongIdType.String)]
    public void Generate_CreatesAggregateRootFile_WithExpectedContent(StrongIdType idType)
    {
        // Arrange
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        const string className = "Customer";
        var parameters = new GenerationCommonParameters(className, tempDir);
        var expectedIdName = $"{className}Id";

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("TestDomain");

        var generator = new AggregateRootGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

        try
        {
            // Act
            generator.Generate(parameters, idType);

            // Assert
            var fileName = Path.Combine(tempDir, className + ".cs");
            Assert.True(File.Exists(fileName), "生成的 AggregateRoot 文件不存在");

            var content = File.ReadAllText(fileName);
            Assert.Contains("namespace TestDomain;", content);
            Assert.Contains($"public partial record {expectedIdName} : I{idType}StronglyTypedId;", content);
            Assert.Contains($"public class {className} : Entity<{expectedIdName}>, IAggregateRoot", content);
            Assert.Contains("protected Customer()", content);
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
        const string className = "Product";
        var parameters = new GenerationCommonParameters(className, tempDir);
        var filePath = Path.Combine(tempDir, className + ".cs");

        Directory.CreateDirectory(tempDir);
        File.WriteAllText(filePath, "// pre-existing content");

        _mockCodeGenerationHelper.Setup(h => h.DetermineNamespace(It.IsAny<string>()))
            .Returns("SomeNS");

        var generator = new AggregateRootGenerator(_mockCodeGenerationHelper.Object, _mockLogger.Object);

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