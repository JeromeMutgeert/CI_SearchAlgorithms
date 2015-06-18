using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CI_SearchAlgoritms
{
    public class UnsolvedSudoku
    {
        public byte[,] field;
        public byte N;
        
        public UnsolvedSudoku(byte N, string fieldString)
        {
            this.N = N;
            int N2 = ((int)N)*N;

            string[] tiles = fieldString.Split(' ');
            field = new byte[N,N];

            for (byte col = 0; col < N; col++)
                for (byte row = 0; row < N; row++)
                {
                    int i = row*N2 + col;
                    byte res;
                    if (!byte.TryParse(tiles[i], out res)) res = 0;
                            
                    field[col, row] = res;
                }
                
        }
    }
}
