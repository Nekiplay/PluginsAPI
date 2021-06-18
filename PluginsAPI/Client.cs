using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace PluginsAPI
{
    public class PluginClient
    {
        Thread netRead; // main thread
        public PluginClient()
        {
            StartUpdating();
        }
        public void Quit()
        {
            netRead = null;
            foreach (Plugin p in plugins)
            {
                PluginUnLoad(p);
            }
        }
        private void StartUpdating()
        {
            netRead = new Thread(new ThreadStart(Updater));
            netRead.Name = "PacketHandler";
            netRead.Start();
        }
        private readonly Dictionary<string, List<Plugin>> registeredPluginsPluginChannels = new Dictionary<string, List<Plugin>>();
        private readonly List<Plugin> plugins = new List<Plugin>();

        #region Системное
        public static bool isUsingMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }
        private void Updater()
        {
            try
            {
                bool keepUpdating = true;
                Stopwatch stopWatch = new Stopwatch();
                while (keepUpdating)
                {
                    stopWatch.Start();
                    keepUpdating = Update();
                    stopWatch.Stop();
                    int elapsed = stopWatch.Elapsed.Milliseconds;
                    stopWatch.Reset();
                    if (elapsed < 100)
                        Thread.Sleep(100 - elapsed);
                }
            }
            catch (System.IO.IOException) { }
            catch (ObjectDisposedException) { }
        }
        private bool Update()
        {
            try
            {
                OnUpdate();

            }
            catch (System.IO.IOException) { return false; }
            catch (NullReferenceException) { return false; }
            return true;
        }
        public void OnUpdate()
        {
            foreach (Plugin bot in plugins.ToArray())
            {
                try
                {
                    bot.Update();
                }
                catch { }
            }
        }
        #endregion

        #region Получение и отправка данных от плагина
        public Action OnUnloadPlugin { set; get; }
        public Action<object> OnPluginPostObject { set; get; }
        public void OnPluginPostObjectMethod(object ob)
        {
            if (OnPluginPostObject != null)
            {
                OnPluginPostObject(ob);
            }
        }

        #endregion

        #region Управление плагином
        public void PluginLoad(Plugin b, bool init = true)
        {
            b.SetHandler(this);
            plugins.Add(b);
            if (init)
            {
                List<Plugin> temp = new List<Plugin>();
                temp.Add(b);
                //new Plugin[] { b }
                DispatchPluginEvent(bot => bot.Initialize(), temp);
            }
        }
        public void PluginPostObject(Plugin b, object obj)
        {
            foreach (Plugin bot in plugins.ToArray())
            {
                try
                {
                    bot.ReceivedObject(obj);
                }
                catch { }
            }
        }
        public void PluginUnLoad(Plugin b)
        {
            plugins.RemoveAll(item => object.ReferenceEquals(item, b));

            var botRegistrations = registeredPluginsPluginChannels.Where(entry => entry.Value.Contains(b)).ToList();
            foreach (var entry in botRegistrations)
            {
                UnregisterPluginChannel(entry.Key, b);
            }
        }
        #endregion

        #region Регистрация плагинов
        private void DispatchPluginEvent(Action<Plugin> action, IEnumerable<Plugin> botList = null)
        {
            Plugin[] selectedBots;

            if (botList != null)
            {
                selectedBots = botList.ToArray();
            }
            else
            {
                selectedBots = plugins.ToArray();
            }

            foreach (Plugin bot in selectedBots)
            {
                try
                {
                    action(bot);
                    //Console.WriteLine("Выполнил: " + action.Method);
                }
                catch (Exception e)
                {
                    if (!(e is ThreadAbortException))
                    {
                        //Retrieve parent method name to determine which event caused the exception
                        System.Diagnostics.StackFrame frame = new System.Diagnostics.StackFrame(1);
                        System.Reflection.MethodBase method = frame.GetMethod();
                        string parentMethodName = method.Name;

                        //Display a meaningful error message to help debugging the ChatBot
                        Console.WriteLine(parentMethodName + ": Got error from " + bot.ToString() + ": " + e.ToString());
                    }
                    else throw;
                }
            }
        }

        public void UnregisterPluginChannel(string channel, Plugin bot)
        {
            if (registeredPluginsPluginChannels.ContainsKey(channel))
            {
                List<Plugin> registeredBots = registeredPluginsPluginChannels[channel];
                registeredBots.RemoveAll(item => object.ReferenceEquals(item, bot));
                if (registeredBots.Count == 0)
                {
                    registeredPluginsPluginChannels.Remove(channel);
                }
            }
        }
        #endregion
    }
}
