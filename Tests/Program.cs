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
        static PluginClient client = new PluginClient();
        static Script s = new Script(@"Test.cs");
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            client.OnUnloadPlugin += OnPluginUnload;
            client.OnPluginPostObject += OnPluginReceivedOnject;
            client.PluginLoad(s);
            Console.ReadKey();
        }
        static void OnPluginReceivedOnject(object obj)
        {
            if (obj.GetType() == typeof(string))
            {
                if (obj == "Привет я плагин")
                {
                    Console.WriteLine(obj);
                    client.PluginPostObject(s, "Привет плагин");
                }
            }
        }
        static void OnPluginUnload()
        {
            Console.WriteLine("Плагин выключился");
        }
    }
}
