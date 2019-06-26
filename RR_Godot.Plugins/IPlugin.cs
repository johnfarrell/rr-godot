namespace RR_Godot.Plugins
{   
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