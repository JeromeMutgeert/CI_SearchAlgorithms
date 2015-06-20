using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CI_SearchAlgoritms
{
    interface SearchProblem<State>
    {
        IEnumerable<State> Successors(State s);
        int Heuristic(State s);

        bool IsGoal(State solution);
    }
}
