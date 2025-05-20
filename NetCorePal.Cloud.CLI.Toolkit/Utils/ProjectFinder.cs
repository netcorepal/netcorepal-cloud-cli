using NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

namespace NetCorePal.Cloud.CLI.Toolkit.Utils;

public class ProjectFinder : IProjectFinder
{
    public string? FindProjectDirectory(string startPath)
    {
        var directory = new DirectoryInfo(startPath);
        while (directory != null)
        {
            if (Directory.GetFiles(directory.FullName, "*.csproj").Length != 0)
                return directory.FullName;
            directory = directory.Parent;
        }

        return null;
    }

    public string? GetProjectFileName(string projectDirectory)
    {
        return Directory.GetFiles(projectDirectory, "*.csproj").FirstOrDefault();
    }
}