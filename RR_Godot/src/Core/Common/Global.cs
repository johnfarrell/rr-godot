using Godot;
using System;
using System.Reflection;
using RR_Godot.Core.Plugins;
using RR_Godot.Core.Urdf;
using RosSharp.Urdf;

namespace RR_Godot.Core
{
    public class Global : Node
    {
        /// <summary>
        /// Godot.Directory object containing the plugin directory.
        /// </summary>
        private Directory PluginDir;

        private UrdfImporter UrdfHandler;

        private Loader PlugLoader;

        public ConfigFile UserConfig;

        /// <summary>
        /// Aboslute path of the user data directory.
        /// </summary>
        private string UserDataDirectory;

        private string width;
        private string height;

        private string plugin_directory;
        private string[] enabled_plugins;
        private string[] disabled_plugins;


        public override void _Ready()
        {
            UrdfHandler = new UrdfImporter();

            UserDataDirectory = OS.GetUserDataDir();

            PlugLoader = new Loader(UserDataDirectory + "/plugins/");

            // Register close button so we can save the program before quitting
            GetNode("/root/main/UI/TitleBar/TitleButtons/QuitButton").Connect(
                    "pressed", this, "OnQuitRequest");

            UserConfig = new ConfigFile();

            // Make sure everything is working properly
            ParseConfig();

            ValidatePluginDirectory();
            CheckPlugins();

            GD.Print("GLOBAL SINGLETON LOADED");
        }

        public void OnQuitRequest()
        {
            GD.Print("Saving config");
            UserConfig.Save("user://settings.cfg");
            GetTree().Quit();
        }

        /// <summary>
        /// Sets all the necessary variables from the configuration file
        /// </summary>
        private void ParseConfig()
        {
            Error err = UserConfig.Load("user://settings.cfg");

            if (err != Error.Ok)
            {
                // Config file doesn't exist, create and populate

                // Display
                UserConfig.SetValue("display", "width", 1920);
                UserConfig.SetValue("display", "height", 1080);

                // Plugins
                UserConfig.SetValue("plugins", "plugin_directory", UserDataDirectory + "/plugins/");
                UserConfig.SetValue("plugins", "enabled_plugins", new string[0]);
                UserConfig.SetValue("plugins", "disabled_plugins", new string[0]);
            }
            else
            {
                SetValueFromConfig("display", "width");
                SetValueFromConfig("display", "height");

                SetValueFromConfig("plugins", "plugin_directory");
                SetValueFromConfig("plugins", "enabled_plugins");
                SetValueFromConfig("plugins", "disabled_plugins");
            }
        }

        public void SetValueFromConfig(string section, string key)
        {
            if (UserConfig.HasSectionKey(section, key))
            {
                Set(key, UserConfig.GetValue(section, key));
            }
        }

        /// <summary>
        /// Makes sure the plugin directory exists and is accessible
        /// </summary>
        private void ValidatePluginDirectory()
        {
            String PluginFolderPath = UserDataDirectory + "/plugins/";
            bool PluginPathExists = System.IO.Directory.Exists(PluginFolderPath);

            if (!PluginPathExists)
            {
                GD.PushWarning("ERROR: Plugin directory does not exist, creating...");
                var err = PluginDir.MakeDirRecursive(PluginFolderPath);
                if (err != Error.Ok)
                {
                    GD.PushError("ERROR: Cannot create directory, " + err);
                }
            }
            PluginDir = new Directory();
            var err2 = PluginDir.ChangeDir(PluginFolderPath);
            var curDir = PluginDir.GetCurrentDir();
            GD.Print("PLUGIN DIR SET TO : " + curDir);
        }

        /// <summary>
        /// Gets the path of the current plugin directory
        /// </summary>
        /// <returns>String containing the absolute path.</returns>
        public string GetPluginDirectoryPath()
        {
            return PluginDir.GetCurrentDir();
        }

        public void CheckPlugins()
        {
            PlugLoader.LoadAllPlugins();
            PopulatePluginSettings();
        }

        public void PopulatePluginSettings()
        {
            string[] plugList = new string[PlugLoader.Plugins.Count];
            int i = 0;
            foreach (IPlugin plug in PlugLoader.Plugins)
            {
                plugList[i] = plug.Name;
                ++i;
            }
            UserConfig.SetValue("plugins", "enabled_plugins", plugList);
        }

        public void CheckForNewPlugins()
        {
            string[] PluginFolders = System.IO.Directory
                .GetDirectories(UserDataDirectory + "/plugins/");
            foreach (string Folder in PluginFolders)
            {
                GD.Print("Found plugin folder: " + Folder);
                var FolderName = System.IO.Path.GetFileName(Folder);
                if (!IsPluginFolderLoaded(FolderName))
                {
                    // UserConfig.AddInactivePluginAfterRefresh(FolderName);
                }
            }
        }

        /// <summary>
        /// Checks to see if a plugin is loaded based off a folder name.
        /// </summary>
        /// <param name="PluginDirName">
        /// Name of the folder in question.
        /// </param>
        /// <returns>
        /// True if there is a plugin loaded that matches the given folder name.
        /// </returns>
        private bool IsPluginFolderLoaded(string PluginDirName)
        {
            foreach (IPlugin plug in PlugLoader.Plugins)
            {
                if (PluginNameMatchesFolder(plug.Name, PluginDirName))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks to see if a specified plugin name matches a specified plugin directory
        /// given the specified naming conventions for plugins.
        /// </summary>
        /// <param name="PluginName">
        /// Name property of the plugin.
        /// </param>
        /// <param name="PluginFolderName">
        /// Name of the directory where plugin files are located.
        /// </param>
        /// <returns>True if the plugin matches naming conventions, false if not</returns>
        private bool PluginNameMatchesFolder(string PluginName, string PluginFolderName)
        {
            string[] plugNameParts = PluginName.Split(' ');
            string[] plugDirParts = PluginFolderName.Split('_');

            // If the split names aren't equal in length, the plugin doesn't
            // match naming conventions. 
            if (plugNameParts.Length != plugDirParts.Length)
            {
                return false;
            }
            for (int i = 0; i < plugNameParts.Length; ++i)
            {
                if (plugNameParts[i].ToLower() != plugDirParts[i].ToLower())
                {
                    return false;
                }
            }
            return true;
        }

        // TODO: Functionalize this and probably move it to plugloader or a different
        // class.
        /// <summary>
        /// Searches for a plugin that can import a certain file extension
        /// and calls that plugins import function.
        /// </summary>
        /// <param name="file"></param>
        public void ImportFile(string file)
        {
            GD.Print("IMPORTING " + file);
            string fileExtension = System.IO.Path.GetExtension(file);
            GD.Print(fileExtension);
            if (fileExtension == ".urdf")
            {
                UrdfHandler.Parse(file);
            }
            // foreach (IPlugin plug in PlugLoader.Plugins)
            // {
            //     GD.Print("Testing " + plug.Name);
            //     if (true)
            //     {
            //         try
            //         {
            //             IImportPlugin temp = (IImportPlugin)plug;

            //             GD.Print("Found IImportPlugin " + temp.Name);
            //             foreach (String ext in temp.Extensions)
            //             {

            //                 if (ext == fileExtension)
            //                 {
            //                     GD.Print("Calling " + temp.Name + " to import.");
            //                     temp.Import(file);
            //                 }
            //             }
            //         }
            //         catch (Exception e)
            //         {
            //             GD.Print(e);
            //         }

            //     }
            // }
        }
    }
}