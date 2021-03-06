﻿
namespace Karagulmez.Text.Fuzzy.DefaultProviders
{
    //Configurators are exposed publicly, so they should be marked public
    public class FromTextFileConfigurator : Karagulmez.Text.Fuzzy.Interfaces.IConfigurator
    {
        public int NGram
        {
            get { return 2; }
        }

        public Karagulmez.Text.Fuzzy.Interfaces.IFilter Filter()
        {
            return SimpleTextFilter.GetInstance();
        }

        public Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider ItemsProvider()
        {
            return FromTextFileItemsProvider.GetInstance();
        }
    }
}
