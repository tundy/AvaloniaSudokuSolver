using System.Text.Json.Serialization;

namespace SudokuSolver.Models
{
    public class Dosuku
    {
        [JsonPropertyName("newboard")]
        public DosukuNewboard? Newboard { get; set; }
    }

    public class DosukuNewboard
    {
        [JsonPropertyName("grids")]
        public DosukuGrid[]? Grids { get; set; }
    }

    public class DosukuGrid
    {
        [JsonPropertyName("difficulty")]
        public string? Difficulty { get; set; }
        [JsonPropertyName("value")]
        public int[][]? Value { get; set; }
    }
}
