using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CI_SearchAlgoritms;

namespace CI_SearchAlgoritms
{
    class LocalSP_Sudoku : SearchProblem<FullSudokuState>
    {
        private UnsolvedSudoku sudoku;
        public int N { get { return sudoku.N; } }
        private int maxQualtiy;
        private Random r;
        private Swap[] randActions;
        private int stepCount;

        public LocalSP_Sudoku(UnsolvedSudoku sudoku)
        {
            this.sudoku = sudoku;
            maxQualtiy = 2 * sudoku.N * sudoku.N;
            r = new Random();
            stepCount = 0;
            randActions = GetPossibleSwaps(sudoku);
        }
        private Swap[] GetPossibleSwaps(UnsolvedSudoku sudoku)
        {
            List<Swap> swaps = new List<Swap>();
            byte N = (byte)sudoku.N;
            byte N2 = (byte)(N * N);
            
            //for each block
            for (byte blockCol = 0; blockCol < N; blockCol++)
                for (byte blockRow = 0; blockRow < N; blockRow++)
                {
                    //Collect free subLocs:
                    List<Tuple<byte, byte>> subLocs = new List<Tuple<byte, byte>>();
                    for (byte col = 0; col < N; col++)
                        for (byte row = 0; row < N; row++)
                        {
                            byte acutalCol = (byte)(blockCol * N + col);
                            byte acutalRow = (byte)(blockRow * N + row);
                            if (sudoku.field[acutalCol, acutalRow] == 0)
                                subLocs.Add(new Tuple<byte, byte>(acutalCol, acutalRow));
                        }

                    //Add all combinations to swaps:
                    for (int i = 0; i < subLocs.Count - 1; i++)
                        for (int j = i + 1; j < subLocs.Count; j++)
                        {
                            Tuple<byte, byte> loc1 = subLocs[i];
                            Tuple<byte, byte> loc2 = subLocs[j];
                            swaps.Add(new Swap(loc1.Item1, loc1.Item2, loc2.Item1, loc2.Item2));
                        }
                }

            return swaps.ToArray();
        }

        public FullSudokuState GetRandomStartState()
        {
            return new FullSudokuState(sudoku, r, Heuristic);
        }

        IEnumerable<FullSudokuState> SearchProblem<FullSudokuState>.Successors(FullSudokuState s)
        {
            foreach (Swap action in RadomOrderActions())
            {
                stepCount++;
                yield return action.DoSwap(s);
            }
        }
        public FullSudokuState GetRandomSuccessor(FullSudokuState s) 
        {
            return randActions[r.Next(randActions.Length)].DoSwap(s);
        }

        private IEnumerable<Swap> RadomOrderActions()
        {
            foreach (Swap s in randActions.RandomOrder(r))
                yield return s;
        }
        public int StepCount()
        {
            return stepCount;
        }

        public bool IsGoal(FullSudokuState solution)
        {
            return solution.quality == maxQualtiy;
        }

        public int Heuristic(FullSudokuState s)
        {
            int N = s.field.GetLength(0);
            int quality = Util.LineIndexesSum(N, (blockCol, col) => ColQuality(s.field, blockCol, col, N));
            quality += Util.LineIndexesSum(N, (blockRow, row) => RowQuality(s.field, blockRow, row, N));
            return quality;
        }

        public static int ColQuality(byte[,][,] field, byte blockCol, byte col, int N)
        {
            return LineQuality(N, (blockRow, row) => field[blockCol, blockRow][col, row]);
        }
        public static int RowQuality(byte[,][,] field, byte blockRow, byte row, int N)
        {
            return LineQuality(N, (blockCol, col) => field[blockCol, blockRow][col, row]);
        }

        public static int LineQuality(int N, Func<byte,byte,byte> fetcher)
        {
            bool[] seen = new bool[N * N];
            return Util.LineIndexesSum(N, (blockI, i) =>
            {
                byte num = fetcher(blockI, i);
                if (!seen[num])
                {
                    seen[num] = true;
                    return 1;
                }
                else return 0;
            });
        }

        private struct Swap
        {
            public byte row1, col1, row2, col2;

            public Swap(byte r1, byte c1, byte r2, byte c2)
            {
                row1 = r1; col1 = c1; row2 = r2; col2 = c2;
            }
            
            public FullSudokuState DoSwap(FullSudokuState s)
            {
                int N = s.field.GetLength(0);
                
                byte bc = (byte)(col1 / N);
                byte br = (byte)(row1 / N);
                byte r1mod = (byte)(row1 % N);
                byte r2mod = (byte)(row2 % N);
                byte c1mod = (byte)(col1 % N);
                byte c2mod = (byte)(col2 % N);

                int qualityBefore = RowQuality(s.field, br, r1mod, N);
                if (r2mod != r1mod) qualityBefore += RowQuality(s.field, br, r2mod, N);
                qualityBefore += ColQuality(s.field, bc, c1mod, N);
                if (c2mod != c1mod) qualityBefore += ColQuality(s.field, bc, c2mod, N);
                
                byte buff = 0;
                byte[,] block = null;
                byte[,] newBlock = new byte[N,N];
                byte[,][,] newField = new byte[N,N][,];

                s.field.CopyToExcept(newField, bc, br, 
                    b => {
                        block = b; 
                        return newBlock;
                    });
                block.CopyToExcept(newBlock, c1mod, r1mod, 
                    by => {
                        buff = by; 
                        return block[c2mod,r2mod];
                    });
                
                newField[bc, br][c2mod, r2mod] = buff;

                int qualityAfter = RowQuality(newField, br, r1mod, N);
                if (r2mod != r1mod) qualityAfter += RowQuality(newField, br, r2mod, N);
                qualityAfter += ColQuality(newField, bc, c1mod, N);
                if (c2mod != c1mod) qualityAfter += ColQuality(newField, bc, c2mod, N);

                int newQuality = s.quality + (qualityAfter - qualityBefore);

                return new FullSudokuState(newField,newQuality);
            }
        }
    }
    
    public class FullSudokuState
    {
        public byte[,][,] field;
        public int quality;

        public FullSudokuState(UnsolvedSudoku sudoku, Random r, Func<FullSudokuState,int> qualityFunction)
        {
            int N = sudoku.N;
            int N2 = N * N;
            int N3 = N2 * N;

            field = new byte[N, N][,];

            //for each block
            for (byte blockCol = 0; blockCol < N; blockCol++)
                for (byte blockRow = 0; blockRow < N; blockRow++)
                {
                    byte[,] block = new byte[N, N];
                    bool[] used = new bool[N2];

                    //Put given numbers
                    for (byte col = 0; col < N; col++)
                        for (byte row = 0; row < N; row++)
                        {
                            byte given = sudoku.field[blockCol*N + col, blockRow*N + row];
                            if (given != 0)
                            {
                                block[col, row] = given;
                                used[given] = true;
                            }
                            else block[col, row] = 0;
                        }
                    
                    //Make shuffled list of remaining numbers
                    byte[] nums = new byte[N2];
                    for (byte i = 1; i <= N2; i++) nums[i] = i;
                    List<byte> shuffledLeftNums = new List<byte>();
                    foreach (byte num in nums.RandomOrder(r))
                        if (!used[num])
                            shuffledLeftNums.Add(num);
                    
                    //Put the shuffeled numbers
                    int k = 0;
                    for (byte col = 0; col < N; col++)
                        for (byte row = 0; row < N; row++)
                            if (block[col, row] == 0)
                                block[col, row] = shuffledLeftNums[k++];

                    field[blockCol, blockRow] = block;
                }

            quality = qualityFunction(this);
        }

        internal FullSudokuState(byte[,][,] field, int quality)
        {
            this.quality = quality;
            this.field = field;
        }
    }
}
