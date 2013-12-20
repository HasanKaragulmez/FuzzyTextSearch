using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karagulmez.Text.Fuzzy.Interfaces
{
    public interface IConfigurator
    {   
        /// <summary>
        /// Which ngram to support, supported are 2 (bigrams) and 3 (trigrams)
        /// </summary>
        int NGram { get; }

        /// <summary>
        /// A provider which is capable of returning all the items. 
        /// In essence, this is the dictionary on which matching will take place
        /// </summary>
        /// <returns>An instance of IItemsProvider</returns>
        IItemsProvider ItemsProvider();

        /// <summary>
        /// An user-created IFilter implementation, which is used to filter search terms
        /// This allows you to filter parts which are not necessary/wanted for matching
        /// (e.g. numbers, hyphens, etc).
        /// </summary>
        /// <returns>An instance of IFilter</returns>
        IFilter Filter();
    }
}
