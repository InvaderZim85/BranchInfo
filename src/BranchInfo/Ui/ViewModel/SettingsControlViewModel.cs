using System.Collections.ObjectModel;
using BranchInfo.Business;
using BranchInfo.Model;
using BranchInfo.Ui.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ControlzEx.Theming;
using System.Windows;

namespace BranchInfo.Ui.ViewModel;

/// <summary>
/// Provides the logic for the <see cref="View.SettingsControl"/>
/// </summary>
internal partial class SettingsControlViewModel : ViewModelBase
{
    /// <summary>
    /// The list with the color themes
    /// </summary>
    [ObservableProperty]
    private ObservableCollection<ColorEntry> _colorThemeList = [];

    /// <summary>
    /// Backing field for <see cref="SelectedColorTheme"/>
    /// </summary>
    private ColorEntry? _selectedColorTheme;

    /// <summary>
    /// Gets or sets the selected color theme
    /// </summary>
    public ColorEntry? SelectedColorTheme
    {
        get => _selectedColorTheme;
        set
        {
            if (SetProperty(ref _selectedColorTheme, value) && value != null)
                ThemeHelper.SetColorTheme(value.Name);
        }
    }

    /// <summary>
    /// Creates a new instance of the <see cref="SettingsControlViewModel"/>
    /// </summary>
    public SettingsControlViewModel()
    {
        var themeName = Properties.Settings.Default.ColorScheme;
        if (string.IsNullOrWhiteSpace(themeName))
            themeName = ThemeHelper.DefaultTheme;

        AddColors(themeName);
    }

    /// <summary>
    /// Adds the colors
    /// </summary>
    /// <param name="preSelection">The name of the color which should be selected</param>
    private void AddColors(string preSelection)
    {
        var tmpList = new List<ColorEntry>();
        tmpList.AddRange(ThemeManager.Current.ColorSchemes.Select(s => new ColorEntry(s, false)));

        // Add the custom colors
        tmpList.AddRange(ThemeHelper.LoadCustomColors().Select(s => s.Name).Select(s => new ColorEntry(s, true)));

        ColorThemeList = new ObservableCollection<ColorEntry>(tmpList);

        SelectedColorTheme = ColorThemeList.FirstOrDefault(f => f.Name.Equals(preSelection, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Adds the window to add a new color
    /// </summary>
    [RelayCommand]
    private void AddCustomColor()
    {
        var dialog = new CustomColorWindow
        {
            Owner = Application.Current.MainWindow
        };

        if (dialog.ShowDialog() != true)
            return;

        AddColors(dialog.ColorName);
    }

    /// <summary>
    /// Deletes the selected custom color
    /// </summary>
    [RelayCommand]
    private async Task DeleteCustomColorAsync()
    {
        if (SelectedColorTheme == null)
            return;

        if (!SelectedColorTheme.CustomColor)
        {
            await ShowMessageAsync("Color", "You can't delete a default color. Please select a custom color.");
            return;
        }

        // Save the custom colors
        var result = ThemeHelper.RemoveCustomColor(SelectedColorTheme.Name);

        if (result)
        {
            AddColors(ColorThemeList.FirstOrDefault()?.Name ?? string.Empty);
        }
        else
        {
            await ShowMessageAsync("Color",
                $"An error has occurred while deleting the color '{SelectedColorTheme.Name}'");
        }
    }

    /// <summary>
    /// Saves the current theme
    /// </summary>
    /// <returns>The awaitable task</returns>
    [RelayCommand]
    private async Task SaveThemeAsync()
    {
        if (SelectedColorTheme == null)
            return;

        var controller = await ShowProgressAsync("Save", "Please wait while saving the theme...");

        try
        {
            Properties.Settings.Default.ColorScheme = SelectedColorTheme.Name;
            Properties.Settings.Default.Save();
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
}