using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PluginsAPI
{
    public class Script : Plugin
    {
        private string file;
        private string[] lines = new string[0];
        private string[] args = new string[0];
        private int sleepticks = 10;
        private bool csharp;
        private Thread thread;
        private Dictionary<string, object> localVars;

        public Script(string filename)
        {
            ParseArguments(filename);
        }

        public Script(string filename, string ownername, Dictionary<string, object> localVars)
            : this(filename)
        {
            this.localVars = localVars;
        }

        private void ParseArguments(string argstr)
        {
            List<string> args = new List<string>();
            StringBuilder str = new StringBuilder();

            bool escape = false;
            bool quotes = false;

            foreach (char c in argstr)
            {
                if (escape)
                {
                    if (c != '"')
                        str.Append('\\');
                    str.Append(c);
                    escape = false;
                }
                else
                {
                    if (c == '\\')
                        escape = true;
                    else if (c == '"')
                        quotes = !quotes;
                    else if (c == ' ' && !quotes)
                    {
                        if (str.Length > 0)
                            args.Add(str.ToString());
                        str.Clear();
                    }
                    else str.Append(c);
                }
            }

            if (str.Length > 0)
                args.Add(str.ToString());

            if (args.Count > 0)
            {
                file = args[0];
                args.RemoveAt(0);
                this.args = args.ToArray();
            }
            else file = "";
        }

        public static bool LookForScript(ref string filename)
        {
            //Automatically look in subfolders and try to add ".txt" file extension
            char dir_slash = Client.isUsingMono ? '/' : '\\';
            string[] files = new string[]
            {
                filename,
                filename + ".txt",
                filename + ".cs",
                "scripts" + dir_slash + filename,
                "scripts" + dir_slash + filename + ".txt",
                "scripts" + dir_slash + filename + ".cs",
                "config" + dir_slash + filename,
                "config" + dir_slash + filename + ".txt",
                "config" + dir_slash + filename + ".cs",
            };

            foreach (string possible_file in files)
            {
                if (System.IO.File.Exists(possible_file))
                {
                    filename = possible_file;
                    return true;
                }
            }

            string caller = "Script";
            try
            {
                StackFrame frame = new StackFrame(1);
                MethodBase method = frame.GetMethod();
                Type type = method.DeclaringType;
                caller = type.Name;
            }
            catch { }

            return false;
        }

        public override void Initialize()
        {
            //Load the given file from the startup parameters
            if (LookForScript(ref file))
            {
                lines = System.IO.File.ReadAllLines(file, Encoding.UTF8);
                //foreach (string l in lines)
                //{
                //    Console.WriteLine(l);
                //}
                csharp = file.EndsWith(".cs");
                thread = null;

            }
            else
            {
                UnLoadPlugin();
            }

            if (csharp) //C# compiled script
            {
                //Initialize thread on first update
                if (thread == null)
                {
                    thread = new Thread(() =>
                    {
                        //try
                        //{
                            PluginLoader.Run(this, lines, args, localVars);
                        //}
                        //catch (CSharpException e)
                        //{
                        //    Console.WriteLine("Ошибка бота");
                        //}
                    });
                    thread.Name = "Plugin Script - " + file;
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
