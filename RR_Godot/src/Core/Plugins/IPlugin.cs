using System.Collections.Generic;

namespace RR_Godot.Core.Plugins
{   
    public class PluginDirectory
    {
        public List<string> LibraryFiles { get; set; }
        public string ConfigFile { get; set; }

        public PluginDirectory()
        {
            LibraryFiles = new List<string>();
        }
    }

    /// <summary>
    /// <para>IPlugin</para>
    /// <para>Base interface for all plugins.</para>
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Config.ini file for the plugin.
        /// </summary>
        /// <value></value>
        string ConfigFile { get; }

        /// <summary>
        /// Shared library files for the plugin.
        /// </summary>
        /// <value></value>
        string LibraryFile { get; }

        /// <summary>
        /// Name of the plugin.
        /// </summary>
        /// <value></value>
        string Name { get;}
    }
}