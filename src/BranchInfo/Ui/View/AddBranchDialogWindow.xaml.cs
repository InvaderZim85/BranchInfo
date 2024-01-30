using System.Windows;
using BranchInfo.Model;
using BranchInfo.Ui.ViewModel;
using MahApps.Metro.Controls;

namespace BranchInfo.Ui.View;

/// <summary>
/// Interaction logic for AddBranchDialogWindow.xaml
/// </summary>
public partial class AddBranchDialogWindow : MetroWindow
{
    /// <summary>
    /// Contains the list with the git directories
    /// </summary>
    private readonly List<string> _gitDirectories;

    /// <summary>
    /// The branch which should be edited
    /// </summary>
    private readonly BranchEntry? _branch;

    /// <summary>
    /// Contains the new created branch entry
    /// </summary>
    public BranchEntry? Branch { get; private set; }

    /// <summary>
    /// Creates a new instance of the <see cref="AddBranchDialogWindow"/>
    /// </summary>
    /// <param name="gitDirectories">The list with the current git directories</param>
    /// <param name="branch">The branch which should be edited</param>
    public AddBranchDialogWindow(List<string> gitDirectories, BranchEntry? branch = null)
    {
        InitializeComponent();

        _gitDirectories = gitDirectories;
        _branch = branch;
    }

    /// <summary>
    /// Closes the window and sets the new generated branch
    /// </summary>
    /// <param name="branch">The new branch</param>
    private void CloseWindow(BranchEntry? branch)
    {
        if (branch == null)
            DialogResult = false;
        else
        {
            Branch = branch;
            DialogResult = true;
        }
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="AddBranchDialogWindow"/></param>
    /// <param name="e">The event arguments</param>
    /// <exception cref="NotImplementedException"></exception>
    private void AddBranchDialogWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is AddBranchDialogWindowViewModel viewModel)
            viewModel.InitViewModel(CloseWindow, _branch, _gitDirectories);
    }
}