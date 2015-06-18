using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CI_SearchAlgoritms
{
    interface SearchAlgorithm<State>
    {
        Dictionary<string,string> Solve(SearchProblem<State> problem);
    }
}
