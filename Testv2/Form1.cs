using PluginsAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testv2
{
    public partial class Form1 : Form
    {
        static PluginUpdater updater = new PluginUpdater();
        PluginClient auto_updater_plugin_client = new PluginClient(updater);
        Script auto_updater_script = new Script(@"Updater.cs");
        public Form1()
        {
            InitializeComponent();

            auto_updater_plugin_client.OnPluginPostObject += OnPluginReceivedObject;
            auto_updater_plugin_client.PluginLoad(auto_updater_script);
        }
        private void OnPluginReceivedObject(object obj)
        {
            if (obj.GetType() == typeof(TextBox))
            {
                this.Invoke((MethodInvoker)(() => this.Controls.Add((TextBox)obj)));
            }
            else if (obj.GetType() == typeof(Label))
            {
                this.Invoke((MethodInvoker)(() => this.Controls.Add((Label)obj)));
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
