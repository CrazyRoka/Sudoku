using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
	class SudokuGenerator
	{
		private int[,] matr;
		HashSet<int>[] rows, cols, boxes;
		Random rand;
		public SudokuGenerator()
		{
			InitComponents();
			Generate(0, 0);
		}

		public int[,] Generate()
		{
			return matr;
		}

		private bool Generate(int x, int y)
		{
			if(y == 9){
				y = 0;
				x++;
			}
			if (x == 9) return true;
			HashSet<int> used = new HashSet<int>();
			while (used.Count < 9)
			{
				int value = rand.Next(1, 10);
				if (used.Contains(value)) continue;
				used.Add(value);
				if (rows[x].Contains(value) || cols[y].Contains(value) || boxes[x / 3 * 3 + y / 3].Contains(value)) continue;
				matr[x, y] = value;
				rows[x].Add(value);
				cols[y].Add(value);
				boxes[x / 3 * 3 + y / 3].Add(value);
				if (Generate(x, y + 1)) return true;
				matr[x, y] = 0;
				rows[x].Remove(value);
				cols[y].Remove(value);
				boxes[x / 3 * 3 + y / 3].Remove(value);
			}
			return false;
		}

		private void InitComponents()
		{
			rand = new Random();
			matr = new int[9, 9];
			rows = new HashSet<int>[9];
			cols = new HashSet<int>[9];
			boxes = new HashSet<int>[9];
			for (int i = 0; i < 9; i++)
			{
				rows[i] = new HashSet<int>();
				cols[i] = new HashSet<int>();
				boxes[i] = new HashSet<int>();
			}
		}
	}
}
