using PluginsAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tests
{
    static class Program
    {
        static PluginUpdater updater = new PluginUpdater();
        static PluginClient test_plugin_client = new PluginClient(updater);
        static Script s = new Script(@"Test.cs");
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            test_plugin_client.OnPluginPostObject += OnPluginReceivedObject;
            test_plugin_client.PluginLoad(s);
            Console.ReadKey();
        }
        static void OnPluginReceivedObject(object obj)
        {
            if (obj.GetType() == typeof(string))
            {
                if (obj == "Привет я плагин")
                {
                    Console.WriteLine(obj);
                    test_plugin_client.PluginPostObject(s, "Привет плагин");
                    updater.Stop();
                }
            }
        }
    }
}
