using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CI_SearchAlgoritms
{
    class ILS : SearchAlgorithm<FullSudokuState>
    {
        int randSteps, totalSteps;

        public ILS(int totalSteps, int randSteps)
        {
            this.randSteps = randSteps;
            this.totalSteps = totalSteps;
        }
        Dictionary<string,string> SearchAlgorithm<FullSudokuState>.Solve(SearchProblem<FullSudokuState> problem)
        {
            LocalSP_Sudoku p = (LocalSP_Sudoku)problem;
            Stopwatch st = new Stopwatch();
            st.Start();

            FullSudokuState solution = DoILS((LocalSP_Sudoku)problem);

            st.Stop();
            Dictionary<string, string> info = new Dictionary<string, string>();
            info["algorithm"] = "ILS, N = " + p.N;
            info["solved"] = p.IsGoal(solution) ? "true" : "false";
            info["time taken"] = st.ElapsedMilliseconds.ToString() + " ms";
            info["steps"] = p.StepCount().ToString();

            return info;

        }
        public FullSudokuState DoILS(LocalSP_Sudoku problem)
        {
            FullSudokuState sol = RRHillClimb.Climb(problem, problem.GetRandomStartState());
            if (problem.IsGoal(sol)) return sol;
            FullSudokuState sol2 = RandomWalk(problem, sol);

            while (problem.StepCount() < totalSteps || problem.IsGoal(sol2))
            {
                if (sol2.quality > sol.quality) sol = sol2;
                sol2 = RandomWalk(problem, sol);
                sol2 = RRHillClimb.Climb(problem, sol2);
            } 

            if (sol2.quality > sol.quality) sol = sol2;
            return sol;
        }

        private FullSudokuState RandomWalk(LocalSP_Sudoku problem, FullSudokuState sFrom)
        {
            throw new NotImplementedException();
        }
    }
}
