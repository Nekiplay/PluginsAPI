using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark
{
    public class Benchmark
    {
        [Benchmark]
        public void Generic()
        {
            PluginsAPI.PluginUpdater pluginUpdater = new PluginsAPI.PluginUpdater();
            PluginsAPI.PluginClient pluginClient = new PluginsAPI.PluginClient(pluginUpdater);
            pluginClient.OnPluginPostObject += Generic2;
            pluginClient.PluginLoad(new PluginsAPI.Script("Test.cs"));
        }
        public void Generic2<T>(T r)
        {
            //Console.WriteLine(r);
        }

        [Benchmark]
        public void Test()
        {
            PluginsAPI.PluginUpdater pluginUpdater = new PluginsAPI.PluginUpdater();
            PluginsAPI.PluginClient pluginClient = new PluginsAPI.PluginClient(pluginUpdater);
            pluginClient.OnPluginPostObject += Test2;
            pluginClient.PluginLoad(new PluginsAPI.Script("Test.cs"));
        }
        public void Test2(object r)
        {
            //Console.WriteLine(r);
        }
    }
}
