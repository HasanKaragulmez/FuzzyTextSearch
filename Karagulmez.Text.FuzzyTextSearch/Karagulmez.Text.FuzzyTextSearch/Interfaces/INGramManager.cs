using System.Collections.Generic;

namespace Karagulmez.Text.Fuzzy.Interfaces
{
    internal interface INGramManager
    {
        NGramManager.NGRAMS ConfiguredNGram { get; }
        Dictionary<string, HashSet<int>> LoadData(IEnumerable<string> data, NGramManager.NGRAMS nGram);
    }
}
