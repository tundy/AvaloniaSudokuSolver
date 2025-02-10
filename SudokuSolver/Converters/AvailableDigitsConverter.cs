using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace SudokuSolver.Converters
{
    public class AvailableDigitsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not IEnumerable<int> digits)
            {
                return new Avalonia.Data.BindingNotification(new ArgumentException("Argument is not IEnumerable<int>.", nameof(value)), Avalonia.Data.BindingErrorType.DataValidationError);
            }
            if (parameter is null)
            {
                if (targetType == typeof(string))
                {
                    if (digits.Any())
                    {
                        return string.Join(" ", digits.Order()); // Format as "1 3 5 7"
                    }
                    return string.Empty; // Empty if no available digits
                }
                else if (targetType == typeof(IBrush))
                {
                    double factor = digits.Count() / 9.0;
                    byte r = (byte)(Brushes.LightYellow.Color.R + ((Brushes.White.Color.R - Brushes.LightYellow.Color.R) * factor));
                    byte g = (byte)(Brushes.LightYellow.Color.G + ((Brushes.White.Color.G - Brushes.LightYellow.Color.G) * factor));
                    byte b = (byte)(Brushes.LightYellow.Color.B + ((Brushes.White.Color.B - Brushes.LightYellow.Color.B) * factor));
                    return new SolidColorBrush(new Color(255, r, g, b));
                }
                return new Avalonia.Data.BindingNotification(new ArgumentException("Missing parameter.", nameof(parameter)), Avalonia.Data.BindingErrorType.DataValidationError);
            }

            int digit;
            if (parameter is string text)
            {
                if (!int.TryParse(text, out digit))
                {
                    return new Avalonia.Data.BindingNotification(new ArgumentException("Failed to parse parameter as int.", nameof(parameter)), Avalonia.Data.BindingErrorType.DataValidationError);
                }
            }
            else
            {
                digit = System.Convert.ToInt32(parameter);
            }

            if(targetType == typeof(double))
            {
                return digits.Contains(digit) ? 100.0 : 0.0;
            }

            if(targetType == typeof(bool))
            {
                return digits.Contains(digit);
            }

            return new Avalonia.Data.BindingNotification(new NotImplementedException(), Avalonia.Data.BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => new Avalonia.Data.BindingNotification(new NotImplementedException(), Avalonia.Data.BindingErrorType.Error);
    }
}
