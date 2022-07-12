using PluginsAPI;
using System;

namespace Tests
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            PluginUpdater pluginUpdater = new PluginUpdater();
            PluginClient pluginClient = new PluginClient(pluginUpdater);
            WebScript webScript = new WebScript("https://raw.githubusercontent.com/Nekiplay/Temp/main/Test2.cs");
            pluginClient.PluginLoad(webScript);
            Console.ReadKey();
        }
    }
}
