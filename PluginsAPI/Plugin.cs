using System;
using System.Collections.Generic;

namespace PluginsAPI
{
    public abstract class Plugin
    {
        private Client _handler = null;
        private Client Handler
        {
            get
            {
                if (master != null)
                    return master.Handler;
                if (_handler != null)
                    return _handler;
                throw new InvalidOperationException("Error");
            }
        }
        protected void LoadPlugin(Plugin bot) 
        { 
            Handler.PluginUnLoad(bot); Handler.PluginLoad(bot); 
        }
        protected void UnLoadPlugin(Plugin bot)
        {
            Handler.PluginUnLoad(bot);
        }
        public void SetHandler(Client handler) { this._handler = handler; }
        private Plugin master = null;
        protected void SetMaster(Plugin master) { this.master = master; }

        public virtual void Initialize() { }

        protected void RunScript(string filename, string playername = null, Dictionary<string, object> localVars = null)
        {
            Handler.PluginLoad(new Script(filename, playername, localVars));
        }


        protected void UnLoadPlugin()
        {
            Handler.PluginUnLoad(this);
            if (Handler.OnUnloadPlugin.Count != 0)
            {
                foreach (Action unloadaction in Handler.OnUnloadPlugin)
                {
                    unloadaction();
                }
            }
        }
    }
}
