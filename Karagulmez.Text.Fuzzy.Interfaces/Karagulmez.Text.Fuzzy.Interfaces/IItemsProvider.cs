using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Karagulmez.Text.Fuzzy.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IItemsProvider
    {
        /// <summary>
        /// Signals the providing code to load content, and return a list of strings on which fuzzy text search should take place
        /// </summary>
        /// <returns>A list of strings which are the fuzzy text search candidates</returns>
        IEnumerable<string> Load();

        /// <summary>
        /// Allows to return the unencoded string to be returned. E.g. Load can return filtered results, but not display values
        /// </summary>
        /// <param name="atIndex">The index whose value should be returned</param>
        /// <returns>A string containing the display value. This will be in the actual return values</returns>
        string GetUnfilteredString(int atIndex);

        /// <summary>
        /// Allows to return the filtered string to be returned.
        /// This allows you to do filtering only once, at the loading-stage, rather than at runtime
        /// </summary>
        /// <param name="atIndex">The index whose value should be returned</param>
        /// <returns>A string containing the filtered value. This will be used internally for approximate string-matching</returns>
        string GetFilteredString(int atIndex);
    }
}