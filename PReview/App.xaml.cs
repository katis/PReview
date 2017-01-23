using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace PReview
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Count() > 0)
            {
                var splitArg = e.Args[0].Split(new char[] { ';' }, 2);
                var eviNumber = splitArg[0];
                var uri = splitArg[1];

                if (eviNumber.Length > 0)
                {
                    var eviUri = $"https://atlas.elisa.fi/jira/browse/EVI-{eviNumber}";
                    Process.Start(new ProcessStartInfo(eviUri));
                }

                Process.Start(new ProcessStartInfo(uri));
                Shutdown();
            } else if (!File.Exists(Config.CONFIG_PATH)) // Create and open a config if one doesn't exist
            {
                Directory.CreateDirectory(Config.CONFIG_DIR);
                var defaultConfig = new Config();
                defaultConfig.Write(Config.CONFIG_PATH);
                Process.Start(Config.CONFIG_PATH);
                Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
