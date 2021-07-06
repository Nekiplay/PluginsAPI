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

            if (Handler.OnUnloadPlugin != null)
            {
                Handler.OnUnloadPlugin();
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
        #endregion

        #region Методы плагина

        protected void PluginPostObject(object obj)
        {
            Handler.OnPluginPostObjectMethod(obj);
        }
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int keys);
        [DllImport("User32.dll")]
        static extern void mouse_event(MouseFlags dwFlags, int dx, int dy, int dwData, UIntPtr dwExtraInfo);
        [Flags]
        enum MouseFlags
        {
            Move = 0x0001, LeftDown = 0x0002, LeftUp = 0x0004, RightDown = 0x0008,
            RightUp = 0x0010, Absolute = 0x8000
        };

        protected bool IsKeyPressed(Keys key)
        {
            return GetAsyncKeyState((int)key) != 0;
        }
        protected void LeftClick()
        {
            mouse_event(MouseFlags.LeftDown | MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }
        protected void LeftClick(int delay)
        {
            mouse_event(MouseFlags.LeftDown, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(delay);
            mouse_event(MouseFlags.LeftUp, 0, 0, 0, UIntPtr.Zero);
        }
        protected void RightClick()
        {
            mouse_event(MouseFlags.RightDown | MouseFlags.RightUp, 0, 0, 0, UIntPtr.Zero);
        }
        protected void RightClick(int delay)
        {
            mouse_event(MouseFlags.RightDown, 0, 0, 0, UIntPtr.Zero);
            System.Threading.Thread.Sleep(delay);
            mouse_event(MouseFlags.RightUp, 0, 0, 0, UIntPtr.Zero);
        }
        protected void MouseMove(int x, int y)
        {
            mouse_event(MouseFlags.Move, x, y, 0, UIntPtr.Zero);
        }
        protected void AbsoluteMouseMove(int x, int y)
        {
            mouse_event(MouseFlags.Move | MouseFlags.Absolute, x, y, 0, UIntPtr.Zero);
        }
        protected System.Diagnostics.Process GetActiveProcess()
        {
            IntPtr hwnd = GetForegroundWindow();
            uint pid;
            GetWindowThreadProcessId(hwnd, out pid);
            System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById((int)pid);
            return p;
        }
        #endregion
    }
}
