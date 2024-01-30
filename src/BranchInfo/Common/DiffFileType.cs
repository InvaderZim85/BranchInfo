namespace BranchInfo.Common;

/// <summary>
/// The different file types
/// </summary>
public enum DiffFileType
{
    /// <summary>
    /// File was added
    /// </summary>
    Added,

    /// <summary>
    /// File was deleted
    /// </summary>
    Delete,

    /// <summary>
    /// File was modified
    /// </summary>
    Modified,

    /// <summary>
    /// The file contains conflicts
    /// </summary>
    Conflicted,

    /// <summary>
    /// The file was copied
    /// </summary>
    Copied,

    /// <summary>
    /// The file was renamed
    /// </summary>
    Renamed,

    /// <summary>
    /// The type of the file was changed
    /// </summary>
    TypeChanged,

    /// <summary>
    /// File was not modified
    /// </summary>
    Unmodified
}