using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    //We chose to implement the IItemsProvider here as something that parses a simple sitemap.xml file
    //If you'd rather load something from e.g. XML, CSV, appsettings, whatever - it's up to you, you're free to do so!
    //Note that this class-type need not be exposed, it's ok for it to be marked internal!!
    class FromSitemapXmlOnDiskItemsProvider : Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider
    {
        private List<string> _fullUnfilteredUrlList = new List<string>();
        private List<string> _fullFilteredUrlList = new List<string>();
        
        //public List<string> UrlList { get { return filteredUrlList; } }
        private const string _sitemapFile = @".\sitemap.xml";    //could read this in via a setting, change to whatever you want
        private static IItemsProvider _instance;

        private FromSitemapXmlOnDiskItemsProvider()
        {
        }
        
        /// <summary>
        /// Interface implementation, this is our signal to load our items
        /// </summary>
        /// <returns>A list containing all items on which matching should take place</returns>
        public IEnumerable<string> Load(object data)
        {
            _fullFilteredUrlList = (List<string>)ConvertInputDictionaryToList(_sitemapFile, _fullUnfilteredUrlList, UrlFilter.GetInstance());

            return _fullFilteredUrlList.AsEnumerable<string>();
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

        public static IItemsProvider GetInstance()
        {
            if (_instance == null)
            {
                _instance = new FromSitemapXmlOnDiskItemsProvider();
            }

            return _instance;
        }

        /// <summary>
        /// Parses the sitemap.xml file, and processes it into a filtered list (reference filteredUrlList)
        /// </summary>
        /// <param name="filename">The sitemap.xml file to load</param>
        /// <param name="filteredUrlList">A list-referencec which will contain all filtered results</param>
        /// <param name="urlFilter">the IFilter to apply for filtering urls</param>
        /// <returns>All (unfiltered) results</returns>
        private static IEnumerable<string> ConvertInputDictionaryToList(string filename, List<string> filteredUrlList, IFilter urlFilter)
        {
            if (!File.Exists(filename))
            {   
                throw new FileNotFoundException(string.Format("File {0} does not exist.", filename));                
            }

            //could use linq, I guess, using regex's here. Use whatever you like.
            string allText = File.ReadAllText(filename);
            var matches = Regex.Matches(allText, "<loc>(?<relurl>.*)</loc>");

            if (matches == null)
            {
                return null;
            }

            List<string> returnList = new List<string>(matches.Count);
            foreach (Match match in matches)
            {
                if (match != null && match.Groups.Count > 0)
                {
                    string value = match.Groups["relurl"].Value;
                    if (!string.IsNullOrEmpty(value))
                    {
                        filteredUrlList.Add(value);

                        value = urlFilter.Filter(value);

                        //if (!string.IsNullOrEmpty(value)) //cant skip items, because of our index!                        
                        returnList.Add(value);
                    }
                }
            }

            return returnList;
        }
    }
}
