using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Umbraco.Courier.Core;
using Umbraco.Courier.Core.Storage;
using Umbraco.Courier.RepositoryProviders;
using System.IO;

namespace PackagingTool
{
    class Program
    {
        static string application = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        static string directory = Directory.GetCurrentDirectory();
        static string mode = "extract";
        static string pass = "1234";
        static string plugins = Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "plugins");
        static string url = "";
        static string user = "admin";

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


            try
            {
                PackagingManager manager = PackagingManager.Instance;

                CourierWebserviceRepositoryProvider cp = new CourierWebserviceRepositoryProvider();
                cp.Login = "admin";
                cp.Password = "1234";
                cp.PasswordEncoding = "Hashed";
                cp.UserId = -1;
                cp.Url = "http://cws.local";


                Repository r = new Repository(cp);
                r.Name = "Console app repo";
                r.Alias = "console";

                manager.Load(Path.Combine(directory, "testingDirectory"));
                manager.EnableRemoteExtraction(r);

                manager.PackagedItem += new EventHandler<ItemEventArgs>(manager_PackagedItem);

                string err = "";

                

                manager.AddAllProvidersToQueue(true);

                Console.Write(manager.Queue.Count + " Items added");

                manager.PackageQueue();
                
     
                Console.Write("DONE");
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());

            }

            Console.Read();
        }

        static void manager_PackagedItem(object sender, ItemEventArgs e)
        {
            Console.Write("*");
        }
    }
}
