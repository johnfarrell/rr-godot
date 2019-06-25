using Godot;
using System;
using System.Collections.Generic;
using RR_Godot;

class Global : Node
{
    /// <summary>
    /// Godot.Directory object containing the plugin directory.
    /// </summary>
    private Directory PluginDir;

    public Config UserConfig;

    /// <summary>
    /// Holds all the currently loaded plugins.
    /// </summary>
    private Plugin<Type>[] Plugins;

    /// <summary>
    /// Aboslute path of the user data directory.
    /// </summary>
    private string UserDataDirectory;

    public override void _Ready()
    {
        UserDataDirectory = OS.GetUserDataDir();

        // Register close button so we can save the program before quitting
        GetNode("/root/main/UI/TitleBar/TitleButtons/QuitButton").Connect(
                "pressed", this, "OnQuitRequest");

        UserConfig = new Config();

        // Populate config with sample settings, will be removed later
        UserConfig.AddPlugin("Plugin 1");
        UserConfig.AddPlugin("Plugin 2");
        UserConfig.AddPlugin("Disabled Plugin");
        UserConfig.SetPluginDisabled("Disabled Plugin");

        // Make sure everything is working properly
        ValidateConfigExistence();
        ParseConfig();
        ValidatePluginDirectory();
        CheckPlugins();

        // Debug prints, will be removed later
        GD.Print("ENABLED PLUGINS: " + UserConfig.GetEnabledPlugins().Length);
        GD.Print("DISABLED PLUGINS: " + UserConfig.GetDisabledPlugins().Length);
        
        GD.Print("GLOBAL SINGLETON LOADED");
    }

    public void OnQuitRequest()
    {
        GD.Print("Saving config");
        UserConfig.Save("user://settings.cfg");
        GetTree().Quit();
    }

    /// <summary>
    /// Makes sure the config file exists
    /// </summary>
    private void ValidateConfigExistence()
    {
        File config = new File();

        if(!config.FileExists("user://settings.cfg"))
        {
            System.IO.File.Create(UserDataDirectory + "/settings.cfg");
        }
    }

    /// <summary>
    /// Sets all the necessary variables from the configuration file
    /// </summary>
    private void ParseConfig()
    {
        File ConfigFile = new File();

        ConfigFile.Open("user://settings.cfg", File.ModeFlags.Read);
        while (!ConfigFile.EofReached())
        {
            var lineString = (string) ConfigFile.GetLine();

            if(lineString == "" || lineString == "null")
            {
                continue;
            }
            UserConfig.PopulateSettings(lineString);

        }
    }

    /// <summary>
    /// Makes sure the plugin directory exists and is accessible
    /// </summary>
    private void ValidatePluginDirectory()
    {
        String PluginFolderPath = "user://addons/plugins";
        PluginDir = new Directory();
        bool PluginPathExists = PluginDir.DirExists(PluginFolderPath);

        if(!PluginPathExists)
        {
            GD.PushWarning("ERROR: Plugin directory does not exist, creating...");
            var err = PluginDir.MakeDirRecursive(PluginFolderPath);
            if(err != Error.Ok)
            {
                GD.PushError("ERROR: Cannot create directory, " + err);
            }
        }

        PluginDir.Open(PluginFolderPath);
        GD.Print("PLUGIN DIR SET TO : " + PluginDir.GetCurrentDir());
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
        PluginDir.ListDirBegin(true);
        string CurrPlugin = PluginDir.GetNext();

        while(CurrPlugin != "")
        {
            CurrPlugin = PluginDir.GetNext();
        }
        
    }
}