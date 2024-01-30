using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BranchInfo.Model;

/// <summary>
/// Represents a branch entry
/// </summary>
public sealed class BranchEntry : ObservableObject
{
    /// <summary>
    /// Gets or sets the id of the entry
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the repository
    /// </summary>
    public string Repo { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the path of the git directory
    /// </summary>
    public string GitDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets the path of the repository
    /// </summary>
    public string RepoDirectory => string.IsNullOrEmpty(GitDirectory) || !Directory.Exists(GitDirectory)
        ? "/"
        : new DirectoryInfo(GitDirectory).Parent?.FullName ?? "";

    /// <summary>
    /// Backing field for <see cref="FriendlyName"/>
    /// </summary>
    private string _friendlyName = string.Empty;

    /// <summary>
    /// Gets or sets the friendly name of the branch
    /// </summary>
    public string FriendlyName
    {
        get => _friendlyName;
        set => SetProperty(ref _friendlyName, value);
    }

    /// <summary>
    /// Backing field for <see cref="Status"/>
    /// </summary>
    private string _status = string.Empty;

    /// <summary>
    /// Gets or sets the status of the repository
    /// </summary>
    public string Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    /// <summary>
    /// Backing field for <see cref="LastCommit"/>
    /// </summary>
    private string _lastCommit = string.Empty;

    /// <summary>
    /// Gets or sets the last commit information
    /// </summary>
    public string LastCommit
    {
        get => _lastCommit;
        set => SetProperty(ref _lastCommit, value);
    }

    /// <summary>
    /// Gets or sets the date / time of the last check
    /// </summary>
    public DateTime LastCheck { get; set; }
}