namespace NetCorePal.Cloud.CLI.Toolkit.Utils.Interface;

public interface IProjectFinder
{
    /// <summary>
    ///     查找项目所在目录
    /// </summary>
    /// <param name="startPath"></param>
    /// <returns></returns>
    string? FindProjectDirectory(string startPath);

    /// <summary>
    ///     获取项目名称
    /// </summary>
    /// <param name="projectDirectory">项目目录</param>
    /// <returns></returns>
    string? GetProjectFileName(string projectDirectory);
}