using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Numerics;

namespace WpfApp1
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private Sudoku.Game game = new Sudoku.Game();

		public MainWindow()
		{
			InitializeComponent();
			InitGrid();
			CreateBlocks();
		}

		private void CreateBlocks()
		{
			game.matr = new SudokuBlock.Block[9, 9];
			for (int i = 0; i < 9; i++)
			{
				for (int j = 0; j < 9; j++)
				{
					SudokuBlock.Block block = new SudokuBlock.Block();
					block.Height = block.Width = 60;
					game.matr[i, j] = block;
					block.onClick += game.OnClick;
					Grid.SetRow(block, i);
					Grid.SetColumn(block, j);
					grid.Children.Add(block);
				}
			}
		}

		private void InitGrid()
		{
			for (int i = 0; i < 9; i++)
			{
				grid.ColumnDefinitions.Add(new ColumnDefinition());
			}

			for (int i = 0; i < 9; i++)
			{
				grid.RowDefinitions.Add(new RowDefinition());
			}
		}

		private void StartGame(object sender, RoutedEventArgs e)
		{
			grid.IsEnabled = true;
			game.GenerateField(slider.Value);
			game.BuildField();
		}

		private void Restart(object sender, RoutedEventArgs e)
		{
			game.BuildField();
		}
	}
}
