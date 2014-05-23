using Karagulmez.Text.Fuzzy.Interfaces;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    class FromInMemoryTextItemsConfigurator : IConfigurator
    {
        public int NGram
        {
            get { return 2; }
        }

        public IItemsProvider ItemsProvider()
        {
            return FromInMemoryTextItemsProvider.GetInstance();
        }

        public IFilter Filter()
        {
            return SimpleTextFilter.GetInstance();
        }
    }
}