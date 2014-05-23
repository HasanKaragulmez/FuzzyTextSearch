using System.Collections.Generic;

namespace Karagulmez.Text.Fuzzy.Interfaces
{
    public interface IFuzzyTextSearchManager
    {
        /// <summary>
        /// Initialize with the given assembly and class
        /// This class should inherit the Karagulmez.Text.Fuzzy.Interfaces.IConfigurator-interface
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="className">Class-name in the given Assembly, which implements the IConfiguration-interface</param>
        /// <param name="data">Optional parameter in which custom-data can be passed</param>
        void InitializeConfiguration(string assembly, string className, object data = null);

        /// <summary>
        /// Tries to find the most likely match by the given searchterm
        /// </summary>
        /// <param name="searchTerm">The term to search for</param>
        /// <param name="maxResults">The maximum number of results to return</param>
        /// <returns>An ICollection of keyvalue-pairs of given items, and the match percentage</returns>
        IEnumerable<KeyValuePair<string, double>> Search(string searchTerm, int maxResults);
    }
}
