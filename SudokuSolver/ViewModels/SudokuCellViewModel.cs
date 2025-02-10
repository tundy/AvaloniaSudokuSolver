using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel;
using SudokuSolver.Models;

namespace SudokuSolver.ViewModels
{
    public partial class SudokuCellViewModel : ObservableObject
    {
        private readonly SudokuCell _cell;

        // Constructor that takes a Cell model
        public SudokuCellViewModel(SudokuCell cell)
        {
            _cell = cell;
            _cell.PropertyChanged += OnCellPropertyChanged;  // Listen for property changes
            UpdateFromCell(); // Initialize the ViewModel from the model
        }

        private void OnCellPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Update ViewModel properties when model changes
            UpdateFromCell();
        }

        private void UpdateFromCell()
        {
            Value = _cell.Value;
            Highlight = _cell.Highlight;
            IsFixed = _cell.IsFixed;
            AvailableDigits = _cell.AvailableDigits;
        }

        [ObservableProperty]
        private int? value;

        [ObservableProperty]
        private bool highlight;

        [ObservableProperty]
        private bool isFixed;

        [ObservableProperty]
        private IReadOnlyCollection<int> availableDigits = [];
    }
}
