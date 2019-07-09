# Plugins

Tutorial on how to get started with the RR_Godot plugin framework

### Requirements

For the time being, plugins are required to be written in C# and compiled as a  Dynamic Link Library (.dll). There are plans to expand this to Python, C++, and the other languages supported by Robot Raconteur.

You will need the windows C-Sharp Compiler invoked by calling ```csc``` in a command-line. This is built in with the .NET framework.
For Windows, a tutorial be found by going [here](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-options/command-line-building-with-csc-exe).

If you are on Linux, you will need to install the Mono framework by going to [this](https://www.mono-project.com/download/stable/) and following the instructions for your distro. After that, you should be able to call ```csc``` in a terminal.

### Plugin types

There are a variety of plugin types that one can create, and each have a specific set of functionality exposed to them. Currently, there is only 1 plugin type.

1. ImportPlugin

You should import the plugin type for what you want your plugin to do. For instance, if you want to add support for importing a new filetype, implement the ImportPlugin type.

### Basic Plugin

To get started, make sure you have the ```plugins.dll``` file that can be found [here](#). This exposes basic functionality to all plugins.

Create a new file called ```[plugin_name].cs```. This is where the main plugin logic will be written.

A bare-bones ImportPlugin will look like this:

```C#
// SampleImporter.cs
using RR_Godot.Core.Plugins;
using RR_Godot.Core.Geometry;

namespace SampleImportPlugin
{
    public class SampleImporter : IImportPlugin
    {
        // Universal plugin settings
        public string LibraryFile { get; }
        public string ConfigFile { get; }
        public string Name { get; }

        public string[] Extensions { get; }

        public SampleImporter()
        {
            LibraryFile = "sample_library.dll";
            ConfigFile = "sample_config.ini";
            Extensions = new string[1] { "test" };
            Name = "Sample Importer";
        }

        public string Ready()
        {
            // code..
        }

        public void Import(string filePath)
        {
            // code...
        }
    }
}
```
##### Code Explanation

```C#
using RR_Godot.Core.Plugins;
using RR_Godot.Core.Geometry;
```
RR_Godot.Core.Plugins contains the necessary functionality for every plugin.
RR_Godot.Core.Geometry on the other hand gives the plugin access to the necessary functions for creating and managing meshes, important for importing 3D files.

```C#
namespace SampleImportPlugin
{
```
Every plugins namespace should be the name of the plugin.

```C#
    public class SampleImporter : IImportPlugin
    {
```
Since we are creating a plugin that adds support for importing a new file, our plugin class needs to extend the IImportPlugin interface. This allows the plugin to access functionality specific to import plugins

```C#
        // Universal plugin settings
        public string LibraryFile { get; }
        public string ConfigFile { get; }
        public string Name { get; }
```
These three parameters are required by every plugin. They are used in the creation and management of the plugin by the RR_Godot system.

```C#
        public string[] Extensions { get; set; }
```
This is a property exposed by the IImportPlugin interface. This is where the supported filetypes are stored.

```C#
        public SampleImporter()
        {
            LibraryFile = "sample_library.dll";
            ConfigFile = "sample_config.ini";
            Extensions = new string[1] { "test" };
            Name = "Sample Importer";
        }
```
The constructor is where all the class members are filled out. For an ImportPlugin, the ```Extensions``` setting is one of the most important.
```C#
        public void Ready()
        {

        }
```
The ```Ready()``` function is called when the plugins context is loaded. For an import function, this is when the GUI is loaded.

```C#
        public void Import(string filePath)
        {
            // code...
        }
```

```Ready()``` is called when the plugin is loaded into memory. If you want custom startup functionality, add it here.
```Import(string filePath)``` is where the bulk of the logic for import functions happens. It is passed an absolute path to the file that is being imported, and it is where the file parsing and translating into something RR_Godot can use happens.

### Compiling the plugin
Once you have finished writing your plugin, compile it with the following command;
```
csc /t:library /out:[Plugin_name].dll /r:Plugins.dll [Plugin_name].cs
```

### Loading the plugin
Now that you have the plugin library file, it is time to get RR_Godot to recognize it. 

1. Navigate to the RR_Godot plugins folder. On Linux this can be found at ```~/.local/share/rr-godot/plugins/``` and on Windows it can be found at ```%APPDATA%/rr-godot/plugins/```. 
2. Create a folder for the plugin called ```[Plugin_name]```. Notice the underscore in place of a space.
3. Copy the ```[Plugin_name].dll``` file created earlier into this folder.
4. Create a file called ```settings.ini```. 
TODO: Go through settings.ini creation

Now that the ```.dll``` and ```.ini``` are in the plugins folder, RR_Godot can find them. Open the program, and navigate to ```File->Preferences->Plugins``` and hit the refresh button. This will reload all plugins, and now that our custom plugin is in the plugins directory, RR_Godot will load it.