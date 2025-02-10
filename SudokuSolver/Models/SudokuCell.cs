using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using SudokuSolver.Extensions;

namespace SudokuSolver.Models
{
    /// <summary>
    /// Single cell of Sudoku grid.
    /// </summary>
    public class SudokuCell : INotifyPropertyChanged
    {
        private int? _value;
        private bool _isFixed;
        private bool _highlight;
        private HashSet<int> _availableDigits = new(Enumerable.Range(1, 9));

        /// <summary>
        /// Gets or sets a value indicating whether the cell is highlighted.
        /// </summary>
        public bool Highlight
        {
            get => _highlight;
            set
            {
                if (_highlight != value)
                {
                    _highlight = value;
                    OnPropertyChanged(nameof(Highlight));
                }
            }
        }

        /// <summary>
        /// Actual written value of the cell. Null if the cell is empty.
        /// </summary>
        public int? Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (value.HasValue)
                    {
                        AvailableDigits = [value.Value];
                    }
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        /// <summary>
        /// Whether the cell is fixed or not. Fixed cells cannot be changed by the user.
        /// </summary>
        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                if (_isFixed != value)
                {
                    _isFixed = value;
                    OnPropertyChanged(nameof(IsFixed));
                }
            }
        }

        /// <summary>
        /// The box that contains this cell.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuBox Box { get; private set; }

        /// <summary>
        /// Shortcut to the grid that contains this cell.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuGrid Grid => Box.Grid;

        /// <summary>
        /// The index of this box in the grid.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int BoxIndex => Box.BoxIndex;

        /// <summary>
        /// The row index of the cell in the grid.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// The column index of the cell in the grid.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// The row and column of the cell in the grid.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuLine Row { get; internal set; } = null!;

        /// <summary>
        /// The row and column of the cell in the grid.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuLine Column { get; internal set; } = null!;

        /// <summary>
        /// The available digits for this cell.
        /// </summary>
        public HashSet<int> AvailableDigits
        {
            get => _availableDigits;
            private set
            {
                if (_availableDigits != value)
                {
                    _availableDigits = value;
                    OnPropertyChanged(nameof(AvailableDigits));
                }
            }
        }

        /// <summary>
        /// The available digits for this cell, that are available only for this box's row.
        /// </summary>
        public ImmutableHashSet<int> AvailableDigitsForRowInBox { get; private set; } = [];

        /// <summary>
        /// The available digits for this cell, that are available only for this box's column.
        /// </summary>
        public ImmutableHashSet<int> AvailableDigitsForColumnInBox { get; private set; } = [];

        /// <summary>
        /// Creates a new cell in the specified box at the specified row and column.
        /// </summary>
        /// <param name="box">The box that contains this cell.</param>
        /// <param name="rowIndex">The row index of the cell in the grid.</param>
        /// <param name="columnIndex">The column index of the cell in the grid.</param>
        public SudokuCell(SudokuBox box, int rowIndex, int columnIndex)
        {
            Box = box ?? throw new ArgumentNullException(nameof(box));
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            SetParent(box);
        }

        /// <summary>
        /// Creates a new cell with the specified available digits and available digits for row and column in the box.
        /// </summary>
        /// <param name="rowIndex">The row index of the cell in the grid.</param>
        /// <param name="columnIndex">The column index of the cell in the grid.</param>
        /// <param name="availableDigits">The available digits for this cell.</param>
        /// <param name="availableDigitsForRowInBox">The available digits for this cell, that are available only for this box's row.</param>
        /// <param name="availableDigitsForColumnInBox">The available digits for this cell, that are available only for this box's column.</param>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public SudokuCell(int rowIndex, int columnIndex, HashSet<int> availableDigits, ImmutableHashSet<int> availableDigitsForRowInBox, ImmutableHashSet<int> availableDigitsForColumnInBox)
        {
            _availableDigits = availableDigits ?? throw new ArgumentNullException(nameof(availableDigits));
            AvailableDigitsForRowInBox = availableDigitsForRowInBox ?? throw new ArgumentNullException(nameof(availableDigitsForRowInBox));
            AvailableDigitsForColumnInBox = availableDigitsForColumnInBox ?? throw new ArgumentNullException(nameof(availableDigitsForColumnInBox));
            Box = null!;
            Column = null!;
            Row = null!;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }

        /// <summary>
        /// Update the parent of the cell.
        /// </summary>
        /// <param name="box">The box that contains this cell.</param>
        public void SetParent(SudokuBox box)
        {
            Box = box ?? throw new ArgumentNullException(nameof(box));
            if (Grid is not null)
            {
                Column = Grid.GetLine(LineType.Column, ColumnIndex);
                if (Column is not null)
                {
                    Column.Cells[ColumnIndex] = this;
                }
                Row = Grid.GetLine(LineType.Row, RowIndex);
                if (Row is not null)
                {
                    Row.Cells[RowIndex] = this;
                }
            }
        }

        /// <summary>
        /// Resets the available digits for this cell to all possible digits (1-9).
        /// </summary>
        public void ResetAvailableDigits()
        {
            AvailableDigits = new HashSet<int>(Enumerable.Range(1, 9));
        }

        /// <summary>
        /// Determines if the cell has a naked or hidden single digit.
        /// </summary>
        /// <returns>The naked or hidden single digit if found, otherwise null.</returns>
        public int? NakedHiddenSingle()
        {
            if (Value.HasValue)
            {
                return Value.Value;
            }
            if (AvailableDigits.Count == 1)
            {
                return AvailableDigits.First();
            }

            foreach (var digit in AvailableDigits)
            {
                if (IsDigitUniqueInCells(Box, digit, this) ||
                    IsDigitUniqueInCells(Row, digit, this) ||
                    IsDigitUniqueInCells(Column, digit, this))
                {
                    return digit;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the available digits for this cell.
        /// </summary>
        public void RemoveUsedOptions()
        {
            void RemoveUsedDigits(IEnumerable<SudokuCell> cells)
            {
                foreach (var cell in cells)
                {
                    if (cell == this)
                    {
                        continue;
                    }
                    if (cell.Value.HasValue)
                    {
                        AvailableDigits.Remove(cell.Value.Value);
                    }
                    if (Box != cell.Box)
                    {
                        if (cell.RowIndex == RowIndex)
                        {
                            AvailableDigits.RemoveWhere(cell.AvailableDigitsForRowInBox.Contains);
                        }
                        else if (cell.ColumnIndex == ColumnIndex)
                        {
                            AvailableDigits.RemoveWhere(cell.AvailableDigitsForColumnInBox.Contains);
                        }
                    }
                }
            }
            RemoveUsedDigits(Box);
            RemoveUsedDigits(Row);
            RemoveUsedDigits(Column);
            AvailableDigits = [.. AvailableDigits];
        }

        /// <summary>
        /// Updates the available digits for this cell, that are available only for this box's row.
        /// </summary>
        public void PointingPairsRow()
        {
            var availableDigits = new HashSet<int>(AvailableDigits);
            // Get all cells in the box except those in the same row.
            var otherCellsInBox = Box.Where(cell => cell.RowIndex != RowIndex);
            // Remove digits used in other rows within the box.
            foreach (var cell in otherCellsInBox)
            {
                if (cell.Value.HasValue)
                {
                    availableDigits.Remove(cell.Value.Value);
                }
                else
                {
                    availableDigits.RemoveWhere(cell.AvailableDigits.Contains);
                }
            }
            AvailableDigitsForRowInBox = [.. availableDigits];
        }

        /// <summary>
        /// Updates the available digits for this cell, that are available only for this box's column.
        /// </summary>
        public void PointingPairsColumn()
        {
            var availableDigits = new HashSet<int>(AvailableDigits);
            // Get all cells in the box except those in the same column.
            var otherCellsInBox = Box.Where(cell => cell.ColumnIndex != ColumnIndex);
            // Remove digits used in other column within the box.
            foreach (var cell in otherCellsInBox)
            {
                if (cell.Value.HasValue)
                {
                    availableDigits.Remove(cell.Value.Value);
                }
                else
                {
                    availableDigits.RemoveWhere(cell.AvailableDigits.Contains);
                }
            }
            AvailableDigitsForColumnInBox = [.. availableDigits];
        }

        /// <summary>
        /// Performs box-line reduction for the row.
        /// </summary>
        public void BoxLineReductionRow()
        {
            var rowCellsSameBox = Row.Where(cell => cell.Box == Box);
            var rowCellsDifferentBox = Row.Where(cell => cell.Box != Box);
            var availableDigitsInBoxRow = rowCellsSameBox.SelectMany(x => x.AvailableDigits).Distinct();
            foreach (var digit in availableDigitsInBoxRow)
            {
                if (IsDigitUniqueInCells(rowCellsDifferentBox, digit, this))
                {
                    var boxCellsFromDifferentRow = Box.Where(cell => cell.RowIndex != RowIndex);
                    foreach (var cell in boxCellsFromDifferentRow)
                    {
                        cell.AvailableDigits.Remove(digit);
                        cell.AvailableDigits = [.. cell.AvailableDigits];
                    }
                }
            }
        }

        /// <summary>
        /// Performs box-line reduction for the column.
        /// </summary>
        public void BoxLineReductionColumn()
        {
            var columnCellsSameBox = Column.Where(cell => cell.Box == Box);
            var columnCellsDifferentBox = Column.Where(cell => cell.Box != Box);
            var availableDigitsInBoxRow = columnCellsSameBox.SelectMany(x => x.AvailableDigits).Distinct();
            foreach (var digit in availableDigitsInBoxRow)
            {
                if (IsDigitUniqueInCells(columnCellsDifferentBox, digit, this))
                {
                    var boxCellsFromDifferentColumn = Box.Where(cell => cell.ColumnIndex != ColumnIndex);
                    foreach (var cell in boxCellsFromDifferentColumn)
                    {
                        cell.AvailableDigits.Remove(digit);
                        cell.AvailableDigits = [.. cell.AvailableDigits];
                    }
                }
            }
        }

        /// <summary>
        /// Finds hidden sets of the specified size in the cell's box, row, and column.
        /// </summary>
        /// <param name="count">The size of the hidden set.</param>
        public void HiddenSet(int count)
        {
            HiddenSet(Box, count);
            HiddenSet(Column, count);
            HiddenSet(Row, count);
        }

        /// <summary>
        /// Finds hidden sets of the specified size in the given set of cells.
        /// </summary>
        /// <param name="set">The set of cells to search for hidden sets.</param>
        /// <param name="count">The size of the hidden set.</param>
        public static void HiddenSet(IReadOnlyList<SudokuCell> set, int count)
        {
            var permutations = set.Where(cell => !cell.Value.HasValue).ToImmutableList().Permutations(count);
            foreach (var permutation in permutations)
            {
                var availableDigits = new HashSet<int>();
                foreach (var cell in permutation)
                {
                    availableDigits.UnionWith(cell.AvailableDigits);
                }
                if (availableDigits.Count == count)
                {
                    foreach (var cell in set)
                    {
                        if (!permutation.Contains(cell))
                        {
                            foreach (var digit in availableDigits)
                            {
                                cell.AvailableDigits.Remove(digit);
                            }
                            cell.AvailableDigits = [.. cell.AvailableDigits];
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds naked sets of the specified size in the cell's box, row, and column.
        /// </summary>
        /// <param name="setSize">The size of the naked set.</param>
        public void NakedSet(int setSize)
        {
            NakedSet(Box, setSize);
            NakedSet(Row, setSize);
            NakedSet(Column, setSize);
        }

        /// <summary>
        /// Finds naked sets of the specified size in the given set of cells.
        /// </summary>
        /// <param name="cells">The set of cells to search for naked sets.</param>
        /// <param name="setSize">The size of the naked set.</param>
        public void NakedSet(IEnumerable<SudokuCell> cells, int setSize)
        {
            if (AvailableDigits.Count == setSize)
            {
                List<SudokuCell> foundSame = new(9);
                var otherCells = cells.Where(cell => cell != this);
                foreach (var cell in otherCells)
                {
                    var availableDigits = new HashSet<int>(cell.AvailableDigits);
                    availableDigits.RemoveWhere(AvailableDigits.Contains);
                    if (availableDigits.Count == 0)
                    {
                        foundSame.Add(cell);
                    }
                }
                if (foundSame.Count == setSize - 1)
                {
                    var notMatched = otherCells.Where(x => !foundSame.Contains(x));
                    foreach (var cell in notMatched)
                    {
                        foreach (var digit in AvailableDigits)
                        {
                            cell.AvailableDigits.Remove(digit);
                        }
                        cell.AvailableDigits = [.. cell.AvailableDigits];
                    }
                }
            }
        }

        /// <summary>
        /// Event that is raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Checks if the specified digit is unique in the specified cells.
        /// </summary>
        /// <param name="cells">The collection of cells to check.</param>
        /// <param name="digit">The digit to check for uniqueness.</param>
        /// <param name="currentCell">The current cell being checked.</param>
        /// <returns>True if the digit is unique in the specified cells, otherwise false.</returns>
        public static bool IsDigitUniqueInCells(IEnumerable<SudokuCell> cells, int digit, SudokuCell currentCell)
        {
            foreach (var cell in cells)
            {
                if (cell == currentCell)
                {
                    continue;
                }
                if (cell.Value == digit)
                {
                    return false;
                }
                if (!cell.Value.HasValue && cell.AvailableDigits.Contains(digit))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
