using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PluginsAPI
{
    public class Client
    {
        private readonly Dictionary<string, List<Plugin>> registeredPluginsPluginChannels = new Dictionary<string, List<Plugin>>();
        private readonly List<Plugin> plugins = new List<Plugin>();
        public Action OnUnloadPlugin { set; get; }

        #region Получение и отправка данных от плагина
        public Action<object> OnPluginPostObject { set; get; }
        public void PluginPostObject(object ob)
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
                DispatchPluginEvent(bot => bot.Initialize(), new Plugin[] { b });
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

        #region Системное
        public static bool isUsingMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
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
