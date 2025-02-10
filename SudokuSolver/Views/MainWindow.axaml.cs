using Avalonia.Controls;
using Avalonia.Threading;
using SudokuSolver.Models;
using SudokuSolver.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MoreLinq;
using System.Collections.Immutable;

namespace SudokuSolver.Views
{
    public partial class MainWindow : Window
    {
        private SudokuGrid _grid = null!;
        private readonly SudokuGridControl _sudokuGridControl = new();

        public MainWindow()
        {
            InitializeComponent();
            Grid.SetRow(_sudokuGridControl, 0);
            Grid.SetColumn(_sudokuGridControl, 0);
            var uniformGrid = this.FindControl<Grid>("grid")!;
            uniformGrid.Children.Add(_sudokuGridControl);
            DownloadNewSudoku().ConfigureAwait(false);
            _timer = new(UpdateInterval, DispatcherPriority.Normal, async (_, _) => await UpdateCellsAsync());
            _timer.Start();
        }

        private async Task DownloadNewSudoku()
        {
            _grid = await SudokuApiClient.DownloadSudokuAsync();
            _sudokuGridControl.Initialize(_grid);
            Guesses = [];
        }

        private readonly TimeSpan WaitDelay = TimeSpan.FromSeconds(2);
        private readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(1);
        private readonly DispatcherTimer _timer;

        private string LastSudoku = string.Empty;
        private Stack<string> Guesses = null!;

        private void NextGuess()
        {
            LastSudoku = Guesses.Pop();
            _grid = SudokuGrid.JsonDeserialize(LastSudoku)!;
            _sudokuGridControl.Initialize(_grid);
        }

        private async Task DeadEnd()
        {
            if (Guesses.Count != 0)
            {
                NextGuess();
            }
            else
            {
                await Task.Delay(WaitDelay);
                await DownloadNewSudoku();
            }
        }

        private async Task UpdateCellsAsync()
        {
            if (_grid is null)
            {
                return;
            }
            _timer.Stop();
            _grid.XWingColumns();
            _grid.XWingRows();
            foreach (var cell in _grid)
            {
                if (cell.Value is not null)
                {
                    continue;
                }
                cell.Highlight = true;
                cell.RemoveUsedOptions();
                cell.PointingPairsRow();
                cell.PointingPairsColumn();
                cell.BoxLineReductionRow();
                cell.BoxLineReductionColumn();
                cell.NakedSet(2);
                cell.NakedSet(3);
                cell.NakedSet(4);
                cell.NakedSet(5);
                cell.HiddenSet(2);
                cell.HiddenSet(3);
                cell.HiddenSet(4);
                cell.HiddenSet(5);
                cell.Value = cell.NakedHiddenSingle();
                await Task.Delay(UpdateInterval);
                cell.Highlight = false;

                if(cell.AvailableDigits.Count == 0)
                {
                    await DeadEnd();
                    _timer.Start();
                    return;
                }
            }

            var allSolved = true;
            var anyEmpty = false;
            foreach (var cell in _grid)
            {
                if (!cell.Value.HasValue)
                {
                    allSolved = false;
                    if(cell.AvailableDigits.Count == 0)
                    {
                        anyEmpty = true;
                        break;
                    }
                    break;
                }
            }

            if (allSolved)
            {
                await Task.Delay(WaitDelay);
                await DownloadNewSudoku();
            }
            else if(anyEmpty)
            {
                await DeadEnd();
            }
            else
            {
                var currentSudoku = _grid.JsonSerialize();
                if (LastSudoku == currentSudoku)
                {
                    var leastAvailableDigits = _grid.Where(x => !x.Value.HasValue).Minima(x => x.AvailableDigits.Count).ToImmutableList();
                    if(leastAvailableDigits.IsEmpty || leastAvailableDigits[0].AvailableDigits.Count == 0) // If there are no available digits, we need to backtrack
                    {
                        await DeadEnd();
                    }
                    else
                    {
                        foreach(var cell in leastAvailableDigits)
                        {
                            foreach(var digit in cell.AvailableDigits)
                            {
                                cell.Value = digit;
                                cell.Highlight = true;
                                Guesses.Push(_grid.JsonSerialize());
                                cell.Highlight = false;
                                cell.Value = null;
                            }
                        }
                        NextGuess();
                    }
                }
                else
                {
                    LastSudoku = currentSudoku;
                }
            }
            _timer.Start();
        }
    }
}