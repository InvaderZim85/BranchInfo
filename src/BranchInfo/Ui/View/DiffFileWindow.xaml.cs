using BranchInfo.Model;
using MahApps.Metro.Controls;
using System.Windows;
using BranchInfo.Ui.ViewModel;

namespace BranchInfo.Ui.View;

/// <summary>
/// Interaction logic for DiffFileWindow.xaml
/// </summary>
public partial class DiffFileWindow : MetroWindow
{
    /// <summary>
    /// Contains the list with the diff files
    /// </summary>
    private readonly List<DiffFileEntry> _diffFiles;

    /// <summary>
    /// Creates a new instance of the <see cref="DiffFileWindow"/>
    /// </summary>
    /// <param name="diffFiles">The list with the diff files</param>
    public DiffFileWindow(List<DiffFileEntry> diffFiles)
    {
        InitializeComponent();

        _diffFiles = diffFiles;
    }

    /// <summary>
    /// Occurs when the user hits the <see cref="ButtonClose"/>
    /// </summary>
    /// <param name="sender">The <see cref="ButtonClose"/></param>
    /// <param name="e">The event arguments</param>
    private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    /// <summary>
    /// Occurs when the window was loaded
    /// </summary>
    /// <param name="sender">The <see cref="DiffFileWindow"/></param>
    /// <param name="e">The event arguments</param>
    private void DiffFileWindow_OnLoaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is DiffFileWindowViewModel viewModel)
            viewModel.InitViewModel(_diffFiles);
    }
}