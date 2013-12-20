using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karagulmez.Text.Fuzzy.Interfaces
{
    internal interface INGramManager
    {
        NGramManager.NGRAMS ConfiguredNGram { get; }
        Dictionary<string, HashSet<int>> LoadData(IEnumerable<string> data, NGramManager.NGRAMS nGram, IFilter filter);
    }
}
