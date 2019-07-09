using Godot;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace RR_Godot.Core.Plugins
{   
    /// <summary>
    /// <para>Loader</para>
    /// <para>Handles all loading and unloading of plugins into memory.</para>
    /// </summary>
    public class Loader
    {

        // We have a separate app domain to allow loading/unloading of plugins easily
        // because you cannot load/unload individual assemblies.
        // Currently not implemented, active TODO item.
        /// <summary>
        /// AppDomain of all the plugins.
        /// </summary>
        /// <value></value>
        AppDomain PluginDomain { get; set; }

        /// <summary>
        /// List of plugins in the plugins directory.
        /// </summary>
        /// <value></value>
        public List<IPlugin> Plugins { get; set; }

        /// <summary>
        /// Absolute path to the plugins directory.
        /// </summary>
        private static string PluginPath;

        /// <summary>
        /// Creates a new Loader.
        /// </summary>
        /// <param name="path">Absolute path to the plugins directory.</param>
        public Loader(string path)
        {
            PluginPath = path;
            Plugins = new List<IPlugin>();
            
            // PluginDomain = AppDomain.CreateDomain("PluginDomain");
        }

        /// <summary>
        /// Loads all the plugins in the plugins path to memory.
        /// <para>Called during startup.</para>
        /// </summary>
        public void LoadAllPlugins()
        {   
            // Sanity check
            if(System.IO.Directory.Exists(PluginPath))
            {
                // Go through each folder and pull out the library files
                // and the config.
                string[] PluginFolders = System.IO.Directory.GetDirectories(PluginPath);
                foreach (string PluginFolder in PluginFolders)
                {
                    PluginDirectory CurrPlugin;
                    try {
                        CurrPlugin = GetPluginDir(PluginFolder);
                    }
                    catch (Exception e)
                    {
                        GD.Print("\t" + e.Message);
                        continue;
                    }

                    foreach (string libFile in CurrPlugin.LibraryFiles)
                    {
                        Assembly.LoadFile(libFile);
                    }
                }
            }
            // TODO: Load all plugin assemblies into a seperate AppDomain
            // to allow for unloading/reloading of plugins.

            Type ImportInterface = typeof(IPlugin);
            // Get all the librarys that have a class that implements IImportPlugin
            // Original code from DukeOfHaren's plugin tutorial
            // https://github.com/dukeofharen/tutorials/blob/master/DotNet.Plugin/DotNet.Plugin.Business/PluginLoader.cs
            Type[] types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => ImportInterface.IsAssignableFrom(p) && p.IsClass)
                .ToArray();

            GD.Print("Loaded " + types.Length + " plugins...");
            foreach (Type type in types)
            {
                IPlugin importPlug = (IPlugin) Activator.CreateInstance(type);
                Plugins.Add(importPlug);
                GD.Print(importPlug.Ready());
            }
        }

        /// <summary>
        /// Deloads all of the plugins from memory.
        /// <para>Called during program exit.</para>
        /// </summary>
        public void DeloadAllPlugins()
        {

        }

        // ---- Helpers -----

        
        /// <summary>
        /// Goes through a plugin folder and gets the library and config file.
        /// </summary>
        /// <param name="path">Absolute path to the plugin folder.</param>
        /// <returns>
        /// <see cref="PluginDirectory"> object containing plugin
        /// information.
        /// </returns>
        private PluginDirectory GetPluginDir(string path)
        {
            PluginDirectory retVal = new PluginDirectory();
            bool foundConfig = false;

            string[] files = System.IO.Directory.GetFiles(path);
            foreach (string file in files)
            {
                if(file.EndsWith(".dll"))
                {
                    retVal.LibraryFiles.Add(file);
                }
                if(file.EndsWith(".ini"))
                {
                    retVal.ConfigFile = file;
                    foundConfig = true;
                }
            }
            // Make sure we found everything before returning.
            if(!foundConfig)
            {
                throw new Exception("No plugin config file found.");
            }
            else if(retVal.LibraryFiles.Capacity == 0)
            {
                throw new Exception("No plugin libraries found.");
            }
            return retVal;
        }

        private string GetNameFromPath(string path)
        {
            string[] SplitPath = path.Split('/');
            return (string) SplitPath.GetValue(SplitPath.Length - 1);
        }
    }
}