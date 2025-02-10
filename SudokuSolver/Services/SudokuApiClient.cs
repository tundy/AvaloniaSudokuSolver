using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SudokuSolver.Models;

namespace SudokuSolver.Services;

public static class SudokuApiClient
{
    private static readonly HttpClient _httpClient = new();

    public static async Task<SudokuGrid>  DownloadSudokuAsync()
    {
        const string url = "https://sudoku-api.vercel.app/api/dosuku?query={newboard(limit:1){grids{difficulty,value}}}";
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        var dosuku = JsonSerializer.Deserialize<Dosuku>(responseBody!);
        var values = dosuku!.Newboard!.Grids![0]!.Value!;
        return ConvertToSudokuGrid(values);
    }

    private static SudokuGrid ConvertToSudokuGrid(int[][] digits)
    {
        SudokuGrid grid = new();
        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                int cellValue = digits[row][col];
                if (cellValue != 0)
                {
                    grid.SetCell(row, col, cellValue, true);
                }
            }
        }
        return grid;
    }
}
