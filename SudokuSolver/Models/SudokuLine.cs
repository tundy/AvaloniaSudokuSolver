using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver.Models
{
    /// <summary>
    /// Represents a line (row or column) in a Sudoku grid.
    /// </summary>
    public class SudokuLine : IReadOnlyList<SudokuCell>
    {
        /// <summary>
        /// The cells in the line.
        /// </summary>
        internal SudokuCell[] Cells { get; }

        /// <summary>
        /// The type of the line (row or column).
        /// </summary>
        public LineType LineType { get; }

        /// <summary>
        /// The index of the line in the grid.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Number of cells in the line.
        /// </summary>
        public int Count => Cells.Length;

        /// <summary>
        /// Gets the cell at the specified index.
        /// </summary>
        /// <param name="index">The index of the cell in the line.</param>
        /// <returns>The cell at the specified index.</returns>
        public SudokuCell this[int index] => Cells[index];

        public int[] GetAvailableDigitsOccurrence()
        {
            var availableDigitsCount = new int[9];
            foreach (SudokuCell cell in Cells.Where(x => x is not null))
            {
                if (cell.Value.HasValue)
                {
                    availableDigitsCount[cell.Value.Value - 1]++;
                }
                else
                {
                    foreach (int digit in cell.AvailableDigits)
                    {
                        availableDigitsCount[digit - 1]++;
                    }
                }
            }
            return availableDigitsCount;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuLine"/> class.
        /// </summary>
        /// <param name="grid">The grid that contains the line.</param>
        /// <param name="lineType">The type of the line (row or column).</param>
        /// <param name="index">The index of the line in the grid.</param>
        public SudokuLine(SudokuGrid grid, LineType lineType, int index)
        {
            Cells = new SudokuCell[9];
            LineType = lineType;
            Index = index;

            for (int i = 0; i < 9; i++)
            {
                if(lineType == LineType.Row)
                {
                    Cells[i] = grid.GetCell(index, i);
                    if(Cells[i] is not null)
                    {
                        Cells[i].Row = this;
                    }
                }
                else
                {
                    Cells[i] = grid.GetCell(i, index);
                    if (Cells[i] is not null)
                    {
                        Cells[i].Column = this;
                    }
                }
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the line.
        /// </summary>
        /// <returns>An enumerator for the cells in the line.</returns>
        public IEnumerator<SudokuCell> GetEnumerator() => ((IEnumerable<SudokuCell>)Cells).GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through the cells in the line.
        /// </summary>
        /// <returns>An enumerator for the cells in the line.</returns>
        IEnumerator IEnumerable.GetEnumerator() => Cells.GetEnumerator();
    }
}
