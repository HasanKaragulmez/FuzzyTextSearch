using Karagulmez.Text.Fuzzy.Interfaces;
using System.Text.RegularExpressions;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    class SimpleTextFilter : IFilter
    {
        private static SimpleTextFilter _instance;

        public static SimpleTextFilter GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SimpleTextFilter();
            }

            return _instance;
        }

        public string Filter(string term)
        {
            if (string.IsNullOrEmpty(term))
                return term;

            //might want to do something with diacritics here...
            
         //   return Regex.Replace(term, "[a-zA-Z]", string.Empty);
            //greedy match for whole words, discards apostrophes
            return Regex.Match(term, "[a-zA-z]+", RegexOptions.CultureInvariant).ToString();
        }
    }
}