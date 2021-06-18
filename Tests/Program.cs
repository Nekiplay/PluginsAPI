using PluginsAPI;
using System;
using System.Collections.Generic;
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
            client.PluginLoad(new PluginsAPI.Script(@"Test.cs"));
            Console.ReadKey();
        }
    }
}
