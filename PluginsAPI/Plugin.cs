using System;
using System.Collections.Generic;

namespace PluginsAPI
{
    public abstract class Plugin
    {

        #region Системное
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
        public void SetHandler(Client handler) { this._handler = handler; }
        private Plugin master = null;
        protected void SetMaster(Plugin master) { this.master = master; }
        #endregion

        #region Загрузка и выгрузка плагина
        protected void LoadPlugin(Plugin bot) 
        { 
            Handler.PluginUnLoad(bot); Handler.PluginLoad(bot);
        }
        protected void UnLoadPlugin(Plugin bot)
        {
            Handler.PluginUnLoad(bot);

            if (Handler.OnUnloadPlugin != null)
            {
                Handler.OnUnloadPlugin();
            }
        }
        protected void UnLoadPlugin()
        {
            UnLoadPlugin(this);
        }
        protected void RunScript(string filename, string playername = null, Dictionary<string, object> localVars = null)
        {
            Handler.PluginLoad(new Script(filename, playername, localVars));
        }
        #endregion

        #region Ивенты плагина

        public virtual void Initialize() { }
        #endregion

        #region Методы плагина

        protected void PluginPostObject(object obj)
        {
            Handler.PluginPostObject(obj);
        }
        #endregion
    }
}
