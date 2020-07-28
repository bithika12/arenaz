using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text.RegularExpressions;

public class KeywordReplace : UnityEditor.AssetModificationProcessor
{

    public static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        int index = path.LastIndexOf(".");
        string file = path.Substring(index);
        if (file != ".cs" && file != ".js" && file != ".boo") return;
        index = Application.dataPath.LastIndexOf("Assets");
        path = Application.dataPath.Substring(0, index) + path;
        file = System.IO.File.ReadAllText(path);

        file = file.Replace("#CREATIONDATE#", System.DateTime.Now + "");
        file = file.Replace("#PROJECTNAME#", Regex.Replace(PlayerSettings.productName, @"[^a-zA-Z]+", "")); // Regex.Replace(PlayerSettings.productName, @"\s+", "")
        file = file.Replace("#SMARTDEVELOPERS#", PlayerSettings.companyName);

        System.IO.File.WriteAllText(path, file);
        AssetDatabase.Refresh();
    }
}