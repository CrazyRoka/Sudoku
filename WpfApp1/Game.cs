using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Sudoku
{
    class Game
    {
		public SudokuBlock.Block[,] matr;
		private int[,] colorMatrix = new int[3,3];
		private int[,] startValues;
		internal void OnClick()
		{
			CheckTable();
		}

		internal void GenerateField(double val)
		{
			val = 10.0 - val;
			SudokuGenerator generator = new SudokuGenerator();
			startValues = generator.Generate();
			Random rand = new Random();
			int numberOfVals = 81 - rand.Next((int)Math.Round(val + 1) * 5, (int)Math.Round(val + 2) * 5);
			while(numberOfVals != 0)
			{
				int x = rand.Next(0, 9);
				int y = rand.Next(0, 9);
				if (startValues[x, y] == 0) continue;
				startValues[x, y] = 0;
				numberOfVals--;
			}

		}


		internal void BuildField()
		{
			if (startValues == null) return;
			for (int i = 0; i < 9; i++)
				for (int j = 0; j < 9; j++)
				{
					matr[i, j].value = startValues[i, j];
					if (startValues[i, j] != 0) matr[i, j].Locked = true;
					else matr[i, j].Locked = false;
				}
			CheckTable();
		}

		
		private void CheckTable()
		{
			HashSet<int> valueSet1 = new HashSet<int>();
			HashSet<int> valueSet2 = new HashSet<int>();
			HashSet<int>[,] valueSet3 = new HashSet<int>[3, 3];
			HashSet<KeyValuePair<String, int>> badValues = new HashSet<KeyValuePair<String, int>>();
			HashSet<KeyValuePair<String, int>> goodValues = new HashSet<KeyValuePair<String, int>>();
			for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) valueSet3[i, j] = new HashSet<int>();
			for (int i = 0; i < 9; i++)
			{
				valueSet1.Clear();
				valueSet2.Clear();
				for (int j = 0; j < 9; j++)
				{
					CheckDuplicates(matr[i, j].value, valueSet1, badValues, i, "Row");
					CheckDuplicates(matr[j, i].value, valueSet2, badValues, i, "Column");
					CheckDuplicates(matr[i, j].value, valueSet3[i / 3, j / 3], badValues, i / 3 * 3 + j / 3, "Box");
					if (valueSet3[i / 3, j / 3].Count == 9)
					{
						goodValues.Add(new KeyValuePair<string, int>("Box", i / 3 * 3 + j / 3));
					}
				}
				if (valueSet2.Count == 9) goodValues.Add(new KeyValuePair<string, int>("Column", i));
				if (valueSet1.Count == 9) goodValues.Add(new KeyValuePair<string, int>("Row", i));
			}
			BuildColorMatrix(badValues, goodValues);
			ApplyChanges();
			CheckWin(goodValues.Count);
		}

		private void CheckWin(int count)
		{
			if (count == 9 + 9 + 9)
			{
				LockBlocks();
				MessageBox.Show("Congratulations!");
			}
		}

		private void LockBlocks()
		{
			for (int i = 0; i < 9; i++)
				for (int j = 0; j < 9; j++) matr[i, j].Locked = true;
		}

		private void BuildColorMatrix(HashSet<KeyValuePair<String, int>> badValues, HashSet<KeyValuePair<String, int>> goodValues)
		{
			ClearColors();
			foreach (var g in badValues)
			{
				if (g.Key == "Row") for (int j = 0; j < 9; j++) colorMatrix[g.Value, j] = -1;
				if (g.Key == "Column") for (int i = 0; i < 9; i++) colorMatrix[i, g.Value] = -1;
				if (g.Key == "Box") for (int r = g.Value / 3 * 3, i = r; i < r + 3; i++) for (int c = g.Value % 3 * 3, j = c; j < c + 3; j++) colorMatrix[i, j] = -1;
			}

			foreach (var g in goodValues)
			{
				if (g.Key == "Row")
				{
					bool t = true;
					for (int j = 0; j < 9; j++) t &= colorMatrix[g.Value, j] != -1;
					if(t)
					for (int j = 0; j < 9; j++) colorMatrix[g.Value, j] = 1;
				}
				if (g.Key == "Column")
				{
					bool t = true;
					for (int i = 0; i < 9; i++) t &= colorMatrix[i, g.Value] != -1;
					if (t)
					for (int i = 0; i < 9; i++) colorMatrix[i, g.Value] = 1;
				}
				if (g.Key == "Box")
				{
					bool t = true;
					for (int r = g.Value / 3 * 3, i = r; i < r + 3; i++) for (int c = g.Value % 3 * 3, j = c; j < c + 3; j++) t &= colorMatrix[i, j] != -1;
					if (t)
					for (int r = g.Value / 3 * 3, i = r; i < r + 3; i++) for (int c = g.Value % 3 * 3, j = c; j < c + 3; j++) colorMatrix[i, j] = 1;
				}
			}
		}

		private void ApplyChanges()
		{
			for (int i = 0; i < 9; i++)
				for (int j = 0; j < 9; j++) matr[i, j].brush = colorMatrix[i, j] == 0 ? Brushes.White : colorMatrix[i, j] == 1 ? Brushes.Green : Brushes.Red;
		}

		private void ClearColors()
		{
			colorMatrix = new int[9, 9];
		}

		private void CheckDuplicates(int value, HashSet<int> valueSet, HashSet<KeyValuePair<string, int>> valuesToChange, int count, string name)
		{
			if (value  != 0)
			{
				if (valueSet.Contains(value))
				{
					valuesToChange.Add(new KeyValuePair<string, int>(name, count));
				}
				valueSet.Add(value);
			}
		}
	}
}
