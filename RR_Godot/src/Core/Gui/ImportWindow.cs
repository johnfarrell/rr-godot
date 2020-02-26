using Godot;
using System;
using RR_Godot.Core;

public class ImportWindow : FileDialog
{
    private Global GlobalSettings;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get GlobalSettings
        GlobalSettings = GetNode<Global>("/root/Global");

        // Connect basic signals
        this.Connect("file_selected", this, "OnFileSelected");
    }

    public void OnFileSelected(string path)
    {
        File selectedFile = new File();
        selectedFile.Open(path, File.ModeFlags.Read);
        string absolutePath = selectedFile.GetPathAbsolute();
        selectedFile.Close();
        GlobalSettings.ImportFile(absolutePath);
    }

}
