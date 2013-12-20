using Karagulmez.Text.Fuzzy;
using Karagulmez.Text.Fuzzy.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;


namespace FuzzyTextSearchImplementor
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();

            IFuzzyTextSearchManager fuzzyTextSearchManager = new FuzzyTextSearch();

            //Test if your assembly can be found by uncommenting the following line
            //var objectInstance = (Karagulmez.Text.Fuzzy.Interfaces.IConfigurator)Activator.CreateInstance("Acme.MySiteItems", "Acme.MySiteItems.FuzzyTextConfigurator").Unwrap();

            //load our configuration-by-code class, by specifying which assembly and class should be used
            fuzzyTextSearchManager.InitializeConfiguration("Acme.MySiteItems", "Acme.MySiteItems.FromSitemapXmlConfigurator");

            TimedSearch(fuzzyTextSearchManager, "/yaoho/bing");
            TimedSearch(fuzzyTextSearchManager, "/googlemaps/phone");
            TimedSearch(fuzzyTextSearchManager, "/news/archive/2012-04/17/google-drive-leak");
            TimedSearch(fuzzyTextSearchManager, "/news/archive/2012-04/17/google-drive");
            TimedSearch(fuzzyTextSearchManager, "/autonoomus-vehicles-legal-in-california");
            TimedSearch(fuzzyTextSearchManager, "/androidnexus7launches");
            TimedSearch(fuzzyTextSearchManager, "/playstationvita");

            //reinitalize with another configurator, without reinstantiating another search-manager
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("Reinitializing with textconfigurator...");
            fuzzyTextSearchManager.InitializeConfiguration("Acme.MySiteItems", "Acme.MySiteItems.FromTextFileConfigurator");

            TimedSearch(fuzzyTextSearchManager, "phohodiester");
            TimedSearch(fuzzyTextSearchManager, "hydroxymetyldoexyuidien");
            TimedSearch(fuzzyTextSearchManager, "desoxriboucleic");
            TimedSearch(fuzzyTextSearchManager, "relatiefly");
            TimedSearch(fuzzyTextSearchManager, "low");
            TimedSearch(fuzzyTextSearchManager, "boud");
            

            Console.ReadKey();
        }

        private static void TimedSearch(IFuzzyTextSearchManager fuzzyTextSearch, string searchTerm)
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IEnumerable<KeyValuePair<string, double>> searchResults = fuzzyTextSearch.Search(searchTerm, 3);
            stopWatch.Stop();
            Console.WriteLine("Searching for \"" + searchTerm + "\"");

            if (searchResults.Count() == 0)
            {
                Console.WriteLine("Sorry, no matches...");
            }
            else if (searchResults.First().Value == 100.0)
            {
                Console.WriteLine("100% match for \"" + searchResults.First().Key + "\"! (" + stopWatch.ElapsedMilliseconds + " ms, exact:" + stopWatch.Elapsed.ToString() + ")");
            }
            else
            {
                Console.WriteLine("Did you mean \"" + searchResults.First().Key + "\"? (" + stopWatch.ElapsedMilliseconds + " ms, exact:" + stopWatch.Elapsed.ToString() + ")");
            }

            Console.WriteLine("Top results:");
            foreach(KeyValuePair<string, double> kvp in searchResults)
            {
                Console.WriteLine(" " + Math.Round(kvp.Value, 2) + "% - " + kvp.Key);
            }
        }
    }
}
