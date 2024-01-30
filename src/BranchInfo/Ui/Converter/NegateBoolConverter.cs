using System.Globalization;
using System.Windows.Data;

namespace BranchInfo.Ui.Converter;

internal class NegateBoolConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ConvertBool(value);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return ConvertBool(value);
    }

    /// <summary>
    /// Converts the bool value
    /// </summary>
    /// <param name="value">The value</param>
    /// <returns>The converted value</returns>
    private static bool ConvertBool(object? value)
    {
        return value is false;
    }
}