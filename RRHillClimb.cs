using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CI_SearchAlgoritms
{
    class RRHillClimb : SearchAlgorithm<FullSudokuState>
    {
        int totalSteps;
        RRHillClimb(int totalSteps)
        {
            this.totalSteps = totalSteps;
        }
        
        Dictionary<string,string> SearchAlgorithm<FullSudokuState>.Solve(SearchProblem<FullSudokuState> problem)
        {
            LocalSP_Sudoku p = (LocalSP_Sudoku)problem;
            Stopwatch st = new Stopwatch();
            st.Start();

            FullSudokuState solution = DoRRHillClimb((LocalSP_Sudoku)problem);

            st.Stop();
            Dictionary<string, string> info = new Dictionary<string, string>();
            info["algorithm"] = "Random-Restart Hillclimb, N = " + p.N;
            info["solved"] = p.IsGoal(solution) ? "true" : "false";
            info["time taken"] = st.ElapsedMilliseconds.ToString() + " ms";
            info["steps"] = p.StepCount().ToString();

            return info;
        }

        private FullSudokuState DoRRHillClimb(LocalSP_Sudoku problem)
        {
            FullSudokuState best = new FullSudokuState(null, 0);
            
            while (problem.StepCount() < totalSteps)
            {
                FullSudokuState result = Climb(problem, problem.GetRandomStartState());
                if (result.quality > best.quality) best = result;
            }

            return best;
        }

        public static FullSudokuState Climb(LocalSP_Sudoku problem, FullSudokuState start)
        {
            FullSudokuState state = start;
            bool betterOption = true;
            while (betterOption)
            {
                betterOption = false;
                foreach (FullSudokuState newS in ((SearchProblem<FullSudokuState>)problem).Successors(state))
                {
                    if (newS.quality > state.quality)
                    {
                        state = newS;
                        betterOption = true;
                        break;
                    }
                }
            }
            return state;
        }
    }
}
