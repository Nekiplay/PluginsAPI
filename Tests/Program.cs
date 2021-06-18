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
            List<Action> actions = new List<Action>();
            actions.Add(Unload);
            client.OnUnloadPlugin = actions;
            client.PluginLoad(new Script(@"Test.cs"));
            Console.ReadKey();
        }
        static void Unload()
        {
            Console.WriteLine("Плагин выключился");
            StreamReader sr = new StreamReader("result.txt");
            string line;
            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                Console.WriteLine(line);
            }
            sr.Close();
        }
    }
}
