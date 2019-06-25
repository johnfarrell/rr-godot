using System;
using Godot;

namespace RR_Godot
{
    /// <summary>
    /// <para></para>
    /// </summary>
    public abstract class Plugin<T>
    {
        /// <summary>
        /// <para>Configuration file of the plugin in .json format</para>
        /// </summary>
        string ConfigFile;

        /// <summary>
        /// <para>Reference to the shared library of the plugin.</para>
        /// </summary>
        string LibraryFile;

        /// <summary>
        /// <para>Unique identifier name of the plugin used internally.</para>
        /// </summary>
        string HandlerName;

        /// <summary>
        /// <para>Human readable name of the plugin, used for display.</para>
        /// </summary>
        string ReadableName; 

        /// <summary>
        /// <para>Creates the plugin.</para>
        /// <param name="FileName">Name of the shared library for the plugin.</param>
        /// <param name="Config">Name of the config file for the plugin.</param>
        /// </summary>
        abstract public void Create(string FileName, string Config);

        /// <summary>
        /// <para>Registers a plugin with the plugin system.</para>
        /// </summary>
        public void Register(T Plugin)
        {
            // TODO
        }

        /// <summary>
        /// <para>Unregisters a plugin with the plugin system.</para>
        /// </summary>
        public void Unregister(T Plugin)
        {
            // TODO
        }

        /// <summary>
        /// <para>Enables the plugin for use.</para>
        /// </summary>
        public void Enable(T Plugin)
        {
            // TODO
        }

        /// <summary>
        /// <para>Disables the plugin while keeping it registered.</para>
        /// <summary>
        public void Disable(T Plugin)
        {
            // TODO
        }

        /// <summary>
        /// <para>Gets the filename of the plugins shared library.</para>
        /// </summary>
        public string GetLibraryFile()
        {
            return this.LibraryFile;
        }

        /// <summary>
        /// <para>Gets the config file for the plugin.</para>
        /// </summary>
        public string GetConfigFile()
        {
            return this.ConfigFile;
        }

        /// <summary>
        /// <para>Gets the unique system name of the plugin.</para>
        /// </summary>
        public string GetSystemName()
        {
            return this.HandlerName;
        }

        /// <summary>
        /// <para>Gets the human readable name of the plugin.</para>
        /// </summary>
        public string GetName()
        {
            return this.ReadableName;
        }
       
    }

    /// <summary>
    /// <para>ImportPlugin</para>
    /// <para>Plugin type to add additional file import support to the program.</para>
    /// </summary>
    public abstract class ImportPlugin : Plugin<ImportPlugin>
    {
        /// <summary>
        /// <para>Recognized extensions that this ImportPlugin can handle.</para>
        /// </summary>
        string[] Extensions;

        /// <summary>
        /// <para>Called after the GUI is loaded, override for custom functionality.</para>
        /// </summary>
        public virtual void Ready() {}

        /// <summary>
        /// <para>Function called for handling the import of the file. This is where the meat
        /// of the plugin happens.</para>
        /// </summary>
        abstract public void Import();

        /// <summary>
        /// <para>Returns a list of strings representing the recognized file extensions of this
        /// importer.</para>
        /// <para>Example: [".stl", ".obj"]</para>
        /// </summary>
        abstract public string[] GetRecognizedExtensions();
    }

    /// <summary>
    /// <para>GUIPlugin</para>
    /// <para>Plugin type to add items to the user interface.</para>
    /// </summary>
    public abstract class GUIPlugin : Plugin<GUIPlugin>
    {
        /// <summary>
        /// Adds a new button to the upper toolbar.
        /// </summary>
        public virtual void AddToolbarItem() {}

        /// <summary>
        /// Adds a new tab to the left menu.
        /// </summary>
        public virtual void AddLeftMenuItem() {}

        /// <summary>
        /// Adds a new setting in the 'File->Preferences' window.
        /// </summary>
        public virtual void AddSettingsMenu() {}
    }
}