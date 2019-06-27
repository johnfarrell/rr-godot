using Godot;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace RR_Godot.Plugins.Loader
{   
    /// <summary>
    /// <para>Loader</para>
    /// <para>Handles all loading and unloading of plugins into memory.</para>
    /// </summary>
    public class Loader
    {
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
                GD.Print(PluginPath);

                // Go through each folder and pull out the library files
                // and the config.
                string[] PluginFolders = System.IO.Directory.GetDirectories(PluginPath);
                foreach (string PluginFolder in PluginFolders)
                {
                    string PluginName = GetNameFromPath(PluginFolder);
                    GD.Print("Found plugin: " + PluginName);

                    PluginDirectory CurrPlugin;
                    try {
                        CurrPlugin = GetPluginDir(PluginFolder);
                    }
                    catch (Exception e)
                    {
                        GD.Print("\t" + e.Message);
                        GD.Print("\tSkipping...");
                        continue;
                    }
                    
                    // Quick debug print
                    GD.Print("\t- " + CurrPlugin.ConfigFile);
                    foreach (string libFile in CurrPlugin.LibraryFiles)
                    {
                        GD.Print("\t- " + libFile);
                    }
                }
            }
        }

        /// <summary>
        /// Deloads all of the plugins from memory.
        /// <para>Called during program exit.</para>
        /// </summary>
        public void DeloadAllPlugins()
        {

        }

        /// <summary>
        /// Enables plugin that has been disabled.
        /// </summary>
        /// <param name="pluginName">Name of the plugin folder.</param>
        public void EnablePlugin(string pluginName)
        {

        }

        /// <summary>
        /// Disables a plugin that has been loaded into memory.
        /// </summary>
        /// <param name="pluginName">Name of the plugin folder.</param>
        public void DisablePlugin(string pluginName)
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