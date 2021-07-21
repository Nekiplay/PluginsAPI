using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using static PluginsAPI.PluginLoader;

namespace PluginsAPI
{
    public class WebScript : Plugin
    {
        private string file = "";
        private string[] lines = new string[0];
        private string[] args = new string[0];
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars = new Dictionary<string, object>();

        public WebScript(string url)
        {
            this.file = url;
        }

        public override void Initialize()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                string res = wc.DownloadString(file);

                lines = res.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            };
            csharp = true;
            thread = null;

        }
        public override void Update()
        {
            if (csharp) //C# compiled script
            {
                //Initialize thread on first update
                if (thread == null)
                {
                    thread = new Thread(() =>
                    {
                        Run(this, lines, args, localVars);
                    });
                    thread.Name = "MCC Script - " + file;
                    thread.Start();
                }

                //Unload bot once the thread has finished running
                if (thread != null && !thread.IsAlive)
                {
                    UnLoadPlugin();
                }
            }
        }
    }
}
