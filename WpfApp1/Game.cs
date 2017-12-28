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
		private const string WIN_CONGRATS = "Congratulations!";
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
			numberOfVals = DeleteBlocks(numberOfVals);

		}

		private int DeleteBlocks(int numberOfVals)
		{
			Random rand = new Random();
			while (numberOfVals != 0)
			{
				int x = rand.Next(0, 9);
				int y = rand.Next(0, 9);
				if (startValues[x, y] == 0) continue;
				startValues[x, y] = 0;
				numberOfVals--;
			}

			return numberOfVals;
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
			HashSet<int> valueSet1, valueSet2;
			HashSet<int>[,] valueSet3;
			HashSet<KeyValuePair<string, int>> badValues, goodValues;
			InitValues(out valueSet1, out valueSet2, out valueSet3, out badValues, out goodValues);
			FindMistakes(valueSet1, valueSet2, valueSet3, badValues, goodValues);
			BuildColorMatrix(badValues, goodValues);
			ApplyChanges();
			CheckWin(goodValues.Count);
		}

		private void FindMistakes(HashSet<int> valueSet1, HashSet<int> valueSet2, HashSet<int>[,] valueSet3, HashSet<KeyValuePair<string, int>> badValues, HashSet<KeyValuePair<string, int>> goodValues)
		{
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
		}

		private static void InitValues(out HashSet<int> valueSet1, out HashSet<int> valueSet2, out HashSet<int>[,] valueSet3, out HashSet<KeyValuePair<string, int>> badValues, out HashSet<KeyValuePair<string, int>> goodValues)
		{
			valueSet1 = new HashSet<int>();
			valueSet2 = new HashSet<int>();
			valueSet3 = new HashSet<int>[3, 3];
			badValues = new HashSet<KeyValuePair<String, int>>();
			goodValues = new HashSet<KeyValuePair<String, int>>();
			for (int i = 0; i < 3; i++) for (int j = 0; j < 3; j++) valueSet3[i, j] = new HashSet<int>();
		}

		private void CheckWin(int count)
		{
			if (count == 27)
			{
				LockBlocks();
				MessageBox.Show(WIN_CONGRATS);
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
			ColorRed(badValues);
			ColorGreen(goodValues);
			ColorFullNumbers();
		}

		private void ColorFullNumbers()
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			for (int i = 0; i < 9; i++)
				for (int j = 0; j < 9; j++) if (colorMatrix[i, j] != -1 && matr[i, j].value != 0)
					{
						if (!dictionary.ContainsKey(matr[i, j].value))
						{
							dictionary[matr[i, j].value] = 0;
						}
						dictionary[matr[i, j].value]++;
					}

			foreach (var g in dictionary)
			{
				if (g.Value != 9) return;
				for (int i = 0; i < 9; i++)
					for (int j = 0; j < 9; j++) if (g.Key == matr[i, j].value) colorMatrix[i, j] = 1;
			}
		}

		private void ColorGreen(HashSet<KeyValuePair<string, int>> goodValues)
		{
			foreach (var g in goodValues)
			{
				if (g.Key == "Row" && CheckRed(g.Value, g.Value, 0, 8))
					for (int j = 0; j < 9; j++) colorMatrix[g.Value, j] = 1;
				if (g.Key == "Column" && CheckRed(0, 8, g.Value, g.Value))
					for (int i = 0; i < 9; i++) colorMatrix[i, g.Value] = 1;
				if (g.Key == "Box" && CheckRed(g.Value / 3 * 3, g.Value / 3 * 3 + 2, g.Value % 3 * 3, g.Value % 3 * 3 + 2)) 
					for (int r = g.Value / 3 * 3, i = r; i < r + 3; i++) for (int c = g.Value % 3 * 3, j = c; j < c + 3; j++) colorMatrix[i, j] = 1;
			}
		}

		private bool CheckRed(int iFrom, int iTo, int jFrom, int jTo)
		{
			bool t = true;
			for (int i = iFrom; i <= iTo; i++)
				for (int j = jFrom; j < jTo; j++)
					t &= colorMatrix[i, j] != -1;
			return t;
		}

		private void ColorRed(HashSet<KeyValuePair<string, int>> badValues)
		{
			foreach (var g in badValues)
			{
				if (g.Key == "Row") for (int j = 0; j < 9; j++) colorMatrix[g.Value, j] = -1;
				if (g.Key == "Column") for (int i = 0; i < 9; i++) colorMatrix[i, g.Value] = -1;
				if (g.Key == "Box") for (int r = g.Value / 3 * 3, i = r; i < r + 3; i++) for (int c = g.Value % 3 * 3, j = c; j < c + 3; j++) colorMatrix[i, j] = -1;
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
