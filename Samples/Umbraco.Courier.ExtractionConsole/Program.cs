using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Umbraco.Courier.Core;

namespace Umbraco.Courier.ExtractionConsole
{
    class Program
    {
        static void Extract(string[] args)
        {
            string error;

            Console.Write("Building dependency graph from directory " + directory + "...");
            try
            {   
                //Umbraco.Courier.Core.Services.Extractor ex = new Core.Services.Extractor(directory);
                var engine = new Core.Extraction.RevisionExtraction(directory);
                engine.ExtractingItem += new EventHandler<Core.ItemEventArgs>(ex_ExtractingItem);
                engine.ExtractingItemResources += new EventHandler<Core.ItemEventArgs>(engine_ExtractingItemResources);

                engine.PopulateGraph();

                    
                int totalItems = engine.ExtractionGraph.CountUnique();

                Console.Write("DONE, " + totalItems.ToString() + " items graphed");

                Console.WriteLine("");
                Console.WriteLine("");

                Console.WriteLine("Continue? (Y/N)");
                Console.WriteLine("");
                Console.WriteLine("");


                if (Console.ReadKey().KeyChar.ToString().ToLower() == "y")
                {
                    if (string.IsNullOrEmpty(url))
                    {
                        Console.WriteLine("Website to connect to?");
                        url = Console.ReadLine();
                    }

                    Umbraco.Courier.Core.Storage.RepositoryStorage rs = new Core.Storage.RepositoryStorage();
                    var r = rs.GetByAlias(url);
                    rs.Dispose();

                    engine.Destination = r;

                    //this is handled by the config instead
                    //engine.ResourceOverwritemode = Core.Enums.OverwriteMode.Always;
                    //engine.ItemOverwritemode = Core.Enums.OverwriteMode.Always;
                    //engine.DependencyOverwritemode = Core.Enums.OverwriteMode.Always;
                    

                    Console.WriteLine("");
                    Console.WriteLine("");

                   
                        Console.WriteLine("");
                        Console.WriteLine("");

                        //builds the graph of dependencies

                        Console.WriteLine("Extracting all items in the graph, and sending them to the server...");
                        Console.WriteLine("This will take some time, so admire this lovely buddha in the meantime...");
                        Console.WriteLine("");

                        Console.WriteLine(@"                           _");
                        Console.WriteLine(@"                        _ooOoo_");
                        Console.WriteLine(@"                       o8888888o");
                        Console.WriteLine(@"                       88' . '88");
                        Console.WriteLine(@"                       (| -_- |)");
                        Console.WriteLine(@"                       O\  =  /O");
                        Console.WriteLine(@"                    ____/`---'\____");
                        Console.WriteLine(@"                  .'  \\|     |//  `.");
                        Console.WriteLine(@"                 /  \\|||  :  |||//  \");
                        Console.WriteLine(@"                /  _||||| -:- |||||_  \");
                        Console.WriteLine(@"                |   | \\\  -  /'| |   |");
                        Console.WriteLine(@"                | \_|  `\`---'//  |_/ |");
                        Console.WriteLine(@"                \  .-\__ `-. -'__/-.  /");
                        Console.WriteLine(@"              ___`. .'  /--.--\  `. .'___");
                        Console.WriteLine(@"           ."" '<  `.___\_<|>_/___.' _> \"".");
                        Console.WriteLine(@"          | | :  `- \`. ;`. _/; .'/ /  .' ; |");
                        Console.WriteLine(@"          \  \ `-.   \_\_`. _.'_/_/  -' _.' /");
                        Console.WriteLine(@"===========`-.`___`-.__\ \___  /__.-'_.'_.-'================");
                        Console.WriteLine(@"                        `=--=-'                   ");

                        Console.WriteLine("");
                        
                    engine.Extract();


                        Console.WriteLine("");
                        Console.WriteLine("");
                        Console.WriteLine("======================================");
                        Console.WriteLine("Whooaah, DONE! ");
                        Console.WriteLine("======================================");

                    engine.ExtractionGraph.ToMindmap().Save("mindmap.bmd");
                    engine.Dispose();
                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("");
                    Console.WriteLine("======================================");
                    Console.WriteLine("Operation cancelled...");
                    Console.WriteLine("======================================");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("");
                Console.WriteLine("======================================");
                Umbraco.Courier.Core.Helpers.Logging._Debug(ex.ToString());
                Console.WriteLine(ex.ToString());
                Console.WriteLine("======================================");
                Console.Read();
            }
        }


		#region Fields (7) 

        static string application = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string directory = Directory.GetCurrentDirectory();
        static string mode = "extract";
        static string pass = "1234";
        static string plugins = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "plugins");
        static string url = "";
        static string user = "admin";

		#endregion Fields 

		#region Methods (6) 

		// Private Methods (6) 

        static void engine_ExtractingItemResources(object sender, Core.ItemEventArgs e)
        {
            Console.Write("*");
        }

        static void ex_ExtractingItem(object sender, Core.ItemEventArgs e)
        {
            Console.Write(".");
        }

        static void ex_PackagingItem(object sender, Core.ItemEventArgs e)
        {
            Console.Write(".");
        }

        
        static void Main(string[] args)
        {
            //To init our app, we need to load all provider dlls from our plugins dir
            //The application needs the DLL umbraco.courier.providers.dll to work with the build-in providers
            //you can add any dll in there to load your own
            Umbraco.Courier.Core.Helpers.TypeResolver.LoadAssembliesIntoAppDomain(plugins, "*.dll");
            
            //we also need to tell it where to get it's config xml
            //this is the standard courier file which contains settings for the engines and providers
            Umbraco.Courier.Core.Context.Current.SettingsFilePath = Path.Combine(application, "courier.config");

            //finally we need to redirect the revisions root for correct mapping
            Umbraco.Courier.Core.Context.Current.BaseDirectory = directory;
            Umbraco.Courier.Core.Context.Current.HasHttpContext = false;

            if (args.Length > 0)
                mode = args[0];           

            if (args.Length > 1)
                url = args[1];

            Console.WriteLine("======================================");
            Console.WriteLine("THE AMAZING COURIER DEPLOYMENT CONSOLE");
            Console.WriteLine("======================================");
            Console.WriteLine("");
            Console.WriteLine("");

            Console.Write(Umbraco.Courier.Core.Context.Current.SettingsFilePath );

            foreach (var repo in Umbraco.Courier.Core.ProviderModel.RepositoryProviderCollection.Instance.GetProviders())
            {
                Console.WriteLine("Repo provider loaded: " + repo.Name + "/" + repo.GetType().ToString());
            }

            Console.WriteLine("-------------------");


            if (mode == "extract")
                Extract(args);
            else if (mode == "package")
                Package(args);
        }

        static void Package(string[] args)
        {
            try
            {
                string revision = args[1];
                string url = args[2];

                Console.WriteLine("");
                Console.WriteLine(@"       _");
                Console.WriteLine(@"      (_)");
                Console.WriteLine(@"     <___>");
                Console.WriteLine(@"      | |_________");
                Console.WriteLine(@"      | |`-._`-._(___________");
                Console.WriteLine(@"      | |`-._`-._|   :|    |(__________");
                Console.WriteLine(@"      | |    `-._|   :|    || _.-'_.-'|");
                Console.WriteLine(@"      | | _ _ _ _|._ :|    ||'_.-'_.-'|");
                Console.WriteLine(@"      | |--------|._`:|    ||'_.-'    |");
                Console.WriteLine(@"      | |        |----      |' _ _ _ _|");
                Console.WriteLine(@"      | |________|          |---------|");
                Console.WriteLine(@"      | |- - - - |____      |         |");
                Console.WriteLine(@"      | |     _.-|.--:|    ||_________|");
                Console.WriteLine(@"      | | _.-'_.-|.-':|    ||- - - - -|");
                Console.WriteLine(@"      | |'_.-'_.-|   :|    ||`-._     |");
                Console.WriteLine(@"      | |~~~~~~~~|   :|    ||`-._`-._ |");
                Console.WriteLine(@"      | |        '~~~~~~~~~~|`-._`-._`|");
                Console.WriteLine(@"      | |                   '~~~~~~~~~~");
                Console.WriteLine(@"      | |");

                

                string[] providers = new string[0];
                
                if(args.Length >= 4)
                    providers = args[3].Split(',');

                Umbraco.Courier.Core.Settings.revisionsPath = "";

                var engine = new Core.Packaging.RevisionPackaging(revision);
                engine.AddedItem += new EventHandler<ItemEventArgs>(engine_AddedItem);
                engine.PackagedItem += new EventHandler<ItemEventArgs>(engine_PackagedItem);
                engine.PackagedItemResource += new EventHandler<ResourceEventArgs>(engine_PackagedItemResource);

                
                Core.Storage.RepositoryStorage rs = new Core.Storage.RepositoryStorage();
                var r = rs.GetByAlias(url);
                rs.Dispose();

                engine.Source = r;
                
                if (providers.Length == 0)
                    engine.AddAllProvidersToQueue(true);
                else
                {
                    foreach (var s in providers)
                    {
                        if (s.Contains(":"))
                        {
                            string provider = s.Split(':')[0];
                            string[] items = s.Split(':')[1].Split(',');

                        }
                        else
                        {
                            var p = Umbraco.Courier.Core.ProviderModel.ItemProviderCollection.Instance.GetProvider(s);
                            if (p != null)
                            {
                                var rootItems = p.AvailableSystemItems();
                                engine.AddToQueue(rootItems, true, int.MaxValue);
                            }
                        }
                    }
                }


                engine.Package();

                Console.WriteLine("================ DONE =======================");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void engine_PackagedItemResource(object sender, ResourceEventArgs e)
        {
            Console.WriteLine("Packaged Resource: " + e.Resource.ExtractToPath);
        }

        static void engine_PackagedItem(object sender, ItemEventArgs e)
        {
            Console.WriteLine("Packaged item: " + e.Item.Name);
        }

        static void engine_AddedItem(object sender, ItemEventArgs e)
        {
            Console.Write("*");
        }

		#endregion Methods 
    }
}
