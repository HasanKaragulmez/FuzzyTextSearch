
namespace Karagulmez.Text.Fuzzy.Interfaces
{
    public interface IFilter
    {
        /// <summary>
        /// User-set function which allows you to filter for terms.
        /// For example, filtering out hyphens and numbers, if you don't want to match on those kind of symbols.
        /// Of course, it's fine to just return the input-string if you don't want to filter at all.
        /// </summary>
        /// <param name="term">The input-string that should be filtered</param>
        /// <returns>A filtered string</returns>
        string Filter(string term);
    }
}
