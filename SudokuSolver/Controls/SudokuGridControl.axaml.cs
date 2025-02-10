using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using SudokuSolver.Models;

namespace SudokuSolver;

public partial class SudokuGridControl : UserControl
{
    public SudokuGridControl()
    {
        InitializeComponent();
    }
    public void Initialize(SudokuGrid grid)
    {
        var uniformGrid = this.FindControl<UniformGrid>("grid")!;
        uniformGrid.Children.Clear();
        for (int i = 0; i < 9; i++)
        {
            var box = grid.GetBox(i);
            var boxControl = new SudokuBoxControl();
            boxControl.Initialize(box);
            uniformGrid.Children.Add(boxControl);
            Grid.SetRow(boxControl, box.RowIndex);
            Grid.SetColumn(boxControl, box.ColumnIndex);
        }
    }
}