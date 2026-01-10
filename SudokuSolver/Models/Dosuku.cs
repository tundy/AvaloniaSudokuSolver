using System.Text.Json.Serialization;

namespace SudokuSolver.Models
{
    /// <summary>
    /// Object for deserializing the response from the Dosuku API.
    /// </summary>
    public class Dosuku
    {
        /// <summary>
        /// Response containing a new sudoku.
        /// </summary>
        [JsonPropertyName("newboard")]
        public DosukuNewboard? Newboard { get; set; }
    }

    /// <summary>
    /// Response containing a new sudoku.
    /// </summary>
    public class DosukuNewboard
    {
        /// <summary>
        /// List of Sudoku grids.
        /// </summary>
        [JsonPropertyName("grids")]
        public DosukuGrid[]? Grids { get; set; }
    }

    /// <summary>
    /// Represents a sudoku grid with its difficulty and filled values.
    /// </summary>
    public class DosukuGrid
    {
        /// <summary>
        /// Difficulty of the sudoku (e.g. "easy", "medium", "hard").
        /// </summary>
        [JsonPropertyName("difficulty")]
        public string? Difficulty { get; set; }
        /// <summary>
        /// The filled values of the sudoku in a 2D array.
        /// </summary>
        [JsonPropertyName("value")]
        public int[][]? Value { get; set; }
        /// <summary>
        /// Solution of completed sudoku in a 2D array.
        /// </summary>
        [JsonPropertyName("solution")]
        public int[][]? Solution { get; set; }
    }
}
