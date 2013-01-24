// -----------------------------------------------------------------------
// <copyright file="Deploy.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Courier.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Umbraco.Courier.Core;
    using System.IO;
    using Umbraco.Courier.Core.Storage;
    using Umbraco.Courier.Core.Packaging;
    using NAnt.Core.Attributes;
    using Umbraco.Courier.Core.Collections.Manifests;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [TaskName("package")]
    public class Package : NAnt.Core.Task
    {
        [TaskAttribute("config", Required = true)]
        public string Config { get; set; }

        [TaskAttribute("source", Required = true)]
        public string Source { get; set; }

        [TaskAttribute("target", Required = true)]
        public string Target { get; set; }

        [TaskAttribute("revision", Required = true)]
        public string Revision { get; set; }

        [TaskAttribute("manifest", Required = true)]
        public string Manifest { get; set; }


        protected override void ExecuteTask()
        {
            Console.WriteLine("");
            Console.WriteLine("-------------------");
            Console.WriteLine("Application: " + application);
            Console.WriteLine("Config: " + Config);
            Console.WriteLine("plugins: " + plugins);

            init();

            foreach (var repo in Umbraco.Courier.Core.ProviderModel.RepositoryProviderCollection.Instance.GetProviders())
            {
                Console.WriteLine("Repo provider loaded: " + repo.Name + "/" + repo.GetType().ToString());
            }
            Console.WriteLine("-------------------");
            
            
            Repository target = null;
            Repository source = null;

            using(var rs = new RepositoryStorage()){
                source = rs.GetByAlias(Source);

                if(!string.IsNullOrEmpty(Target))
                    target = rs.GetByAlias(Target);
            }
            
            if(source == null)
                Console.WriteLine("Could not find source: " + Source);

            if (target == null)
                Console.WriteLine("Could not find target: " + Source);
            
            var engine = new RevisionPackaging(Revision);
            engine.Source = source;
            
            engine.StoredItem += new EventHandler<ItemEventArgs>(engine_StoredItem);
            engine.SkippedItem += new EventHandler<ItemEventArgs>(engine_SkippedItem);
            
            if (target != null)
            {
                Console.WriteLine("Enabling compare....");
                engine.EnableInstantCompare(target);
            }


            Console.WriteLine("Loading manifest...");
            var manifest = RevisionManifest.Load( Manifest );
            if (manifest == null)
                Console.WriteLine("Manifest file not found: " + Manifest);
            else
            {
                engine.AddToQueue(manifest);

                Console.WriteLine("Starting packaging...");
                engine.Package();
                engine.Dispose();

                Console.ResetColor();

                Console.WriteLine("All done, yay!");
            }
        }

        void engine_SkippedItem(object sender, ItemEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Skipped: " + e.Item.Name);
        }

        void engine_StoredItem(object sender, ItemEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Packaged: " + e.Item.Name);
        }
        
        static string application = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string directory = Directory.GetCurrentDirectory();
        static string plugins = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "courierplugins");

        private void init()
        {
            //To init our app, we need to load all provider dlls from our plugins dir
            //The application needs the DLL umbraco.courier.providers.dll to work with the build-in providers
            //you can add any dll in there to load your own

            Umbraco.Courier.Core.Helpers.TypeResolver.LoadAssembliesIntoAppDomain(plugins, "*.dll");

            //we also need to tell it where to get it's config xml
            //this is the standard courier file which contains settings for the engines and providers
            Umbraco.Courier.Core.Context.Current.SettingsFilePath = Context.Current.MapPath(Config);

            //finally we need to redirect the revisions root for correct mapping
            Umbraco.Courier.Core.Context.Current.BaseDirectory = directory;

            Umbraco.Courier.Core.Context.Current.HasHttpContext = false;
        }

    }
}
