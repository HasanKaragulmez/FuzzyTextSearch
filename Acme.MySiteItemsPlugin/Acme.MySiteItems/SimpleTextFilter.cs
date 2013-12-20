using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Acme.MySiteItems
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
            
            return Regex.Replace(term, "[^a-zA-Z]", string.Empty);
        }
    }
}
