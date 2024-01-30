using BranchInfo.Model;
using ControlzEx.Theming;
using Newtonsoft.Json;
using Serilog;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace BranchInfo.Business;

/// <summary>
/// Provides several functions for the interaction with the MahApps themes
/// </summary>
public static class ThemeHelper
{
    /// <summary>
    /// Contains the default theme color
    /// </summary>
    public const string DefaultTheme = "Emerald";

    /// <summary>
    /// Gets the filepath of the custom color file
    /// </summary>
    private static string CustomColorFile => Path.Combine(AppContext.BaseDirectory, "CustomColors.json");

    /// <summary>
    /// Contains the list with the custom color schemes
    /// </summary>
    private static List<CustomColorScheme> _customColorSchemes = [];

    /// <summary>
    /// Sets the color scheme
    /// </summary>
    /// <param name="colorScheme">The scheme which should be set</param>
    public static void SetColorTheme(string colorScheme = "")
    {
        if (string.IsNullOrEmpty(colorScheme))
            colorScheme = Properties.Settings.Default.ColorScheme;

        var customColors = LoadCustomColors();
        if (customColors.Select(s => s.Name).Contains(colorScheme, StringComparer.OrdinalIgnoreCase))
        {
            var customColor = customColors.FirstOrDefault(f => f.Name.Equals(colorScheme, StringComparison.OrdinalIgnoreCase));
            if (customColor == null)
                return;

            var newTheme = new Theme("AppTheme", "AppTheme", "Dark", customColor.ColorValue.ToHex(), customColor.ColorValue,
                new SolidColorBrush(customColor.ColorValue), true, false);
            ThemeManager.Current.ChangeTheme(Application.Current, newTheme);
        }
        else
        {
            var schemeName =
                ThemeManager.Current.ColorSchemes.FirstOrDefault(f =>
                    f.Equals(colorScheme, StringComparison.OrdinalIgnoreCase)) ?? DefaultTheme;
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, schemeName);
        }
    }

    /// <summary>
    /// Loads the custom colors
    /// </summary>
    /// <returns>The list with the custom color</returns>
    public static List<CustomColorScheme> LoadCustomColors()
    {
        if (_customColorSchemes.Count > 0)
            return _customColorSchemes;

        if (!File.Exists(CustomColorFile))
            return []; // Return an empty list

        try
        {
            var content = File.ReadAllText(CustomColorFile);

            _customColorSchemes = JsonConvert.DeserializeObject<List<CustomColorScheme>>(content) ?? [];

            return _customColorSchemes;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Can't load custom colors. File: '{path}'", CustomColorFile);
            return [];
        }
    }

    /// <summary>
    /// Loads the default MahApps colors
    /// </summary>
    /// <returns>The list with the default colors</returns>
    public static List<Color?> LoadDefaultColors()
    {
        var defaultColors = new List<string>
        {
            "#FF0050EF",
            "#FF0078D7",
            "#FF008A00",
            "#FF00ABA9",
            "#FF1BA1E2",
            "#FF60A917",
            "#FF6459DF",
            "#FF647687",
            "#FF6A00FF",
            "#FF6D8764",
            "#FF76608A",
            "#FF825A2C",
            "#FF87794E",
            "#FFA0522D",
            "#FFA20025",
            "#FFA4C400",
            "#FFAA00FF",
            "#FFD80073",
            "#FFE51400",
            "#FFF0A30A",
            "#FFF472D0",
            "#FFFA6800",
            "#FFFEDE06"
        };

        var resultList = new List<Color?>();

        foreach (var entry in defaultColors)
        {
            if (ColorConverter.ConvertFromString(entry) is Color color)
                resultList.Add(color);
        }

        return resultList;
    }

    /// <summary>
    /// Saves the list with the custom colors
    /// </summary>
    /// <param name="customColors">The list with the custom colors</param>
    /// <returns><see langword="true"/> when the colors were saves successfully, otherwise <see langword="false"/></returns>
    public static bool SaveCustomColors(List<CustomColorScheme> customColors)
    {
        if (customColors.Count == 0)
            return true;

        try
        {
            var content = JsonConvert.SerializeObject(customColors, Formatting.Indented);

            File.WriteAllText(CustomColorFile, content);

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Can't save custom colors. File: '{path}'", CustomColorFile);
            return false;
        }
    }

    /// <summary>
    /// Removes a custom color from the list
    /// </summary>
    /// <param name="name">The name of the color</param>
    /// <returns><see langword="true"/> when the color was deleted successfully, otherwise <see langword="false"/></returns>
    public static bool RemoveCustomColor(string name)
    {
        var customColors = LoadCustomColors();

        var customColor = customColors.FirstOrDefault(f => f.Name.Equals(name));
        if (customColor == null)
            return true;

        customColors.Remove(customColor);

        return SaveCustomColors(customColors);
    }

    /// <summary>
    /// Converts the color to a HEX value (for example <i>Green</i> > <c>#FF00FF00</c>)
    /// </summary>
    /// <param name="color">The color</param>
    /// <returns>The HEX value of the color</returns>
    public static string ToHex(this Color? color)
    {
        return color == null
            ? string.Empty
            : color.Value.ToHex();
    }

    /// <summary>
    /// Converts the color to a HEX value (for example <i>Green</i> > <c>#FF00FF00</c>)
    /// </summary>
    /// <param name="color">The color</param>
    /// <returns>The HEX value of the color</returns>
    public static string ToHex(this Color color)
    {
        return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";
    }
}