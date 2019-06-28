namespace RR_Godot.Core.Plugins
{
    /// <summary>
    /// <para>IImportPlugin</para>
    /// <para>Interface to extend for creating plugin that supports importing
    /// of a new file type.</para>
    /// </summary>
    public interface IImportPlugin : IPlugin
    {
        /// <summary>
        /// <para>Recognized file extensions that this plugin can import</para>
        /// </summary>
        /// <value></value>
        string[] Extensions { get; set; }

        /// <summary>
        /// <para>Called after GUI is loaded</para>
        /// </summary>
        string Ready();

        /// <summary>
        /// <para>Called when a supported extension is trying to be imported.</para>
        /// <para>This is where the meat of the plugin comes from.</para>
        /// </summary>        
        void Import();
    }

}