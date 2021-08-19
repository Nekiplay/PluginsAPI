using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PluginsAPI
{
    public abstract class Plugin
    {

        #region Системное
        private PluginClient _handler = null;

        private PluginClient Handler
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
        public void SetHandler(PluginClient handler) { this._handler = handler; }
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

            if (Handler.OnPluginUnload != null)
            {
                Handler.OnPluginUnload();
            }
        }
        protected void UnLoadPlugin()
        {
            UnLoadPlugin(this);
        }
        protected void RunScript(string filename)
        {
            Handler.PluginLoad(new Script(filename));
        }
        #endregion

        #region Ивенты плагина

        public virtual void Initialize() { }

        public virtual void Update() { }

        public virtual void ReceivedObject(object s) { }

        public virtual void ReceivedObject<T>(T s) { }
        #endregion

        #region Методы плагина

        protected void PluginPostObject(object obj)
        {
            Handler.OnPluginPostObjectMethod(obj);
        }
        #endregion
    }
}
