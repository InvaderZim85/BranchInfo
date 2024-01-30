using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using BranchInfo.Business;
using BranchInfo.Model;
using BranchInfo.Ui.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Timer = System.Timers.Timer;

namespace BranchInfo.Ui.ViewModel;

/// <summary>
/// Provides the functions for the <see cref="MainWindow"/>
/// </summary>
internal partial class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The instance for the interaction with the branch information
    /// </summary>
    private readonly BranchManager _manager = new();

    /// <summary>
    /// Contains the check timer
    /// </summary>
    private Timer? _checkTimer;

    /// <summary>
    /// Contains the date of the last branch check
    /// </summary>
    private DateTime _lastCheck;

    /// <summary>
    /// Gets or sets the list with the branches
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<BranchEntry> _branchList = [];

    /// <summary>
    /// Gets or sets the selected branch
    /// </summary>
    [ObservableProperty]
    private BranchEntry? _selectedBranch;

    /// <summary>
    /// Gets or sets the value which indicates if the "remove" button is enabled
    /// </summary>
    [ObservableProperty]
    private bool _buttonRemoveEnabled;

    /// <summary>
    /// Gets or sets the value which indicates if the timer is enabled
    /// </summary>
    [ObservableProperty]
    private bool _timerEnabled;

    /// <summary>
    /// Gets or sets the check time
    /// </summary>
    [ObservableProperty]
    private int _checkTime = 5;

    /// <summary>
    /// Gets or sets the window title
    /// </summary>
    [ObservableProperty]
    private string _windowTitle = "Branch Info";

    /// <summary>
    /// Gets or sets the last check info
    /// </summary>
    [ObservableProperty]
    private string _lastCheckInfo = "Last check: /";

    /// <summary>
    /// Gets or sets the settings control window view model
    /// </summary>
    [ObservableProperty] 
    private SettingsControlViewModel _settingsControlViewModel = new();

    /// <summary>
    /// Gets or sets the value which indicates if the diff file window context menu is enabled
    /// </summary>
    [ObservableProperty]
    private bool _contextMenuDiffFilesEnabled;

    /// <summary>
    /// Occurs when the user selected another branch
    /// </summary>
    /// <param name="value">The selected branch</param>
    partial void OnSelectedBranchChanged(BranchEntry? value)
    {
        ButtonRemoveEnabled = value != null;
        ContextMenuDiffFilesEnabled = value is { DiffFiles.Count: > 0 };
    }

    /// <summary>
    /// Occurs when the user activates the timer
    /// </summary>
    /// <param name="value">The new value</param>
    partial void OnTimerEnabledChanged(bool value)
    {
        if (value)
        {
            _checkTimer = new Timer(TimeSpan.FromMinutes(CheckTime));

            _checkTimer.Elapsed += (_, _) =>
            {
                LoadBranchInfo();
            };

            LoadBranchInfo();

            _checkTimer.Start();
        }
        else
        {
            _checkTimer?.Stop();
            _checkTimer?.Dispose();

            LoadBranchInfo();
        }
    }

    /// <summary>
    /// Init the view model
    /// </summary>
    public async void InitViewModel()
    {
        var controller = await ShowProgressAsync("Please wait", "Please wait while adding the branch...");

        try
        {
            // Add the new branch
            await _manager.LoadBranchListAsync();

            _lastCheck = DateTime.Now;

            SetWindowTitle();

            SetBranchList();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Loads the branch information
    /// </summary>
    [RelayCommand]
    private void LoadBranchInfo()
    {
        if (BranchList.Count == 0)
            return;

        foreach (var branchEntry in BranchList)
        {
            BranchManager.LoadBranchInformation(branchEntry);
        }

        _lastCheck = DateTime.Now;

        SetWindowTitle();
    }

    /// <summary>
    /// Sets the branch list
    /// </summary>
    /// <param name="preSelection">The branch which should be selected (optional)</param>
    private void SetBranchList(BranchEntry? preSelection = null)
    {
        BranchList = new ObservableCollection<BranchEntry>(_manager.BranchList);

        if (preSelection == null)
            return;

        SelectedBranch = BranchList.FirstOrDefault(f => f.GitDirectory.Equals(preSelection.GitDirectory));
    }

    /// <summary>
    /// Sets the window title
    /// </summary>
    private void SetWindowTitle()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version ?? new Version();
        WindowTitle = TimerEnabled
            ? $"Branch Info - v{version} [auto refresh]"
            : $"Branch Info - v{version}";

        LastCheckInfo = $"Last check: {_lastCheck:yyyy-MM-dd HH:mm:ss}";
    }

    /// <summary>
    /// Adds a new branch
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task AddBranchAsync()
    {
        var dialog = new AddBranchDialogWindow(BranchList.Select(s => s.GitDirectory).ToList())
        {
            Owner = Application.Current.MainWindow
        };
        if (dialog.ShowDialog() != true || dialog.Branch == null)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while adding the branch...");

        try
        {
            // Add the new branch
            await _manager.AddBranchAsync(dialog.Branch);

            SetBranchList(dialog.Branch);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Edits the selected branch
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task EditBranchAsync()
    {
        if (SelectedBranch == null)
            return;

        var dialog =
            new AddBranchDialogWindow(
                BranchList.Where(w => w.Id != SelectedBranch.Id).Select(s => s.GitDirectory).ToList(), SelectedBranch)
            {
                Owner = Application.Current.MainWindow
            };

        if (dialog.ShowDialog() != true || dialog.Branch == null)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while updating the branch...");

        try
        {
            // Add the new branch
            await _manager.SaveBranchListAsync();

            SetBranchList(dialog.Branch);
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Removes the selected branch
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task RemoveBranchAsync()
    {
        if (SelectedBranch == null)
            return;

        var controller = await ShowProgressAsync("Please wait", "Please wait while removing the branch...");

        try
        {
            await _manager.RemoveBranchAsync(SelectedBranch);

            SetBranchList();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync(ex, ErrorMessageType.Save);
        }
        finally
        {
            await controller.CloseAsync();
        }
    }

    /// <summary>
    /// Opens the current selected entry in the file explorer
    /// </summary>
    [RelayCommand]
    private void OpenFolderInFileExplorer()
    {
        if (SelectedBranch == null || !Directory.Exists(SelectedBranch.RepoDirectory))
            return;

        var arguments = $"/n, /e, \"{SelectedBranch.RepoDirectory}\"";
        Process.Start("explorer.exe", arguments);
    }

    /// <summary>
    /// Opens the diff file window
    /// </summary>
    [RelayCommand]
    private void ShowDiffFiles()
    {
        if (SelectedBranch == null || SelectedBranch.DiffFiles.Count == 0)
            return;

        var diffFileWindow = new DiffFileWindow(SelectedBranch.DiffFiles)
        {
            Owner = Application.Current.MainWindow
        };

        diffFileWindow.ShowDialog();
    }
}