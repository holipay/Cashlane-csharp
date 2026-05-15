using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Cashlane.Converters;

public class StatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value as string ?? "";
        return status switch
        {
            "完成" or "付款" => new SolidColorBrush(Color.FromRgb(0x0f, 0x7b, 0x0f)),
            "签录" or "发票" => new SolidColorBrush(Color.FromRgb(0x00, 0x78, 0xd4)),
            "填单" => new SolidColorBrush(Color.FromRgb(0x9d, 0x5d, 0x00)),
            "取消" => new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99)),
            _ => new SolidColorBrush(Color.FromRgb(0x99, 0x99, 0x99))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class StatusToBackgroundConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = value as string ?? "";
        return status switch
        {
            "完成" or "付款" => new SolidColorBrush(Color.FromRgb(0xdf, 0xf6, 0xdd)),
            "签录" or "发票" => new SolidColorBrush(Color.FromRgb(0xe8, 0xf4, 0xfd)),
            "填单" => new SolidColorBrush(Color.FromRgb(0xff, 0xf4, 0xce)),
            "取消" => new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0)),
            _ => new SolidColorBrush(Color.FromRgb(0xf0, 0xf0, 0xf0))
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class BoolToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return b ? Visibility.Visible : Visibility.Collapsed;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility v)
            return v == Visibility.Visible;
        return false;
    }
}

public class InverseBoolConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return true;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b)
            return !b;
        return true;
    }
}

public class MoneyFormatConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double d && d > 0)
            return $"¥{d:N2}";
        return "—";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

public class NullableIntToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i && i > 0)
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}

/// <summary>
/// Converts between string (yyyy-MM-dd) and DateTime? for DatePicker bindings.
/// </summary>
public class StringToDateTimeConverter : IValueConverter
{
    public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string s && !string.IsNullOrEmpty(s))
        {
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
        }
        return null;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTime dt)
            return dt.ToString("yyyy-MM-dd");
        return null;
    }
}

/// <summary>
/// Converts an IsEditing bool to a title string.
/// </summary>
public class EditModeToTitleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool b && b)
            return "✏️ 编辑费用记录";
        return "➕ 新增费用记录";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
