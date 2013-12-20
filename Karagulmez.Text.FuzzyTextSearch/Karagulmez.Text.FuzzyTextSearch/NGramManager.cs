using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//shorthand-types
using PartitionData = System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<int>>;
using NGramsForId = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<System.Collections.Generic.HashSet<string>, int>>;

namespace Karagulmez.Text.Fuzzy
{
    internal class NGramManager : INGramManager
    {
        /// <summary>
        /// Supported ngrams-types
        /// </summary>
        public enum NGRAMS
        {
            BIGRAM = (int)2,
            TRIGRAM = (int)3
        }

        //Our default ngram type
        private NGRAMS _nGram = NGRAMS.BIGRAM;
        private PartitionData _partitionData = null;
        private NGramsForId _nGramsForId = null;

        /// <summary>
        /// Retrieves which kind of ngram we're currently using
        /// </summary>
        public NGRAMS ConfiguredNGram { get { return _nGram; } }

        /// <summary>
        /// Load data into the ngram-manager
        /// </summary>
        /// <param name="data"></param>        
        /// <returns></returns>
        public PartitionData LoadData(IEnumerable<string> data, NGRAMS nGram, IFilter filter)
        {
            _nGram = nGram;
            _nGramsForId = new NGramsForId();
            foreach(var s in data)
            {   
                int ngramsCount = 0;
                var nGrams = GetNGramsAsHashSet(s, nGram, out ngramsCount);
                if (nGrams == null)
                {
                    nGrams = new HashSet<string>();
                }
                
                _nGramsForId.Add(new KeyValuePair<HashSet<string>, int>(nGrams, ngramsCount));
            }

            _partitionData = PartitionNGramsToDictionary(data, this._nGram);
            return _partitionData;
        }

        public NGramsForId GetNGramsForIdList()
        {
            return _nGramsForId;
        }

        private static PartitionData PartitionNGramsToDictionary(IEnumerable<string> nGramReadyWords, NGRAMS nGram)
        {
            if (nGramReadyWords == null || nGramReadyWords.Count() == 0)
            {
                return null;
            }

            PartitionData returnDictionary = new PartitionData();

            int arrayLength = nGramReadyWords.Count();
            for (int i = 0; i != arrayLength; i++)
            {
                string word = nGramReadyWords.ElementAt(i);
                
                int ngrammsCount = 0;
                HashSet<string> ngrams = GetNGramsAsHashSet(word, nGram, out ngrammsCount);
                if (ngrams == null || ngrams.Count == 0)
                {
                    continue;
                }

                foreach (var ngram in ngrams)
                {
                    if (string.IsNullOrEmpty(ngram))
                    {
                        continue;
                    }

                    if (!returnDictionary.ContainsKey(ngram))   //ngram does not yet exist in dictionary, add it.
                    {
                        returnDictionary.Add(ngram, new HashSet<int>());
                    }

                    returnDictionary[ngram].Add(i); //for this ngram, word i applies, so add it to the list                    
                }
            }

            return returnDictionary;
        }

        /// <summary>
        /// Gets the ngrams for the given word (as we do correcting per word)
        /// </summary>
        /// <param name="word">the word to n-gram</param>
        /// <param name="n">2 for bigrams, 3 for trigrams</param>
        /// <returns>a list of all n-grams for this word</returns>
        public static List<string> GetNGrams(string word, NGramManager.NGRAMS nGramToUse)
        {
            int n = (int)nGramToUse;

            if (string.IsNullOrEmpty(word) || word.Length < n)
                return null;

            int spacer = word.Length % n;
            if (spacer > 0)
            {
                word += "     ".Substring(spacer);
            }

            int wordLength = word.Length;
            int partitions = wordLength == n ? 1 : wordLength - n;

            List<string> nGrams = new List<string>(wordLength / n);
            
            string[] nGramsTest = new string[partitions];
            for (int i = 0; i != partitions; i++)
            {
                nGramsTest[i] = word.Substring(i, n);
            }

            nGrams.AddRange(nGramsTest);
            return nGrams;
        }

        /// <summary>
        /// Gets the ngrams for the given word (as we do correcting per word)
        /// </summary>
        /// <param name="word">the word to n-gram</param>
        /// <param name="n">2 for bigrams, 3 for trigrams</param>
        /// <param name="wordNgramsCount">The actual partitions, because the HashSet will not allow for duplicates</param>
        /// <returns>a list of all n-grams for this word</returns>
        internal static HashSet<string> GetNGramsAsHashSet(string word, NGramManager.NGRAMS nGramToUse, out int wordNgramsCount)
        {
            int n = (int)nGramToUse;
            wordNgramsCount = 0;
            if (word == null || word.Length < n)
            {
                return null;
            }

            int spacer = word.Length % n;
            if (spacer > 0)
            {
                word += "     ".Substring(spacer);
            }

            int wordLength = word.Length;
            int partitions = wordLength == n ? 1 : wordLength - n;
            var nGramhashSet = new string[partitions];

            for (int i = 0; i != partitions; i++)
            {
                nGramhashSet[i] = word.Substring(i, n);
            }

            wordNgramsCount = partitions;

            //return nGramhashSet;
            return new HashSet<string>(nGramhashSet);
        }


        public static NGRAMS NGramAsEnum(int value)
        {
            if (Enum.IsDefined(typeof(NGRAMS), value))
            {
                return (NGRAMS)value;
            }

            return NGRAMS.BIGRAM;
        }

        public static IEnumerable<KeyValuePair<int, double>> FindMostLikelyTermIdsByMatchPercentage(string searchTerm,
                                                                    int maxIdCount,
                                                                    IItemsProvider itemsProvider,
                                                                    IFilter termFilter,
                                                                    NGramManager.NGRAMS nGramMapper,
                                                                    Dictionary<string, HashSet<int>> nGramTermsIndex,
                                                                    NGramsForId nGramsForIds)
        {
            string searchTermNGramFriendly = termFilter.Filter(searchTerm);

            List<string> searchWordNgrams = GetNGrams(searchTermNGramFriendly, nGramMapper);

            var candidateTerms = new HashSet<int>();
            //pre-optimize -> don't take into account any words we know won't match
            foreach (var ngram in searchWordNgrams)
            {
                //make sure that an indexed ngram exists
                if (nGramTermsIndex.ContainsKey(ngram))
                {
                    //add the matching terms for this ngram
                    candidateTerms.UnionWith(nGramTermsIndex[ngram]);
                }
            }

            var termMatchPercentage = new HashSet<KeyValuePair<int, double>>();
            double perElementPercentage = 100.0 / searchWordNgrams.Count;

            //determine match-percentage for each candidate
            //todo: multithread this, if there are more than 32 candidateterms
            //1- get number of procs
            //2- set minimum amount of threadpool-threads
            //3- divide amount of candidateterms over availablethreads
            //   perThreadItems = candidateterms / amountofprocs
            // threadStart = 0
            //  createthreads (threadStart
            //   threadStart += perThreadItems

            int candidateTermsCount = candidateTerms.Count();
            if (candidateTermsCount > 32)
            {
                int createThreadCount = Environment.ProcessorCount;
                int minimumWorkerThreads, minimumIOCompletionPorts;
                ThreadPool.GetMinThreads(out minimumWorkerThreads, out minimumIOCompletionPorts); // Get the current settings.

                if (minimumWorkerThreads < createThreadCount + 1)
                {
                    //instantly set the min number of threads for the threadpool, rather than 1 every 500ms
                    //but keep the setting for the minimumIOCompletionPorts
                    ThreadPool.SetMinThreads(createThreadCount + 1, minimumIOCompletionPorts);
                }

                ManualResetEvent[] workerThreadDoneEvents = new ManualResetEvent[createThreadCount];
                MatchPercentageWorker[] matchPercentageWorker = new MatchPercentageWorker[createThreadCount];

                int threadCountStep = candidateTermsCount / createThreadCount;
                for (int i = 0; i != createThreadCount; i++)
                {
                    int startId = i * threadCountStep;
                    
                    var candidateTermsSubset = candidateTerms.Skip(startId).Take(threadCountStep);

                    workerThreadDoneEvents[i] = new ManualResetEvent(false);

                    MatchPercentageWorker f = new MatchPercentageWorker(startId,
                                                                        candidateTermsSubset,
                                                                        nGramMapper,
                                                                        searchWordNgrams,
                                                                        perElementPercentage,
                                                                        itemsProvider,
                                                                        workerThreadDoneEvents[i],
                                                                        nGramsForIds);
                    matchPercentageWorker[i] = f;
                    ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
                }

                WaitHandle.WaitAll(workerThreadDoneEvents);

                termMatchPercentage = matchPercentageWorker[0].TermMatchPercentage;
                for (int i = 1; i != matchPercentageWorker.Length; i++)
                {
                    termMatchPercentage.UnionWith(matchPercentageWorker[i].TermMatchPercentage);
                }
            }
            else
            {
                foreach (int termId in candidateTerms)
                {
                    //string nGramFriendly = itemsProvider.GetFilteredString(termId);                    

                    int wordNgramsCount = 0;
                    //HashSet<string> word_ngrams = GetNGramsAsHashSet(nGramFriendly, nGramMapper, out wordNgramsCount);

                    var idData = nGramsForIds[termId];
                    wordNgramsCount = idData.Value;
                    HashSet<string> word_ngrams = idData.Key;

                    if (word_ngrams == null || wordNgramsCount == 0)
                        continue;

                    //calculate matches

                    //calcuate how much actually matches
                    double matchCount = searchWordNgrams.Where(word_ngrams.Contains).Count();

                    //but we should still determine how many misses have occured for an accurate percentage
                    double totalNGramsWord = wordNgramsCount;
                    double thisWordMatchPercentage = matchCount / totalNGramsWord;

                    matchCount = (matchCount * thisWordMatchPercentage);

                    double matchPercentage = matchCount * perElementPercentage;

                    if (matchPercentage > 0)
                    {
                        termMatchPercentage.Add(new KeyValuePair<int, double>(termId, matchPercentage));
                    }
                }
            }

            return termMatchPercentage.OrderBy(x => -x.Value);
        }
    }
}