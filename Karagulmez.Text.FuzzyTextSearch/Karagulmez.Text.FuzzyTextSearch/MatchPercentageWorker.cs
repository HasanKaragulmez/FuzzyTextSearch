using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using NGramsForId = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Collections.Generic.HashSet<string>, int>>;

namespace Karagulmez.Text.Fuzzy
{
    class MatchPercentageWorker
    {
        private ManualResetEvent _workerThreadDoneEvent;
        private int _startId = 0;

        private IEnumerable<int> _candidateTerms = null;
        private IItemsProvider _itemsProvider = null;

        private NGramManager.NGRAMS _nGramMapper;
        private List<string> _searchWordNgrams;
        private double _perElementPercentage;
        private HashSet<KeyValuePair<int, double>> _termMatchPercentage = new HashSet<KeyValuePair<int, double>>();
        private NGramsForId _nGramsForIds = null;

        // Constructor. 
        public MatchPercentageWorker(int startId, 
                                     IEnumerable<int> candidateTerms, NGramManager.NGRAMS nGramMapper,
                                     List<string> searchWordNgrams, double perElementPercentage,
                                     IItemsProvider itemsProvider, ManualResetEvent threadDoneEvent, NGramsForId nGramsForIds)
        {
            _startId = startId;
            _candidateTerms = candidateTerms;
            _itemsProvider = itemsProvider;
            _nGramMapper = nGramMapper;
            _searchWordNgrams = searchWordNgrams;
            _perElementPercentage = perElementPercentage;
            _workerThreadDoneEvent = threadDoneEvent;
            _nGramsForIds = nGramsForIds;
        }

        public HashSet<KeyValuePair<int, double>> TermMatchPercentage { get { return _termMatchPercentage; } }

        /// <summary>
        /// Calculates the matching percentages for the specified parameters
        /// </summary>
        /// <param name="threadContext">An object containing the threadcontext</param>
        public void ThreadPoolCallback(Object threadContext)
        {
            foreach (int termId in _candidateTerms)
            {
               // string nGramFriendly = _itemsProvider.GetFilteredString(termId);

                int wordNgramsCount = 0;
               // HashSet<string> word_ngrams = NGramManager.GetNGramsAsHashSet(nGramFriendly, _nGramMapper, out wordNgramsCount);

                var idData = _nGramsForIds[termId];
                wordNgramsCount = idData.Value;
                HashSet<string> word_ngrams = idData.Key;

                if (word_ngrams == null || wordNgramsCount == 0)
                    continue;

                //calculate matches

                //calculate how much actually matches                
                double matchCount = _searchWordNgrams.Where(word_ngrams.Contains).Count();

                //but we should still determine how many misses have occured for an accurate percentage
                double totalNGramsWord = wordNgramsCount;
                double thisWordMatchPercentage = matchCount / totalNGramsWord;

                matchCount = matchCount * thisWordMatchPercentage;

                double matchPercentage = matchCount * _perElementPercentage;

                if (matchPercentage > 0)
                {
                    _termMatchPercentage.Add(new KeyValuePair<int, double>(termId, matchPercentage));
                }
            }

            _workerThreadDoneEvent.Set();   //signal that we're done
        }
    }
}
