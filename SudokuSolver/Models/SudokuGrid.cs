using Avalonia.Controls;
using SudokuSolver.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SudokuSolver.Models
{
    /// <summary>
    /// Represents the entire Sudoku grid.
    /// </summary>
    [Newtonsoft.Json.JsonObject]
    public class SudokuGrid : IReadOnlyList<SudokuCell>
    {
        public string JsonSerialize() => JsonSerialize(this);
        public static string JsonSerialize(SudokuGrid grid) => Newtonsoft.Json.JsonConvert.SerializeObject(grid);
        public static SudokuGrid? JsonDeserialize(string json) => Newtonsoft.Json.JsonConvert.DeserializeObject<SudokuGrid>(json);

        public void Load(ReadOnlySpan<char> cells)
        {
            if (cells.Length != 81)
            {
                throw new ArgumentException("Invalid input", nameof(cells));
            }
            for (int i = 0; i < cells.Length; i++)
            {
                var cell = GetCell(i);
                if(char.IsAsciiDigit(cells[i]) && cells[i] != '0')
                {
                    cell.Value = cells[i] - '0';
                    cell.IsFixed = true;
                }
                else
                {
                    cell.Value = null;
                    cell.IsFixed = false;
                }
            }
        }

        public void Load(string cells)
        {
            if(string.IsNullOrWhiteSpace(cells))
            {
                throw new ArgumentException("Invalid input", nameof(cells));
            }
            Load(cells.AsSpan());
        }

        /// <summary>
        /// The 3x3 boxes in the grid.
        /// </summary>
        public SudokuBox[,] Boxes { get; }

        /// <summary>
        /// The rows in the grid.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuLine[] Rows { get; }

        /// <summary>
        /// The columns in the grid.
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public SudokuLine[] Columns { get; }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int Count => Boxes.Length;

        public SudokuCell this[int index] => GetCell(index);

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuGrid"/> class.
        /// </summary>
        public SudokuGrid()
        {
            Boxes = new SudokuBox[3, 3];
            Rows = new SudokuLine[9];
            Columns = new SudokuLine[9];
            int boxIndex = 0;
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Boxes[r, c] = new SudokuBox(this, boxIndex++, r, c);
                }
            }
            for (int i = 0; i < 9; i++)
            {
                Rows[i] = new SudokuLine(this, LineType.Row, i);
                Columns[i] = new SudokuLine(this, LineType.Column, i);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuGrid"/> class.
        /// </summary>
        /// <param name="boxes">Array of deserialized boxes.</param>
        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public SudokuGrid(SudokuBox[,] boxes)
        {
            Boxes = boxes ?? throw new ArgumentNullException(nameof(boxes));
            Rows = new SudokuLine[9];
            Columns = new SudokuLine[9];
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    Boxes[r, c].SetParent(this);
                }
            }
            for (int i = 0; i < 9; i++)
            {
                Rows[i] = new SudokuLine(this, LineType.Row, i);
                Columns[i] = new SudokuLine(this, LineType.Column, i);
            }
        }

        /// <summary>
        /// Gets the cell at the specified row and column.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>The cell at the specified row and column.</returns>
        public SudokuCell GetCell(int row, int col)
        {
            int boxRow = row / 3;
            int boxCol = col / 3;
            int cellRow = row % 3;
            int cellCol = col % 3;
            return Boxes[boxRow, boxCol].Cells[cellRow, cellCol];
        }

        private SudokuCell GetCell(int index)
        {
            int cellRow = index % 3;
            int cellCol = index / 3;
            return GetCell(cellRow, cellCol);
        }

        /// <summary>
        /// Gets the box at the specified index.
        /// </summary>
        /// <param name="index">The index of the box.</param>
        /// <returns>The box at the specified index.</returns>
        public SudokuBox GetBox(int index)
        {
            int cellRow = index % 3;
            int cellCol = index / 3;
            return Boxes[cellRow, cellCol];
        }

        /// <summary>
        /// Sets the value of the cell at the specified row and column.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <param name="value">The value to set.</param>
        /// <param name="isFixed">Whether the cell is fixed or not.</param>
        public void SetCell(int row, int col, int value, bool isFixed = false)
        {
            var cell = GetCell(row, col);
            cell.Value = value;
            cell.IsFixed = isFixed;
        }

        /// <summary>
        /// Gets the line (row or column) at the specified index.
        /// </summary>
        /// <param name="lineType">The type of line (row or column).</param>
        /// <param name="index">The index of the line.</param>
        /// <returns>The line at the specified index.</returns>
        public SudokuLine GetLine(LineType lineType, int index)
        {
            return lineType == LineType.Row ? Rows[index] : Columns[index];
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the box.
        /// </summary>
        /// <returns>An enumerator for the cells in the box.</returns>
        public IEnumerator<SudokuCell> GetEnumerator()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    foreach (var cell in Boxes[r, c])
                    {
                        yield return cell;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the box.
        /// </summary>
        /// <returns>An enumerator for the cells in the box.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void XWingColumns()
        {
            var pairs = Columns.Permutations(2);
            foreach (var pair in pairs)
            {
                var firstRow = pair[0].GetAvailableDigitsOccurrence();
                var secondRow = pair[1].GetAvailableDigitsOccurrence();
                for (int i = 0; i < 9; i++)
                {
                    if (firstRow[i] == 2 && secondRow[i] == 2)
                    {
                        var cells1 = pair[0].Where(x => x.AvailableDigits.Contains(i + 1)).OrderBy(x => x.ColumnIndex).ToImmutableList();
                        var cells2 = pair[1].Where(x => x.AvailableDigits.Contains(i + 1)).OrderBy(x => x.ColumnIndex).ToImmutableList();
                        if (cells1[0].RowIndex == cells2[0].RowIndex
                            && cells1[1].RowIndex == cells2[1].RowIndex)
                        {
                            foreach (var mainCell in cells1)
                            {
                                foreach (var cell in mainCell.Column)
                                {
                                    if (cell != cells1[0] && cell != cells1[1])
                                    {
                                        cell.AvailableDigits.Remove(i + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void XWingRows()
        {
            var pairs = Rows.Permutations(2);
            foreach (var pair in pairs)
            {
                var firstRow = pair[0].GetAvailableDigitsOccurrence();
                var secondRow = pair[1].GetAvailableDigitsOccurrence();
                for (int i = 0; i < 9; i++)
                {
                    if (firstRow[i] == 2 && secondRow[i] == 2)
                    {
                        var cells1 = pair[0].Where(x => x.AvailableDigits.Contains(i + 1)).OrderBy(x => x.RowIndex).ToImmutableList();
                        var cells2 = pair[1].Where(x => x.AvailableDigits.Contains(i + 1)).OrderBy(x => x.RowIndex).ToImmutableList();
                        if (cells1[0].ColumnIndex == cells2[0].ColumnIndex
                            && cells1[1].ColumnIndex == cells2[1].ColumnIndex)
                        {
                            foreach (var mainCell in cells1)
                            {
                                foreach (var cell in mainCell.Row)
                                {
                                    if (cell != cells1[0] && cell != cells1[1])
                                    {
                                        cell.AvailableDigits.Remove(i + 1);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
