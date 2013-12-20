using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Acme.MySiteItems
{
    //Configurators are exposed publicly, so they should be marked public
    public class FromSitemapXmlConfigurator : Karagulmez.Text.Fuzzy.Interfaces.IConfigurator
    {
        public int NGram
        {
            get { return 2; }   //2 = bigrams, 3 = trigrams
        }

        public Karagulmez.Text.Fuzzy.Interfaces.IItemsProvider ItemsProvider()
        {
            return FromSitemapXmlOnDiskItemsProvider.GetInstance();
        }

        public Karagulmez.Text.Fuzzy.Interfaces.IFilter Filter()
        {
            return UrlFilter.GetInstance();
        }
    }
}
