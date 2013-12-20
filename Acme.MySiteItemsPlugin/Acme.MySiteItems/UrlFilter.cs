using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acme.MySiteItems
{
    class UrlFilter : IFilter
    {
        private static readonly Dictionary<string, string> _urlTranslationDictionary = null;
        private static UrlFilter _instance;

        static UrlFilter()
        {
            _urlTranslationDictionary = InitTranslationDictionary();
        }

        public static UrlFilter GetInstance()
        {
            if(_instance == null)
            {
                _instance = new UrlFilter();
            }

            return _instance;
        }

        private static Dictionary<string, string> InitTranslationDictionary()
        {
            //todo: make external and configurable
            Dictionary<string, string> urlTranslationDictionary = new Dictionary<string, string>();
            //filter out hostname (else it would be found on every url)            
            urlTranslationDictionary.Add("http://www.wired.co.uk", string.Empty);
            //common misspelling
            urlTranslationDictionary.Add("definately", "definitely");
            //url parts which aren't interesting (i.e. aren't keywords, or shouldn't be in the sitemap in the first place            
            urlTranslationDictionary.Add("/404", string.Empty);
            urlTranslationDictionary.Add("/500", string.Empty);
            urlTranslationDictionary.Add("/sitemap.xml", string.Empty);
            urlTranslationDictionary.Add("/robots.txt", string.Empty);
            urlTranslationDictionary.Add("/news/archive", string.Empty);   //occurs in almost every url -> doesn't help distinguish items
            urlTranslationDictionary.Add("/magazine/archive", string.Empty);
            
            //filter url-characters which should be flattened or removed
            urlTranslationDictionary.Add("/", string.Empty);
            urlTranslationDictionary.Add("-", string.Empty);
            urlTranslationDictionary.Add("é", "e"); //flatten diacritics perhaps
            urlTranslationDictionary.Add(",", string.Empty);
            urlTranslationDictionary.Add(" ", string.Empty);

            //or: just run a regex[a-zA-Z] instead -> whatever you want :)

            return urlTranslationDictionary;
        }

        public string Filter(string term)
        {

            if (string.IsNullOrEmpty(term) || string.IsNullOrWhiteSpace(term))
            {
                return term;
            }

            foreach (KeyValuePair<string, string> kvp in _urlTranslationDictionary)
            {
                term = term.ToLowerInvariant().Replace(kvp.Key, kvp.Value);
            }

            return term;
        }
    }
}
