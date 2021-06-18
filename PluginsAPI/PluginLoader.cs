﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PluginsAPI
{
    public class PluginLoader
    {
        private static readonly Dictionary<ulong, Assembly> CompileCache = new Dictionary<ulong, Assembly>();

        /// <summary>
        /// Run the specified C# script file
        /// </summary>
        /// <param name="apiHandler">ChatBot handler for accessing ChatBot API</param>
        /// <param name="lines">Lines of the script file to run</param>
        /// <param name="args">Arguments to pass to the script</param>
        /// <param name="localVars">Local variables passed along with the script</param>
        /// <param name="run">Set to false to compile and cache the script without launching it</param>
        /// <exception cref="CSharpException">Thrown if an error occured</exception>
        /// <returns>Result of the execution, returned by the script</returns>
        public static object Run(Plugin apiHandler, string[] lines, string[] args, Dictionary<string, object> localVars, bool run = true)
        {
            //Script hash for determining if it was previously compiled
            ulong scriptHash = QuickHash(lines);
            Assembly assembly = null;

            //No need to compile two scripts at the same time
            lock (CompileCache)
            {
                ///Process and compile script only if not already compiled
                if (!CompileCache.ContainsKey(scriptHash))
                {
                    //Process different sections of the script file
                    bool scriptMain = true;
                    List<string> script = new List<string>();
                    List<string> extensions = new List<string>();
                    List<string> libs = new List<string>();
                    List<string> dlls = new List<string>();
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("//using"))
                        {
                            libs.Add(line.Replace("//", "").Trim());
                        }
                        else if (line.StartsWith("//dll"))
                        {
                            dlls.Add(line.Replace("//dll ", "").Trim());
                        }
                        else if (line.StartsWith("//Script"))
                        {
                            if (line.EndsWith("Extensions"))
                                scriptMain = false;
                        }
                        else if (scriptMain)
                            script.Add(line);
                        else extensions.Add(line);
                    }

                    //Add return statement if missing
                    if (script.All(line => !line.StartsWith("return ") && !line.Contains(" return ")))
                        script.Add("return null;");

                    //Generate a class from the given script
                    string code = String.Join("\n", new string[]
                    {
                        "using System;",
                        "using System.Collections.Generic;",
                        "using System.Text.RegularExpressions;",
                        "using System.Linq;",
                        "using System.Text;",
                        "using System.IO;",
                        "using System.Net;",
                        "using System.Threading;",
                        "using PluginsAPI;",
                        String.Join("\n", libs),
                        "namespace ScriptLoader {",
                        "public class Script {",
                        "public CSharpAPI MCC;",
                        "public object __run(CSharpAPI __apiHandler, string[] args) {",
                            "this.MCC = __apiHandler;",
                            String.Join("\n", script),
                        "}",
                            String.Join("\n", extensions),
                        "}}"
                    });

                    //Compile the C# class in memory using all the currently loaded assemblies
                    CSharpCodeProvider compiler = new CSharpCodeProvider();
                    CompilerParameters parameters = new CompilerParameters();
                    parameters.ReferencedAssemblies
                        .AddRange(AppDomain.CurrentDomain
                                .GetAssemblies()
                                .Where(a => !a.IsDynamic)
                                .Select(a => a.Location).ToArray());
                    parameters.CompilerOptions = "/t:library";
                    parameters.GenerateInMemory = true;
                    parameters.ReferencedAssemblies.AddRange(dlls.ToArray());
                    //Console.WriteLine(code);
                    CompilerResults result = compiler.CompileAssemblyFromSource(parameters, code);

                    for (int i = 0; i < result.Errors.Count; i++)
                    {
                        throw new CSharpException(CSErrorType.LoadError,
                            new InvalidOperationException(result.Errors[i].ErrorText + " | " + result.Errors[i].Line));
                    }

                    //Retrieve compiled assembly
                    assembly = result.CompiledAssembly;
                    CompileCache[scriptHash] = result.CompiledAssembly;
                }
                    assembly = CompileCache[scriptHash];
            }

            //Run the compiled assembly with exception handling
            if (run)
            {
                try
                {
                    object compiledScript = assembly.CreateInstance("ScriptLoader.Script");
                    return
                        compiledScript
                        .GetType()
                        .GetMethod("__run")
                        .Invoke(compiledScript,
                            new object[] { new CSharpAPI(apiHandler, localVars), args });
                }
                catch (Exception e) { throw new CSharpException(CSErrorType.RuntimeError, e); }
            }
            else return null;
        }
        private static ulong QuickHash(string[] lines)
        {
            ulong hashedValue = 3074457345618258791ul;
            for (int i = 0; i < lines.Length; i++)
            {
                for (int j = 0; j < lines[i].Length; j++)
                {
                    hashedValue += lines[i][j];
                    hashedValue *= 3074457345618258799ul;
                }
                hashedValue += '\n';
                hashedValue *= 3074457345618258799ul;
            }
            return hashedValue;
        }
        public enum CSErrorType { FileReadError, InvalidScript, LoadError, RuntimeError };

        public class CSharpException : Exception
        {
            private CSErrorType _type;
            public CSErrorType ExceptionType { get { return _type; } }
            public override string Message { get { return InnerException.Message; } }
            public override string ToString() { return InnerException.ToString(); }
            public CSharpException(CSErrorType type, Exception inner)
                : base(inner != null ? inner.Message : "", inner)
            {
                _type = type;
            }
        }

    }
    public class CSharpAPI : Plugin
    {
        public CSharpAPI(Plugin apiHandler, Dictionary<string, object> localVars)
        {
            SetMaster(apiHandler);
        }
        new public void LoadPlugin(Plugin bot)
        {
            base.LoadPlugin(bot);
        }

        //public object CallScript(string script, string[] args)
        //{
        //    string[] lines = null;
        //    Plugin.Script.LookForScript(ref script);
        //    try { lines = File.ReadAllLines(script, Encoding.UTF8); }
        //    catch (Exception e) { throw new CSharpException(CSErrorType.FileReadError, e); }
        //    return CSharpRunner.Run(this, lines, args, localVars);
        //}
    }
}