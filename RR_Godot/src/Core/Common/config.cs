using System.Collections.Generic;
using Godot;
using RR_Godot.Core.Plugins;

namespace RR_Godot.Core
{
    /// <summary>
    /// <para>Config</para>
    /// <para>Used to easily create/save the settings config file</para>
    /// </summary>
    public class Config
    {
        /// <summary>
        /// List of the enabled plugin unique names.
        /// </summary>
        public List<string> EnabledPlugins;
        /// <summary>
        /// List of the disabled plugin unique names.
        /// </summary>
        public List<string> DisabledPlugins;

        public List<string> InactivePlugins;

        public Config()
        {
            EnabledPlugins = new List<string>();
            DisabledPlugins = new List<string>();
            InactivePlugins = new List<string>();
        }

        // TODO
        // * Change to use IPlugins instead of strings to create the 
        //   settings.
        /// <summary>
        /// <para>
        /// Populates the enabled plugins list from a dictionary object.
        /// </para>
        /// <para>
        /// Arg is an object due to the way GD.JSON parses the settings file.
        /// Design choice to include all the casting in this function
        /// instead of <see cref="PopulateSettings" />.
        /// </para>
        /// </summary>
        /// <param name="SettingVal">
        /// String array containing the list of plugins to be enabled.
        /// </param>
        public void AddEnabledPluginsSetting(object SettingVal)
        {
            if(!(SettingVal is object[]))
                return;

            object[] PluginList = (object[]) SettingVal;
            foreach (var PluginName in PluginList)
            {
                AddPlugin((string) PluginName);
            }
        }

        // TODO
        // * Change to use IPlugins instead of strings to create the 
        //   settings.
        /// <summary>
        /// <para>
        /// Populates the disabled plugins list from a dictionary value object.
        /// </para>
        /// <para>
        /// Arg is an object for the same reason 
        /// <see cref="AddEnabledPluginsSetting" /> takes an object.</para>
        /// </summary>
        /// <param name="SettingVal">
        /// String array containing the list of plugins to be enabled.
        /// </param>
        public void AddDisabledPluginsSetting(object SettingVal)
        {
            if(!(SettingVal is object[]))
                return;

            object[] PluginList = (object[]) SettingVal;
            foreach (var PluginName in PluginList)
            {
                AddPlugin((string) PluginName);
                SetPluginDisabled((string) PluginName);
            }
        }

        public void AddInactivePluginAfterRefresh(string PluginName)
        {
            InactivePlugins.Add(PluginName);
        }

        /// <summary>
        /// Sets all the class variables according to the config file
        /// </summary>
        /// <param name="JsonString">
        /// JSON formatted string of the settings.cfg file
        /// </param>
        public void PopulateSettings(string JsonString)
        {
            // Parse the config file and quick sanity check to
            // make sure its a dictionary.
            var setting = JSON.Parse(JsonString).Result;
            if(!(setting is Godot.Collections.Dictionary))
            {
                return;
            }

            // Go through the JSON file and add the necessary items
            Godot.Collections.Dictionary nsetting = 
                (Godot.Collections.Dictionary) setting;
            foreach (System.Collections.Generic.KeyValuePair<object, object> entry in nsetting)
            {
                switch (entry.Key)
                {
                    case "enabled_plugins":
                        AddEnabledPluginsSetting(entry.Value);
                        break;
                    case "disabled_plugins":
                        AddDisabledPluginsSetting(entry.Value);
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// <para>Adds a plugin to the list of enabled plugins if it is not
        /// already added.</para>
        /// </summary>
        /// <param name="PluginName">Name of the plugin to add.</param>
        public void AddPlugin(string PluginName)
        {
            if(!EnabledPlugins.Contains(PluginName)
                && !DisabledPlugins.Contains(PluginName))
            {
                EnabledPlugins.Add(PluginName);
            }
        }

        /// <summary>
        /// <para>Removes a plugin if it is either enabled or disabled.</para>
        /// </summary>
        /// <param name="PluginName">Name of the plugin to remove.</param>
        public void RemovePlugin(string PluginName)
        {
            EnabledPlugins.Remove(PluginName);
        }

        // TODO
        // * Make it take in the name of the plugin as a string
        //   and do a search by name for the plugin.
        /// <summary>
        /// Moves a plugin from the disabled list to the enabled list.
        /// </summary>
        /// <param name="PluginName">The plugin to enable.</param>
        public void SetPluginEnabled(string PluginName)
        {
            if(DisabledPlugins.Contains(PluginName)
                && !EnabledPlugins.Contains(PluginName))
            {
                EnabledPlugins.Add(PluginName);
                DisabledPlugins.Remove(PluginName);
            }
        }

        // TODO
        // * Make it take in the name of the plugin as a string
        //   and do a search by name for the plugin.
        /// <summary>
        /// Moves a plugin from the enabled list to the disabled list.
        /// </summary>
        /// <param name="PluginName">The plugin to disable.</param>
        public void SetPluginDisabled(string PluginName)
        {
            if(EnabledPlugins.Contains(PluginName)
                && !DisabledPlugins.Contains(PluginName))
            {
                DisabledPlugins.Add(PluginName);
                EnabledPlugins.Remove(PluginName);
            }
        }

        /// <summary>
        /// Retrieves the list of enabled plugins.
        /// </summary>
        /// <returns>
        /// An array containing the names of the plugins that are enabled.
        /// </returns>
        public string[] GetEnabledPlugins()
        {
            return EnabledPlugins.ToArray();
        }

        /// <summary>
        /// Retrieves the list of disabled plugins.
        /// </summary>
        /// <returns>
        /// An array containing the names of the plugins that are disabled.
        /// </returns>
        public string[] GetDisabledPlugins()
        {
            return DisabledPlugins.ToArray();
        }

        public string[] GetInactivePlugins()
        {
            return InactivePlugins.ToArray();
        }

        /// <summary>
        /// Creates a JSON-parseable dictionary of all config information.
        /// </summary>
        /// <returns>Dictionary of config keys and values.</returns>
        public Dictionary<object, object> CreateSettingsDictionary()
        {
            return new Dictionary<object, object>()
            {
                {"enabled_plugins", GetEnabledPlugins() },
                {"disabled_plugins", GetDisabledPlugins() },
            };
        }

        public void Save(string ConfigPath)
        {
            var configFile = new File();
            configFile.Open(ConfigPath, File.ModeFlags.Write);

            var ConfigDict = CreateSettingsDictionary();

            configFile.StoreLine(JSON.Print(ConfigDict));
            configFile.Close();
        }
    }
}