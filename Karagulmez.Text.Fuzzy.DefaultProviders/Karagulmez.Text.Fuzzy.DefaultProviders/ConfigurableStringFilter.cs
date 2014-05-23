using Karagulmez.Text.Fuzzy.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    /// <summary>
    /// This filter allows data to be set from outside this class, thus ConfigurableStringFilter
    /// as an ILoadableFilter (which contains a mandatory method Load).
    /// Other filters contain their logic. Either way is fine, depending on what you want to do.
    /// </summary>
    class ConfigurableStringFilter : ILoadableFilter
    {
        private Dictionary<string, string> _translationDictionary;
        private static ConfigurableStringFilter _instance;

        public static ConfigurableStringFilter GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ConfigurableStringFilter();
            }

            return _instance;
        }

        public void Load(object data)
        {
            //We expect a dictionary of string containing match/replaces, 
            //so a hard-cast is in order to fail as early as possible
            _translationDictionary = (Dictionary<string, string>)data;
        }

        public string Filter(string term)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrWhiteSpace(term))
            {
                return term;
            }

            foreach (KeyValuePair<string, string> kvp in _translationDictionary)
            {
                string termReplacement = Regex.Replace(term, kvp.Key, kvp.Value);
                term = term.ToLowerInvariant().Replace(term, termReplacement);
            }

            return term;
        }
    }
}