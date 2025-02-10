using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using SudokuSolver.Models;
using SudokuSolver.ViewModels;

namespace SudokuSolver;

public partial class SudokuBoxControl : UserControl
{
    public SudokuBoxControl()
    {
        InitializeComponent();
    }

    public void Initialize(SudokuBox box)
    {
        var uniformGrid = this.FindControl<UniformGrid>("grid")!;
        uniformGrid.Children.Clear();
        for (int i = 0; i < 9; i++)
        {
            var cell = box[i];
            var cellControl = new SudokuCellControl
            {
                DataContext = new SudokuCellViewModel(cell)
            };
            uniformGrid.Children.Add(cellControl);
            Grid.SetRow(cellControl, cell.RowIndex);
            Grid.SetColumn(cellControl, cell.ColumnIndex);
        }
    }
}