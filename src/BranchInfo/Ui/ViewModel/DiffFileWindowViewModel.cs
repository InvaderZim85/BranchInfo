using BranchInfo.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace BranchInfo.Ui.ViewModel;

/// <summary>
/// Provides the logic for <see cref="View.DiffFileWindow"/>
/// </summary>
internal partial class DiffFileWindowViewModel : ViewModelBase
{
    /// <summary>
    /// Gets or sets the list with the diff files
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<DiffFileEntry> _diffFiles = [];

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="diffFiles"></param>
    public void InitViewModel(List<DiffFileEntry> diffFiles)
    {
        DiffFiles = new ObservableCollection<DiffFileEntry>(diffFiles);
    }
}