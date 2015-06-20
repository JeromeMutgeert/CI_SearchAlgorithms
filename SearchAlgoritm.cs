using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CI_SearchAlgoritms
{
    interface SearchAlgorithm<State>
    {
        bool Printing { set; get; }
        bool Solve(SearchProblem<State> problem);
    }
}
