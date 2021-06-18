using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluginsAPI
{
    public class Client
    {
        private readonly Dictionary<string, List<Plugin>> registeredPluginsPluginChannels = new Dictionary<string, List<Plugin>>();
        private readonly List<Plugin> plugins = new List<Plugin>();
        public void PluginLoad(Plugin b, bool init = true)
        {
            //if (InvokeRequired)
            //{
            //    InvokeOnMainThread(() => BotLoad(b, init));
            //    return;
            //}

            b.SetHandler(this);
            plugins.Add(b);
            if (init)
                DispatchPluginEvent(bot => bot.Initialize(), new Plugin[] { b });
        }
        public void PluginUnLoad(Plugin b)
        {
            //if (InvokeRequired)
            //{
            //    InvokeOnMainThread(() => BotUnLoad(b));
            //    return;
            //}

            plugins.RemoveAll(item => object.ReferenceEquals(item, b));

            // ToList is needed to avoid an InvalidOperationException from modfiying the list while it's being iterated upon.
            var botRegistrations = registeredPluginsPluginChannels.Where(entry => entry.Value.Contains(b)).ToList();
            foreach (var entry in botRegistrations)
            {
                UnregisterPluginChannel(entry.Key, b);
            }
        }
        public static bool isUsingMono
        {
            get
            {
                return Type.GetType("Mono.Runtime") != null;
            }
        }
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
                    else throw; //ThreadAbortException should not be caught here as in can happen when disconnecting from server
                }
            }
        }
        
        public void UnregisterPluginChannel(string channel, Plugin bot)
        {
            //if (InvokeRequired)
            //{
            //    InvokeOnMainThread(() => UnregisterPluginChannel(channel, bot));
            //    return;
            //}

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
    }
}
