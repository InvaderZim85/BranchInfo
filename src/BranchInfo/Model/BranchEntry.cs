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
    /// Gets or sets the status of the repository
    /// </summary>
    public string Status => DiffFiles.Count > 0 ? "Has changes" : "No changes since last commit";

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
    /// Backing field for <see cref="DiffFiles"/>
    /// </summary>
    private List<DiffFileEntry> _diffFiles = [];

    /// <summary>
    /// Gets or sets the list with the diff files
    /// </summary>
    public List<DiffFileEntry> DiffFiles
    {
        get => _diffFiles;
        set
        {
            if (SetProperty(ref _diffFiles, value))
                OnPropertyChanged(nameof(Status));
        }
    }

    /// <summary>
    /// Gets or sets the date / time of the last check
    /// </summary>
    public DateTime LastCheck { get; set; }
}