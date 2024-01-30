using BranchInfo.Business;
using BranchInfo.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Media;

namespace BranchInfo.Ui.ViewModel;

/// <summary>
/// Provides the logic for the <see cref="View.CustomColorWindow"/>
/// </summary>
internal partial class CustomColorWindowViewModel : ViewModelBase
{
    /// <summary>
    /// The action to close the window
    /// </summary>
    private Action<bool>? _closeWindow;

    /// <summary>
    /// Contains the list with the custom color schemes
    /// </summary>
    private List<CustomColorScheme> _customColorSchemes = [];

    /// <summary>
    /// Gets or sets the name of the color
    /// </summary>
    [ObservableProperty]
    private string _name = string.Empty;

    /// <summary>
    /// Gets or sets the selected color
    /// </summary>
    [ObservableProperty]
    private Color? _selectedColor;

    /// <summary>
    /// Init the view model
    /// </summary>
    /// <param name="closeWindow">The action to close the window</param>
    public void InitViewModel(Action<bool> closeWindow)
    {
        _customColorSchemes = ThemeHelper.LoadCustomColors();
        _closeWindow = closeWindow;
    }

    /// <summary>
    /// Saves the desired color
    /// </summary>
    [RelayCommand]
    private void SaveColor()
    {
        // Check if the selected color already exists
        if (string.IsNullOrWhiteSpace(Name) || SelectedColor == null)
            return;

        if (_customColorSchemes.Any(a => a.Name.Equals(Name, StringComparison.OrdinalIgnoreCase)))
        {
            ShowInfoMessage($"The color with the name '{Name}' already exists.");
            return;
        }

        _customColorSchemes.Add(new CustomColorScheme
        {
            Name = Name,
            Color = SelectedColor.ToHex()
        });

        var result = ThemeHelper.SaveCustomColors(_customColorSchemes);

        if (result)
            _closeWindow?.Invoke(true);
        else
            ShowInfoMessage("An error has occurred.");
    }

    /// <summary>
    /// Closes the window
    /// </summary>
    [RelayCommand]
    private void CloseWindow()
    {
        _closeWindow?.Invoke(false);
    }
}