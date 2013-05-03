using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Storage;
using System.Reflection;

namespace ExtractionTool
{
    class Program
    {
        static string application = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        
        static string directory = Directory.GetCurrentDirectory();

        static string userConfig = Path.Combine(directory, System.Environment.MachineName + ".courier.config");
        static string courierConfig = Path.Combine(directory, "courier.config");
        static string pluginFolder = Path.Combine(directory, "plugins");
        
        static void Deploy(XmlDocument connectionSettings) {
            List<RevisionSetting> revs = GetRevisionsFroDeploy(connectionSettings);
            Deploy(revs);
        }

        static void Deploy(IEnumerable<RevisionSetting> revisions) {
            try
            {  
                Console.WriteLine("Loading extraction instance");

                ExtractionManager manager = ExtractionManager.Instance;
                RepositoryStorage rs = new RepositoryStorage();

                manager.ExtractedItem += new EventHandler<ItemEventArgs>(manager_ExtractedItem);
                manager.Extracted += new EventHandler<ExtractionEventArgs>(manager_Extracted);

                manager.OverwriteExistingDependencies = true;
                manager.OverwriteExistingitems = true;
                manager.OverwriteExistingResources = true;

                Console.WriteLine("Deployment manager loaded");


                foreach (var rev in revisions.Where(x => x.Active))
                {
                    string revFolder = Path.GetFullPath(rev.Folder);
                    Console.WriteLine("Loading folder: " + revFolder);

                    Repository r = rs.GetByAlias(rev.Alias);

                    /*
                    Console.WriteLine("Syncing files");
                    foreach (var s in rev.Syncs)
                    {
                        string pattern = "*.*";
                        if (!string.IsNullOrEmpty(s.Pattern))
                            pattern = s.Pattern;

                        var files = System.IO.Directory.GetFiles(s.Target, pattern);
                    }
                    */


                    Console.WriteLine("Enabling remote deployment for: " + r.Name);
                    manager.EnableRemoteExtraction(r);

                    Console.WriteLine("Loading Contents: " + revFolder);
                    manager.Load(revFolder);

                    Console.WriteLine("Building graph...");
                    manager.BuildGraph();

                    Console.WriteLine(manager.ExtractionGraph.CountUnique() + " Items added to graph");

                    Console.WriteLine("Extraction...");
                    manager.ExtractAll(true, true);

                    Console.WriteLine("Unloading...");
                    manager.Unload();
                }

                Console.WriteLine("DONE...");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void manager_Extracted(object sender, ExtractionEventArgs e)
        {
            Console.WriteLine("extraction completed");
        }
        
        static void manager_ExtractedItem(object sender, ItemEventArgs e)
        {
            Console.WriteLine(e.Item.Name + " Extracted");
        }


        static List<RevisionSetting> GetRevisionsFroDeploy(XmlDocument settings)
        {
            List<RevisionSetting> revisions = new List<RevisionSetting>();

            foreach (XmlNode n in settings.SelectNodes("//revision"))
            {
                RevisionSetting rev = new RevisionSetting();

                rev.Alias = n.Attributes["repository"].Value;
                rev.Folder = n.Attributes["folder"].Value;
                rev.Active = false;

                rev.OverWrite = true;

                if (n.Attributes["active"] != null && n.Attributes["active"].Value.ToLower() == "true")
                    rev.Active = true;

                if (n.Attributes["overWrite"] != null && n.Attributes["overWrite"].Value.ToLower() == "false")
                    rev.OverWrite = false;

                rev.OverWriteDependencies = rev.OverWrite;
                rev.OverWriteResources = rev.OverWrite;
                
                if (n.Attributes["overWriteFiles"] != null && n.Attributes["overWriteFiles"].Value.ToLower() == "true")
                    rev.OverWriteResources = true;

                if (n.Attributes["overWriteDependencies"] != null && n.Attributes["overWriteDependencies"].Value.ToLower() == "true")
                    rev.OverWriteDependencies = true;


                List<Sync> syncs = new List<Sync>();
                foreach (XmlNode ns in n.SelectNodes("./sync"))
                {
                    Sync fs = new Sync();
                    fs.Source = Context.Current.MapPath(ns.Attributes["source"].Value);
                    fs.Target = Path.GetFullPath(ns.Attributes["target"].Value);
                    fs.Pattern = string.Empty;
                    
                    if (ns.Attributes["pattern"] != null)
                        fs.Pattern = ns.Attributes["pattern"].Value;

                    syncs.Add(fs);
                }

                revisions.Add(rev);
            }

            return revisions;
        }



        static void Main(string[] args)
        {
            try
            {
                //To init our app, we need to load all provider dlls from our plugins dir
                //The application needs the DLL umbraco.courier.providers.dll to work with the build-in providers
                //you can add any dll in there to load your own
                Umbraco.Courier.Core.Helpers.TypeResolver.LoadAssembliesIntoAppDomain(pluginFolder, "umbraco.courier.providers.dll");
                Umbraco.Courier.Core.Helpers.TypeResolver.LoadAssembliesIntoAppDomain(pluginFolder, "umbraco.courier.dataresolvers.dll");
                Umbraco.Courier.Core.Helpers.TypeResolver.LoadAssembliesIntoAppDomain(pluginFolder, "umbraco.courier.repositoryproviders.dll");

                //AppDomain currentDomain = AppDomain.CurrentDomain;
                //currentDomain.AssemblyResolve += new ResolveEventHandler(currentDomain_AssemblyResolve);


                //we also need to tell it where to get it's config xml
                //this is the standard courier file which contains settings for the engines and providers
                Umbraco.Courier.Core.Context.Current.SettingsFilePath = courierConfig;

                //finally we need to redirect the revisions root for correct mapping
                Umbraco.Courier.Core.Context.Current.BaseDirectory = directory;
                Umbraco.Courier.Core.Context.Current.HasHttpContext = false;
                
                Arguments arguments = new Arguments(args);

                Console.WriteLine(Umbraco.Courier.Core.Context.Current.SettingsFilePath);
                Console.WriteLine(pluginFolder);
                
                if (arguments["config"] != null)
                {
                    string cfg = Context.Current.MapPath(arguments["config"]);

                    Console.WriteLine(cfg);

                    XmlDocument xd = new XmlDocument();
                    xd.Load(cfg);

                    Deploy(xd);
                }
                else if (File.Exists(userConfig))
                {
                    Console.WriteLine(userConfig);

                    XmlDocument xd = new XmlDocument();
                    xd.Load(userConfig);

                    Deploy(xd);
                }
                else
                {
                    throw new Exception(userConfig + " not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        
    }

    class Sync
    {
        public string Target { get; set; }
        public string Source { get; set; }
        public string Pattern { get; set; }

    }

    class RevisionSetting
    {
        public string Folder { get; set; }
        public string Alias { get; set; }
        public List<Sync> Syncs { get; set; }

        public bool Active { get; set; }

        public bool OverWriteResources { get; set; }
        public bool OverWrite { get; set; }
        public bool OverWriteDependencies { get; set; }
    }
}
