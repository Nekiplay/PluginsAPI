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

namespace Tests
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Client client = new Client();
            client.PluginLoad(new PluginsAPI.Plugins.Script(@"C:\Users\Herob\source\repos\PluginsAPI\Tests\bin\Debug\Test.cs"));
            //client.PluginLoad(new Test());
        }
    }
    public class Test : Plugin
    {
        public override void Initialize()
        {
            Console.WriteLine("Кек");
        }
    }
}
