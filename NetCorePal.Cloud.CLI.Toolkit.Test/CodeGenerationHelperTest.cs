using Moq;
using NetCorePal.Cloud.CLI.Toolkit.Utils;
using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Test;

public class CodeGenerationHelperTests
{
    private readonly Mock<IProjectFinder> _mockProjectFinder;
    private readonly CodeGenerationHelper _helper;

    public CodeGenerationHelperTests()
    {
        _mockProjectFinder = new Mock<IProjectFinder>();
        _helper = new CodeGenerationHelper(_mockProjectFinder.Object);
    }
    
    [Fact]
    public void DetermineNamespace_WithValidProject_ReturnsCorrectNamespace()
    {
        // Arrange
        _mockProjectFinder.Setup(p => p.FindProjectDirectory("test/project/subdir"))
            .Returns("test/project");
        _mockProjectFinder.Setup(p => p.GetProjectFileName("test/project"))
            .Returns("test/project/MyProject.csproj");
        // _mockFileSystem.Setup(fs => fs.GetFiles("test/project", "*.csproj"))
        //     .Returns(["test/project/MyProject.csproj"]);

        // Act
        var ns = _helper.DetermineNamespace("test/project/subdir");

        // Assert
        Assert.Equal("MyProject.subdir", ns);
    }

    [Fact]
    public void DetermineNamespace_WithInvalidPathCharacters_ReplacesWithUnderscores()
    {
        // Arrange
        _mockProjectFinder.Setup(p => p.FindProjectDirectory("test/project/bad-dir/123-subdir"))
            .Returns("test/project");
        _mockProjectFinder.Setup(p => p.GetProjectFileName("test/project"))
            .Returns("test/project/MyProject.csproj");
        // _mockFileSystem.Setup(fs => fs.GetFiles("test/project", "*.csproj"))
        //     .Returns(["test/project/MyProject.csproj"]);

        // Act
        var ns = _helper.DetermineNamespace("test/project/bad-dir/123-subdir");

        // Assert
        Assert.Equal("MyProject.bad_dir._123_subdir", ns);
    }

    [Fact]
    public void DetermineNamespace_WhenNoProjectFound_ThrowsFileNotFoundException()
    {
        // Arrange
        _mockProjectFinder.Setup(p => p.FindProjectDirectory(It.IsAny<string>()))
            .Returns((string?)null);

        // Act & Assert
        var ex = Assert.Throws<FileNotFoundException>(() =>
            _helper.DetermineNamespace("invalid/path"));
        Assert.Equal("未找到 .csproj 文件", ex.Message);
    }
}