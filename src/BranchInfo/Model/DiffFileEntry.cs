using BranchInfo.Common;
using System.IO;

namespace BranchInfo.Model;

/// <summary>
/// Provides the information about the "diff" file
/// </summary>
/// <param name="path">The path of the file</param>
/// <param name="type">The type</param>
public sealed class DiffFileEntry(string path, DiffFileType type)
{
    /// <summary>
    /// Gets the name of the file
    /// </summary>
    public string Name { get; } = Path.GetFileName(path);

    /// <summary>
    /// Gets the path of the file
    /// </summary>
    public string FilePath { get; } = path;

    /// <summary>
    /// Gets the type of the file
    /// </summary>
    public string Type => type.ToString();
}