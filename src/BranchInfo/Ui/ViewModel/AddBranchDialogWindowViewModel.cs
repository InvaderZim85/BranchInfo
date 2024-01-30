using BranchInfo.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace BranchInfo.Ui.ViewModel;

/// <summary>
/// Provides the functions for the interaction with the <see cref="View.AddBranchDialogWindow"/>
/// </summary>
internal partial class AddBranchDialogWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Contains the branch which should be updated
    /// </summary>
    private BranchEntry _branchEntry = new();

    /// <summary>
    /// The action to close the window
    /// </summary>
    private Action<BranchEntry?>? _closeWindow;

    /// <summary>
    /// Contains the list with the current git directories
    /// </summary>
    private List<string> _gitDirectories = [];

    /// <summary>
    /// Gets or sets the name of the repo
    /// </summary>
    [ObservableProperty]
    private string _repoName = string.Empty;

    /// <summary>
    /// Gets or sets the path of the Git directory
    /// </summary>
    [ObservableProperty]
    private string _gitDir = string.Empty;

    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = string.Empty;

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="closeWindow">The action to close the window</param>
    /// <param name="branch">The branch which should be edited</param>
    /// <param name="gitDirectories">The list with the already existing git directories</param>
    public void InitViewModel(Action<BranchEntry?> closeWindow, BranchEntry? branch, List<string> gitDirectories)
    {
        WindowTitle = branch == null ? "Add new entry" : "Edit entry";
        _branchEntry = branch ?? new BranchEntry();
        _closeWindow = closeWindow;
        _gitDirectories = gitDirectories;

        // Set the values
        RepoName = _branchEntry.Repo;
        GitDir = _branchEntry.GitDirectory;
    }

    /// <summary>
    /// Opens a folder dialog to select a Git directory
    /// </summary>
    [RelayCommand]
    private void SelectDirectory()
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Select the .git directory to add a repo",
            ShowHiddenItems = true
        };

        if (dialog.ShowDialog() != true)
            return;

        if (!dialog.FolderName.EndsWith(".git"))
        {
            ShowInfoMessage("You have to choose a '.git' folder!");
            return;
        }

        GitDir = dialog.FolderName;
    }

    /// <summary>
    /// Accepts the values
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private void AcceptValues()
    {
        if (string.IsNullOrEmpty(RepoName) || string.IsNullOrEmpty(GitDir))
        {
            ShowInfoMessage("Name and / or GIT directory are missing.");
            return;
        }

        if (_gitDirectories.Contains(GitDir, StringComparer.OrdinalIgnoreCase))
        {
            ShowInfoMessage("An entry with the specified path is already added.");
            return;
        }

        _branchEntry.GitDirectory = GitDir;
        _branchEntry.Repo = RepoName;

        _closeWindow?.Invoke(_branchEntry);
    }

    /// <summary>
    /// Cancels the input and closes the window
    /// </summary>
    [RelayCommand]
    private void Cancel()
    {
        _closeWindow?.Invoke(null);
    }
}