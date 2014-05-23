
namespace Karagulmez.Text.Fuzzy.Interfaces
{
    public interface ILoadableFilter : IFilter
    {
        /// <summary>
        /// Allows you to add an arbitrary object containing the information for 
        /// filtering items
        /// </summary>
        /// <param name="data"></param>
        void Load(object data);
    }
}