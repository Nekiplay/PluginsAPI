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
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            PluginUpdater pluginUpdater = new PluginUpdater();
            PluginClient pluginClient = new PluginClient(pluginUpdater);
            AutoClicker auto = new AutoClicker();
            pluginClient.PluginLoad(auto);
            Console.ReadKey();
        }
    }

    public class AutoClicker : Plugin
    {
        public override void Update()
        {
            if (IsKeyPressed(Keys.XButton2))
            {
                for (int i = 0; i < 20; i++)
                {
                    if (IsKeyPressed(Keys.XButton2))
                    {
                        LeftClick();
                        Thread.Sleep(50);
                    }
                    else { break; }
                }
            }
            if (IsKeyPressed(Keys.XButton2))
            {
                for (int i = 0; i < 30; i++)
                {
                    if (IsKeyPressed(Keys.XButton2))
                    {
                        LeftClick();
                        Thread.Sleep(60);
                    }
                    else { break; }
                }
            }
        }
    }
}
