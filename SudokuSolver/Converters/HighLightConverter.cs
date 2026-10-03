using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace SudokuSolver.Converters
{
    public class HighLightConverter : IValueConverter, IMultiValueConverter
    {
        /// <summary>
        /// Converts a boolean value to a brush.
        /// </summary>
        /// <param name="value">The boolean value to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted brush.</returns>
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (targetType != typeof(IBrush))
            {
                return new Avalonia.Data.BindingNotification(new InvalidOperationException("The target must be a brush."), Avalonia.Data.BindingErrorType.DataValidationError, Brushes.Black);
            }
            else if (value is bool boolValue)
            {
                return boolValue ? Brushes.Yellow : (object)Brushes.White;
            }
            else
            {
                return new Avalonia.Data.BindingNotification(new InvalidOperationException("Input value is not a bool."), Avalonia.Data.BindingErrorType.DataValidationError, Brushes.Black);
            }
        }
        /// <summary>
        /// Converts multiple values to a brush based on the provided boolean and list of integers.
        /// </summary>
        /// <param name="values">The values to convert.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted brush.</returns>
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            // Check if we have both required values
            if (values?.Count != 2)
                return new Avalonia.Data.BindingNotification(new NotSupportedException("Converter requires exactly 2 values"), Avalonia.Data.BindingErrorType.DataValidationError);

            // Get the boolean and list values
            if (values[0] is not bool boolValue || values[1] is not IReadOnlyCollection<int> list)
                return new Avalonia.Data.BindingNotification(new NotSupportedException("First value must be bool, second must be IReadOnlyList"), Avalonia.Data.BindingErrorType.DataValidationError);

            // If bool is true, return Yellow
            if (boolValue)
                return Brushes.Yellow;

            // If bool is false, interpolate between LightGreen and White
            int length = list.Count;
            if (length < 0 || length > 9)
                return new Avalonia.Data.BindingNotification(new ArgumentOutOfRangeException(nameof(values), "List length must be between 0 and 9"), Avalonia.Data.BindingErrorType.DataValidationError);

            double factor = (length + 1) / 10.0;
            var r = (byte)(Brushes.LightYellow.Color.R + ((Brushes.White.Color.R - Brushes.LightYellow.Color.R) * factor));
            var g = (byte)(Brushes.LightYellow.Color.G + ((Brushes.White.Color.G - Brushes.LightYellow.Color.G) * factor));
            var b = (byte)(Brushes.LightYellow.Color.B + ((Brushes.White.Color.B - Brushes.LightYellow.Color.B) * factor));
            return new SolidColorBrush(new Color(255, r, g, b));
        }
        /// <summary>
        /// Converts back from a brush to a boolean value. This method is not implemented and will throw a NotImplementedException if called.
        /// </summary>
        /// <param name="value">The brush to convert back.</param>
        /// <param name="targetType">The type of the target.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="culture">The culture to use.</param>
        /// <returns>The converted boolean value.</returns>
        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
            => new Avalonia.Data.BindingNotification(new NotImplementedException(), Avalonia.Data.BindingErrorType.Error);
    }
}
