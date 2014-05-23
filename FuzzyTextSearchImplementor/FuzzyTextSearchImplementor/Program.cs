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
            //enable for debugging
            {
                Console.WriteLine("Process Paused");
                Console.WriteLine("Attach a debugger, then press any key to continue");
                Console.ReadKey();
            }

            IFuzzyTextSearchManager fuzzyTextSearchManager = new FuzzyTextSearch();

            //Test if your assembly can be found by uncommenting the following line
            //var objectInstance = (Karagulmez.Text.Fuzzy.Interfaces.IConfigurator)Activator.CreateInstance("Karagulmez.Text.Fuzzy.DefaultProviders", "Karagulmez.Text.Fuzzy.DefaultProviders.FuzzyTextConfigurator").Unwrap();
            {
                //load our configuration-by-code class, by specifying which assembly and class should be used
                fuzzyTextSearchManager.InitializeConfiguration("Karagulmez.Text.Fuzzy.DefaultProviders", "Karagulmez.Text.Fuzzy.DefaultProviders.FromSitemapXmlConfigurator");

                TimedSearch(fuzzyTextSearchManager, "/yaoho/bing");
                TimedSearch(fuzzyTextSearchManager, "/googlemaps/phone");
                TimedSearch(fuzzyTextSearchManager, "/news/archive/2012-04/17/google-drive-leak");
                TimedSearch(fuzzyTextSearchManager, "/news/archive/2012-04/17/google-drive");
                TimedSearch(fuzzyTextSearchManager, "/autonoomus-vehicles-legal-in-california");
                TimedSearch(fuzzyTextSearchManager, "/androidnexus7launches");
                TimedSearch(fuzzyTextSearchManager, "/playstationvita");
            }
           
            //reinitalize with another configurator, without reinstantiating another search-manager
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("Reinitializing with textconfigurator...");
            {
                fuzzyTextSearchManager.InitializeConfiguration("Karagulmez.Text.Fuzzy.DefaultProviders", "Karagulmez.Text.Fuzzy.DefaultProviders.FromTextFileConfigurator");

                TimedSearch(fuzzyTextSearchManager, "relatiefly");
                TimedSearch(fuzzyTextSearchManager, "low");
                TimedSearch(fuzzyTextSearchManager, "boud");
                TimedSearch(fuzzyTextSearchManager, "supeman");
                TimedSearch(fuzzyTextSearchManager, "releaset");
                TimedSearch(fuzzyTextSearchManager, "interiews");
                TimedSearch(fuzzyTextSearchManager, "informtion");
                TimedSearch(fuzzyTextSearchManager, "dissapeared");
                TimedSearch(fuzzyTextSearchManager, "truted");
                TimedSearch(fuzzyTextSearchManager, "knokking");
                TimedSearch(fuzzyTextSearchManager, "suface");
            }

            //reinitalize with another configurator, without reinstantiating another search-manager
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("Reinitializing with in-memory data configurator 1...");

            {
                var inMemoryUrlsList = new List<string>();
                inMemoryUrlsList.AddRange(new string[]{                    
                    "shows/luxor"
                });

                var translateDict = new Dictionary<string, string>();
                translateDict.Add("/media/", string.Empty);
                translateDict.Add("http(s)://{.*}/", string.Empty); //filter out the domain, if it's there
                translateDict.Add("/[a-z][0-9]/", "sony"); //if there's a single letter and a single digit, use searchterm "Sony"
                translateDict.Add("/", string.Empty);


                //Wrap these in a dictionary as a container for a single object
                //Use whatever fancy code/container/proxy/data-structure you like in your case, 
                //this is just one of the possible examples
                Dictionary<string, object> initializationData = new Dictionary<string, object>();
                initializationData.Add("filter", translateDict);
                initializationData.Add("strings", inMemoryUrlsList);

                //pass our data to our fresh new plugin.
                fuzzyTextSearchManager.InitializeConfiguration("Karagulmez.Text.Fuzzy.DefaultProviders", "Karagulmez.Text.Fuzzy.DefaultProviders.FromInMemoryUrlsConfigurator",
                                                                initializationData);

                //Test it
                TimedSearch(fuzzyTextSearchManager, "lux");
            }


            //Note that there's no harm in re-initializing with the same data - that's fine.
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Console.WriteLine("Reinitializing with in-memory data configurator 2...");

            {
                //we add an in-memory list of url's we feed into the fuzzyTextSearchManager
                var inMemoryUrlsList = new List<string>();
                inMemoryUrlsList.AddRange(new string[]{
                    "/yahoo/bing/google", 
                    "/google/android",
                    "/microsoft/azure",
                    "/apple/iphone",
                    "/media/1234",
                    "/test/z1/",
                    "shows/luxor",
                });

                var translateDict = new Dictionary<string, string>();
                translateDict.Add("/media/", string.Empty);
                translateDict.Add("http(s)://{.*}/", string.Empty); //filter out the domain, if it's there
                translateDict.Add("/[a-z][0-9]/", "sony"); //if there's a single letter and a single digit, use searchterm "Sony"
                translateDict.Add("/", string.Empty);
                
                //Wrap these in a dictionary as a container for a single object
                //Use whatever fancy code/container/proxy/data-structure you like in your case, 
                //this is just one of the possible examples
                Dictionary<string, object> initializationData = new Dictionary<string, object>();
                initializationData.Add("filter", translateDict);
                initializationData.Add("strings", inMemoryUrlsList);
                
                //pass our data to our fresh new plugin.
                fuzzyTextSearchManager.InitializeConfiguration("Karagulmez.Text.Fuzzy.DefaultProviders", "Karagulmez.Text.Fuzzy.DefaultProviders.FromInMemoryUrlsConfigurator",
                                                                initializationData);
                
                //Test it
                TimedSearch(fuzzyTextSearchManager, "micsoft");
                TimedSearch(fuzzyTextSearchManager, "goodleandoid");
                //Test 100% match
                TimedSearch(fuzzyTextSearchManager, "/google/android");

                //Filter-test: next item will not find "/media/1234", because we filtered "media" out
                TimedSearch(fuzzyTextSearchManager, "media");

                //Test regular expressions, 
                //single letter followed by single digit will be found by "sony"
                //in the testurls, this is the item "/test/z1"
                TimedSearch(fuzzyTextSearchManager, "sony");                
                TimedSearch(fuzzyTextSearchManager, "lux");
            }

            {
                //Here we will make suggestions based on text we feed as a string. Any text is ok!
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("==================================================");
                Console.WriteLine("Reinitializing with in-memory text configurator...");

                string initializationData = "The latest Microsoft product is a laptop which is called the Surface. Actually it's a tablet. The Apple Air is bigger than it.";
                fuzzyTextSearchManager.InitializeConfiguration("Karagulmez.Text.Fuzzy.DefaultProviders", "Karagulmez.Text.Fuzzy.DefaultProviders.FromInMemoryTextItemsConfigurator",
                                                                initializationData);

                TimedSearch(fuzzyTextSearchManager, "moccosoft");
                TimedSearch(fuzzyTextSearchManager, "leptop");
                TimedSearch(fuzzyTextSearchManager, "sufface");
                TimedSearch(fuzzyTextSearchManager, "Appel");
                TimedSearch(fuzzyTextSearchManager, "Airs");
                TimedSearch(fuzzyTextSearchManager, "biger");
                TimedSearch(fuzzyTextSearchManager, "then");
                TimedSearch(fuzzyTextSearchManager, "surfuce");
                TimedSearch(fuzzyTextSearchManager, "produt");
                TimedSearch(fuzzyTextSearchManager, "a"); //smaller than the n-gram, so empty collection
            }

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }

        private static void TimedSearch(IFuzzyTextSearchManager fuzzyTextSearchManager, string searchTerm)
        {
            Console.WriteLine();
            Console.WriteLine("==================================================");
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            IEnumerable<KeyValuePair<string, double>> searchResults = fuzzyTextSearchManager.Search(searchTerm, 3);
            stopWatch.Stop();
            Console.WriteLine("Searching for \"" + searchTerm + "\"");

            if (searchResults.Count() == 0)
            {
                Console.WriteLine("Sorry, no matches...");
            }
            else if (searchResults.First().Value == 100.0)
            {
                Console.WriteLine("100% match for \"" + searchResults.First().Key + "\"! (" + stopWatch.ElapsedMilliseconds + " ms, exact: " + stopWatch.Elapsed.ToString() + ")");
            }
            else
            {
                Console.WriteLine("Did you mean \"" + searchResults.First().Key + "\"? (" + stopWatch.ElapsedMilliseconds + " ms, exact: " + stopWatch.Elapsed.ToString() + ")");
            }

            foreach(KeyValuePair<string, double> kvp in searchResults)
            {
                Console.WriteLine(" " + Math.Round(kvp.Value, 2) + "% - " + kvp.Key);
            }
        }
    }
}