using PluginsAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tests
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Client client = new Client();
            client.OnUnloadPlugin += OnPluginUnload;
            client.PluginLoad(new Script(@"Test.cs"));
            client.OnPluginPostObject += OnPluginReceivedOnject;
            Console.ReadKey();
        }
        static void OnPluginReceivedOnject(object obj)
        {
            if (obj.GetType() == typeof(string))
            {
                Console.WriteLine(obj);
            }
        }
        static void OnPluginUnload()
        {
            Console.WriteLine("Плагин выключился");
        }
    }
}
