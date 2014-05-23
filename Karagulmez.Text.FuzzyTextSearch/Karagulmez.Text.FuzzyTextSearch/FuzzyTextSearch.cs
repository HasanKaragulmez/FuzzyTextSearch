using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Karagulmez.Text.Fuzzy
{
    public class FuzzyTextSearch : IFuzzyTextSearchManager
    {
        private Karagulmez.Text.Fuzzy.Interfaces.IConfigurator _iConfigurator = null;
        private NGramManager _nGramManager = null;
        private IEnumerable<string> _loadedItems = null;
        private IFilter _termFilter = null;
        private IItemsProvider _itemsProvider = null;
        private int _loadedItemsCount = -1;
        private NGramManager.NGRAMS _nGram = NGramManager.NGRAMS.BIGRAM;
        private Dictionary<string, HashSet<int>> _nGramTermsIndex = null;

        /// <summary>
        /// Initialize with the given assembly and class
        /// This class should inherit the Karagulmez.Text.Fuzzy.Interfaces.IConfigurator-interface
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="classInit">Class-name in the given Assembly, which implements the IConfiguration-interface</param>
        public void InitializeConfiguration(string assembly, string className, object data = null)
        {
            _iConfigurator = (Karagulmez.Text.Fuzzy.Interfaces.IConfigurator)Activator.CreateInstance(assembly, className).Unwrap();

            Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider itemsProvider = _iConfigurator.ItemsProvider();
            _loadedItems = itemsProvider.Load(data);

            if (_loadedItems == null)
            {
                _loadedItems = new List<string>();  //initialize with a default
            }
            
            _loadedItemsCount = _loadedItems.Count();
            
            _nGramManager = new NGramManager();
            _nGram = NGramManager.NGramAsEnum(_iConfigurator.NGram);
            _termFilter = _iConfigurator.Filter();
            _itemsProvider = _iConfigurator.ItemsProvider();

            _nGramTermsIndex = _nGramManager.LoadData(_loadedItems, _nGram);
        }

        /// <summary>
        /// Tries to find the most likely match by the given searchterm
        /// </summary>
        /// <param name="searchTerm">The term to search for</param>
        /// <returns>An ICollection of keyvalue-pairs of given items, and the match percentage</returns>
        public IEnumerable<KeyValuePair<string, double>> Search(string searchTerm, int maxResults)
        {   
            if(string.IsNullOrEmpty(searchTerm) || searchTerm.Length < (int)_nGram)
            {
                return new List<KeyValuePair<string, double>>();
            }

            if (maxResults <= 0)
            {
                maxResults = 1;
            }
            
            IEnumerable<KeyValuePair<int, double>> idResults = NGramManager.FindMostLikelyTermIdsByMatchPercentage(
                                                                    searchTerm,
                                                                    _loadedItemsCount,
                                                                    _itemsProvider,
                                                                    _termFilter,
                                                                    _nGram,
                                                                    _nGramTermsIndex,
                                                                    _nGramManager.GetNGramsForIdList()
                                                                );

            List<KeyValuePair<string, double>> displayResults = new List<KeyValuePair<string, double>>();
            if (idResults == null || idResults.Count() == 0)
            {
                return displayResults;
            }

            //Convert to display values
            foreach (KeyValuePair<int, double> kvp in idResults)
            {
                displayResults.Add(new KeyValuePair<string, double>(_itemsProvider.GetUnfilteredString(kvp.Key), kvp.Value));
            }

            return displayResults.Take(maxResults);
        }
    }
}