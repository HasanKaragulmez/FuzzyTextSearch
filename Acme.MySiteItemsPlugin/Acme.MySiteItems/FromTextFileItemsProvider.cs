using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Acme.MySiteItems
{
    class FromTextFileItemsProvider : Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider
    {
        private const string _filename = @".\textfile.txt";    //could read this in via a setting, change to whatever you want
        private static IItemsProvider _instance;
        private List<string> _allItems = null;

        private FromTextFileItemsProvider()
        {
        }

        public static IItemsProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FromTextFileItemsProvider();
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

        public IEnumerable<string> Load()
        {
            _allItems = LoadItemsFromTextFile(_filename).ToList();
            return _allItems;
        }

        private IEnumerable<string> LoadItemsFromTextFile(string filename)
        {
            if (!File.Exists(filename))
            {
                throw new FileNotFoundException(string.Format("File {0} does not exist.", filename));
            }

            string allText = File.ReadAllText(filename);
            var splittedItems = allText.ToLowerInvariant().Split(' ');
            //filter our items at loading process, this will speed up the overall process as we won't need to filter constantly afterwards
            
            for(int i = 0; i != splittedItems.Count(); i++)
            {
                splittedItems[i] = SimpleTextFilter.GetInstance().Filter(splittedItems[i]);
            }

            return splittedItems.Distinct();
        }
    }
}
