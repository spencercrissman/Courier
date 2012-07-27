using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NAnt.Core;
using NAnt.Core.Attributes;
using Umbraco.Courier.Core;
using System.IO;
using Umbraco.Courier.Core.Storage;
using Umbraco.Courier.Core.Extraction;

namespace Courier.Tasks
{
    [TaskName("extract")]
    public class Extract : NAnt.Core.Task
    {
        [TaskAttribute("config", Required=true)]
        public string Config { get; set; }

        [TaskAttribute("target", Required = true)]
        public string Target { get; set; }

        [TaskAttribute("revision", Required = true)]
        public string Revision { get; set; }

        protected override void ExecuteTask()
        {

            Console.WriteLine("Application: " + application);
            Console.WriteLine("Config: " + Config);
            Console.WriteLine("plugins: " + plugins);

            init();

            RepositoryStorage rs = new RepositoryStorage();
            var r = rs.GetByAlias(Target);
            rs.Dispose();

            var engine = new RevisionExtraction(Revision);
            engine.Destination = r;
            engine.ExtractedItem += new EventHandler<ItemEventArgs>(engine_ExtractedItem);
            
            engine.Extract();

            engine.Dispose();

            Console.WriteLine("All done, yay!");
        }

        void engine_ExtractedItem(object sender, ItemEventArgs e)
        {
            Console.WriteLine("extracting: " + e.Item.Name);
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

    public class ConsoleSpiner
    {
        int counter;
        public ConsoleSpiner()
        {
            counter = 0;
        }
        public void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("-"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    } 
    
}
