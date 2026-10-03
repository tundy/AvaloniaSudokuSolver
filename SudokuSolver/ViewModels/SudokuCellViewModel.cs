using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using SudokuSolver.Models;

namespace SudokuSolver.ViewModels
{
    public partial class SudokuCellViewModel : ObservableObject
    {
        private readonly SudokuCell _cell;

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuCellViewModel"/> class.
        /// </summary>
        /// <param name="cell">The Sudoku cell to wrap.</param>
        public SudokuCellViewModel(SudokuCell cell)
        {
            _cell = cell;
            _cell.PropertyChanged += OnCellPropertyChanged;  // Listen for property changes
            UpdateFromCell(); // Initialize the ViewModel from the model
        }
        /// <summary>
        /// Handles the PropertyChanged event of the SudokuCell model.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            UpdateFromCell();
        }
        /// <summary>
        /// Updates the ViewModel properties from the underlying SudokuCell model.
        /// </summary>
        private void UpdateFromCell()
        {
            Value = _cell.Value;
            Highlight = _cell.Highlight;
            IsFixed = _cell.IsFixed;
            AvailableDigits = _cell.AvailableDigits;
        }
        /// <summary>
        /// Gets or sets the value of the Sudoku cell. This property is observable and will notify any listeners when it changes.
        /// </summary>
        [ObservableProperty]
        public partial int? Value { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Sudoku cell is highlighted. This property is observable and will notify any listeners when it changes.
        /// </summary>
        [ObservableProperty]
        public partial bool Highlight { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether the Sudoku cell is fixed (i.e., part of the initial puzzle). This property is observable and will notify any listeners when it changes.
        /// </summary>
        [ObservableProperty]
        public partial bool IsFixed { get; set; }
        /// <summary>
        /// Gets or sets the collection of available digits for the Sudoku cell. This property is observable and will notify any listeners when it changes.
        /// </summary>
        [ObservableProperty]
        public partial IReadOnlyCollection<int> AvailableDigits { get; set; } = [];
    }
}
