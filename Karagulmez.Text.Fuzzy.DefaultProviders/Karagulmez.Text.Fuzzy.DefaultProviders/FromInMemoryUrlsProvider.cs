using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    class FromInMemoryUrlsProvider : Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider
    {
        private List<string> _fullUnfilteredUrlList = new List<string>();
        private List<string> _fullFilteredUrlList = new List<string>();
        private static IItemsProvider _instance;

        public static IItemsProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FromInMemoryUrlsProvider();
            }

            return _instance;
        }

        /// <summary>
        /// A filtered string (one where the IFilter has been applied)
        /// </summary>
        /// <param name="atIndex">Index to use</param>
        /// <returns>A string representing the unfiltered value, i.e. one whereby e.g. dashes, semicolons etc have been filtered away</returns>
        public string GetFilteredString(int atIndex)
        {
            if (_fullFilteredUrlList == null || atIndex > (_fullFilteredUrlList.Count() - 1))
            {
                throw new ArgumentException("No matching item in _fullFilteredUrlList!");
            }

            return _fullFilteredUrlList[atIndex];
        }

        /// <summary>
        /// Gets the actual display value that'll be communicated as the option to use.
        /// This gives you the option to use return unfiltered results here
        /// </summary>
        /// <param name="atIndex">Index to use</param>
        /// <returns>A string representing the display value. Generally this will be the unfiltered list value</returns>
        public string GetUnfilteredString(int atIndex)
        {
            if (_fullUnfilteredUrlList == null || atIndex > (_fullUnfilteredUrlList.Count() - 1))
            {
                throw new ArgumentException("No matching item in fullurl-list!");
            }

            return _fullUnfilteredUrlList[atIndex];
        }

        /// <summary>
        /// Load the data for this in-memory provider.
        /// Because we handle urls ourselves in this case, we know what's coming in is a List<string>
        /// </summary>
        /// <param name="data">A List<string> object containing </param>
        /// <returns>A filtered list of urls, this will be used for actual indexing and lookups</returns>
        public IEnumerable<string> Load(object data)
        {
            if(!(data is Dictionary<string, object>))
            {
                throw new ArgumentException("FromInMemoryUrlsProvider expected key-value pair");
            }

            LoadData(data);

            ILoadableFilter configurableFilter = ConfigurableStringFilter.GetInstance();
            _fullFilteredUrlList = new List<string>();
            foreach (string s in _fullUnfilteredUrlList)
            {
                _fullFilteredUrlList.Add(configurableFilter.Filter(s));
            }

            return _fullFilteredUrlList;
        }

        private void LoadData(object data)
        {
            Dictionary<string, object> initializationData = (Dictionary<string, object>)data;
            if (initializationData.ContainsKey("strings"))
            {
                //hard cast, because anything else is an error
                _fullUnfilteredUrlList = (List<string>)initializationData["strings"]; 
            }

            if (initializationData.ContainsKey("filter"))
            {
                //hard cast, because anything else is an error
                Dictionary<string, string> filterData = (Dictionary<string, string>)initializationData["filter"]; 

                ILoadableFilter configurableFilter = ConfigurableStringFilter.GetInstance();
                configurableFilter.Load(filterData);
            }
        }
    }
}
