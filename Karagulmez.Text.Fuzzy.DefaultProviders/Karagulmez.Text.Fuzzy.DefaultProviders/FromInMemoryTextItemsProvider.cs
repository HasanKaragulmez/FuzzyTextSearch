using Karagulmez.Text.Fuzzy.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    class FromInMemoryTextItemsProvider : Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider
    {
        private static IItemsProvider _instance;
        private List<string> _allItems = null;

        public static IItemsProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FromInMemoryTextItemsProvider();
            }

            return _instance;
        }

        public string GetUnfilteredString(int atIndex)
        {
            //we expect to be initalized here
            //For the textfilter, we present the filtered result as the unencoded-string.
            //Note that filtering has already taken place at the loading process, but we could have done it here as well, at a higher cost
            return _allItems[atIndex];
        }

        public string GetFilteredString(int atIndex)
        {
            return _allItems[atIndex];  //we don't make a distinction here between filtered and unfiltered results, so address the same list
        }

        public IEnumerable<string> Load(object data)
        {
            _allItems = LoadItemsFromText(data as string).ToList<string>();

            return _allItems;
        }

        private IEnumerable<string> LoadItemsFromText(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return new List<string>();
            }

            //split and distinct in advance, to lower the amount of duplicate items as much as possible
            var splittedItems = text.ToLowerInvariant().Split(' ').Distinct().ToList();
            var splitItemsCount = splittedItems.Count();
            
            //filter our items at the loading-stage, this will speed up the overall process as we won't need to filter constantly afterwards
            for (int i = 0; i != splitItemsCount; i++)
            {
                splittedItems[i] = SimpleTextFilter.GetInstance().Filter(splittedItems[i]);
            }

            //because we've filtered, items may be duplicate as well - so distinct again
            return splittedItems.Distinct();
        }
    }
}