using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CI_SearchAlgoritms
{
    class Program
    {
        static void Main(string[] args)
        {
            //Norvig n = new Norvig(5);
            //n.Test();
            Program p = new Program();
        }

        public Program()
        {
            Norvig n = new Norvig(3);
            foreach (UnsolvedSudoku sudoku in LoadSudokus())
            {
                n.Pringting
                bool solved = n.Solve(new NorvigSudoku(sudoku));
            }
        }

        private IEnumerable<UnsolvedSudoku> LoadSudokus()
        {
            StreamReader sr = new StreamReader("sudokus.txt");
            string[] sudokuStrings = sr.ReadToEnd().Split('\n');
            sr.Close();
            foreach (string sudoku in sudokuStrings)
            {
                yield return new UnsolvedSudoku(3, UnsolvedSudoku.AddSpaces(sudoku));
            }
        }
    }
    
    public static class Util
    {
        public static int LineIndexesSum(int N, Func<byte, byte, int> f)
        {
            int sum = 0;
            for (byte blockI = 0; blockI < N; blockI++)
                for (byte i = 0; i < N; i++)
                    sum += f(blockI, i);
            return sum;
        }

        public static void CopyToExcept<T>(this T[,] from, T[,] to, byte cEx, byte rEx, Func<T, T> F)
        {
            int N = from.GetLength(0);
            for (byte col = 0; col < N; col++)
                if (col == cEx)
                    for (byte row = 0; row < N; row++)
                        if (row == rEx)
                            to[col, row] = F(from[col, row]);
                        else
                            to[col, row] = from[col, row];
                else
                    for (byte row = 0; row < N; row++)
                        to[col, row] = from[col, row];
        }

        public static IEnumerable<T> RandomOrder<T>(this T[] arr, Random r)
        {
            for (int i = arr.Length, j = 0; i >= 0; i--, j++)
            {
                int k = j + r.Next(i);
                T result = arr[k];
                arr[k] = arr[j];
                arr[j] = result;
                yield return result;
            }
        }
    }
}
