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
        }

        /// <summary>
        /// Loads all the plugins in the plugins path to memory.
        /// <para>Called during startup.</para>
        /// </summary>
        public void LoadAllPlugins()
        {

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
    }
}