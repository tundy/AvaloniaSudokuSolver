using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Models
{
    /// <summary>
    /// Represents a 3x3 box in a Sudoku grid.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    public class SudokuBox : IReadOnlyList<SudokuCell>
    {
        /// <summary>
        /// The cells contained in this box.
        /// </summary>
        public SudokuCell[,] Cells { get; }

        /// <summary>
        /// The index of this box in the grid.
        /// </summary>
        public int BoxIndex { get; }

        /// <summary>
        /// The row index of this box in the grid.
        /// </summary>
        public int RowIndex { get; }

        /// <summary>
        /// The column index of this box in the grid.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        /// The grid that contains this box.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuGrid Grid { get; private set; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Count => Cells.Length;

        public SudokuCell this[int index] => GetCell(index);

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuBox"/> class.
        /// </summary>
        /// <param name="grid">The grid that contains this box.</param>
        /// <param name="boxIndex">The index of this box in the grid.</param>
        /// <param name="rowIndex">The row index of this box in the grid.</param>
        /// <param name="columnIndex">The column index of this box in the grid.</param>
        public SudokuBox(SudokuGrid grid, int boxIndex, int rowIndex, int columnIndex)
        {
            Grid = grid;
            BoxIndex = boxIndex;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Cells = new SudokuCell[3, 3];
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Cells[r, c] = new SudokuCell(this, (rowIndex * 3) + r, (columnIndex * 3) + c);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuBox"/> class.
        /// </summary>
        /// <param name="boxIndex">The index of this box in the grid.</param>
        /// <param name="rowIndex">The row index of this box in the grid.</param>
        /// <param name="columnIndex">The column index of this box in the grid.</param>
        /// <param name="cells">Array of deserialized cells.</param>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public SudokuBox(int boxIndex, int rowIndex, int columnIndex, SudokuCell[,] cells)
        {
            Grid = null!;
            BoxIndex = boxIndex;
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
            Cells = cells;
            foreach (var cell in this)
            {
                cell.SetParent(this);
            }
        }

        /// <summary>
        /// Update the parent of the cell.
        /// </summary>
        /// <param name="grid">The grid that contains this box.</param>
        public void SetParent(SudokuGrid grid)
        {
            Grid = grid ?? throw new ArgumentNullException(nameof(grid));
            foreach (var cell in this)
            {
                cell.SetParent(this);
            }
        }

        /// <summary>
        /// Gets the cell at the specified index in the box.
        /// </summary>
        /// <param name="index">The index of the cell in the box (0-8).</param>
        /// <returns>The cell at the specified index.</returns>
        private SudokuCell GetCell(int index)
        {
            int cellRow = index % 3;
            int cellCol = index / 3;
            return Cells[cellRow, cellCol];
        }

        /// <summary>
        /// Get missing digits in this box.
        /// </summary>
        /// <returns>Set of missing digits in this box.</returns>
        public HashSet<int> GetMissingDigits()
        {
            var missingDigits = new HashSet<int>(Enumerable.Range(1, 9));
            foreach (var cell in this)
            {
                if (cell.Value.HasValue)
                {
                    missingDigits.Remove(cell.Value.Value);
                }
            }
            return missingDigits;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the box.
        /// </summary>
        /// <returns>An enumerator for the cells in the box.</returns>
        public IEnumerator<SudokuCell> GetEnumerator()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    yield return Cells[i, j];
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the box.
        /// </summary>
        /// <returns>An enumerator for the cells in the box.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
