using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace SudokuSolver.Converters
{
    /// <summary>
    /// Converts a boolean value to a color. If the value is true, it returns black; otherwise, it returns blue.
    /// </summary>
    public class FixedToColorConverter : IValueConverter
    {
        /// <summary>
        /// Convert a boolean value to a color.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>Color.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType != typeof(IBrush))
            {
                return new Avalonia.Data.BindingNotification(new InvalidOperationException("The target must be a brush."), Avalonia.Data.BindingErrorType.DataValidationError, Brushes.Black);
            }
            else if (value is bool boolValue)
            {
                return boolValue ? Brushes.Black : Brushes.BlueViolet;
            }
            else
            {
                return new Avalonia.Data.BindingNotification(new InvalidOperationException("Input value is not a bool."), Avalonia.Data.BindingErrorType.DataValidationError, Brushes.Black);
            }
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">A user-defined parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted value.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => new Avalonia.Data.BindingNotification(new NotImplementedException(), Avalonia.Data.BindingErrorType.Error);
    }
}
