using Karagulmez.Text.Fuzzy.Interfaces;

namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    class FromInMemoryUrlsConfigurator : IConfigurator
    {
        public int NGram
        {
            get { return 2; }   //2 = bigrams, 3 = trigrams
        }

        public IItemsProvider ItemsProvider()
        {
            return FromInMemoryUrlsProvider.GetInstance();
        }

        public IFilter Filter()
        {
            return ConfigurableStringFilter.GetInstance();
        }
    }
}
